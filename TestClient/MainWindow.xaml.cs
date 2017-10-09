using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ComponentModel;
using System.Net;
using System.IO;

using UXProject.Data;

namespace TestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Participant participant;
        private Test test;

        public MainWindow()
        {
            InitializeComponent();

            //Puts it at the bottom right of the screen
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            LoginPage login = new LoginPage();
            login.LoggedIn += login_LoggedIn;
            pagecontent.Content = login;

        }

        void login_LoggedIn(object sender, LoggedInEventArgs e)
        {
            this.participant = e.Participant;
            this.test = e.Participant.Test;

            //Create browser object and launch overlay window
            overlay = new Overlay(test);
            overlay.TestStarted += overlay_TestStarted;
            overlay.TestEnded += overlay_TestEnded;
            overlay.NextTask += overlay_NextTask;
            overlay.TestPaused += overlay_TestPaused;
            overlay.TestResumed += overlay_TestResumed;
            pagecontent.Content = overlay;
        }


        void overlay_TestResumed(object sender, TestEventArgs e)
        {
            browser.Resume();
            recorder.Resume();
        }

        void overlay_TestPaused(object sender, TestEventArgs e)
        {
            browser.Pause();
            recorder.Pause();
        }

        #region Test Events + Recording management

        Overlay overlay;
        AwesomiumBrowserWindow browser;
        ScreenCapture.Recorder recorder;
        List<TimeSpan> markers = new List<TimeSpan>();

        List<TaskResult> taskResults = new List<TaskResult>();
        
        void overlay_TestStarted(object sender, TestEventArgs e)
        {
            browser = new AwesomiumBrowserWindow(test.URL);
            browser.Closing += browser_Closing;
            browser.Show();

            string localName = String.Format("Test{0}_Participant{1}.avi", test.TestId, participant.ParticipantId);

            if (Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "/video/"))
                System.IO.Directory.Delete(System.IO.Directory.GetCurrentDirectory() + "/video/", true);

            System.IO.DirectoryInfo videoDir = System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "/video/" + localName.Replace(".avi","/"));

            recorder = new ScreenCapture.Recorder(videoDir.FullName, 15, new ScreenCapture.Encoders.MpegEncoder(25));
            recorder.Start();
        }
        
        void overlay_NextTask(object sender, TaskCompletedEventArgs e)
        {
            TimeSpan started = markers.Count == 0 ? TimeSpan.Zero : markers.Last();
            TimeSpan taken = markers.Count == 0 ? recorder.RecordDuration : recorder.RecordDuration - started;

            taskResults.Add(new TaskResult()
            {
                TaskId = e.Task.TaskId,
                URL = browser.getUrl(),
                Abandoned = e.Abandoned,
                TimeStarted = started,
                TimeTaken = taken,
                NavigationPath = browser.NavigationPath,
            });

            browser.NavigationPath = "";
            markers.Add(recorder.RecordDuration);
            Console.WriteLine("Task Ended at {0} seconds", recorder.RecordDuration.Seconds);
        }

        void overlay_TestEnded(object sender, TestEventArgs e)
        {
            recorder.Stop();
            if (browser != null && browser.IsClosing == false)
            {
                browser.Close();
                browser.webControl.Dispose();
                browser.webControl = null;
                browser = null;
            }
            
            //Upload to FTP - in a new thread so the UI doesn't lock up
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;

            status.Content = new UXProject.BackgroundWorkerGUI(worker)
            {
                WorkingMessage = "Saving your session, please wait...",
                SuccessMessage = "Saved. You may now exit the application",
                FailMessage = "There was an error saving your session",
            };

            worker.RunWorkerAsync();
            
            TestResult res = new TestResult()
            {
                RecordingFilepath = "",
                //TODO: update this in the FTP area, after upload is successful - maybe even create a new entity called File which has a 1-many relationship with test result... recorder.Filepath.Replace(".avi", ".zip"),
                TestId = this.test.TestId,
                ParticipantId = this.participant.ParticipantId,
                TaskResults = taskResults,
            };

            DataAccessObject.SaveTestResult(res);
        }
        
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //wait for recording thread to finish (ie. close the file stream)
            recorder.Join();

            while (true)
            {
                if (!FTPQueue.Running)
                {
                    e.Result = true;
                    break;
                }
            }

        }
        
        #endregion
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConnectionChecker c = new ConnectionChecker();
            c.Owner = this;
            c.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (recorder != null) recorder.Stop(false);
            if (browser != null && browser.IsClosing == false)
            {
                browser.Close();
                browser.webControl.Dispose();
                browser.webControl = null;
                browser = null;
            }
        }

        private MessageBoxResult PromptExitMessage()
        {
            return MessageBox.Show("Closing this window will end the test, save test progress? 'Yes' to save test progress, 'No' to exit without saving, 'Cancel' if you do not wish to exit",
                        "End test?", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (browser == null || browser.IsClosing) return;
            if (overlay != null && overlay.TestInProgress)
            {
                switch(PromptExitMessage())
                {
                    case MessageBoxResult.Yes:
                        e.Cancel = true;
                        overlay.EndTest();
                        break;
                    case MessageBoxResult.No:
                        this.IsClosing = true;
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }
        
        void browser_Closing(object sender, CancelEventArgs e)
        {
            if (this.IsClosing) return;
            if (overlay != null && overlay.TestInProgress)
            {
                switch (PromptExitMessage())
                {
                    case MessageBoxResult.Yes: 
                        browser.IsClosing = true;
                        overlay.EndTest();
                        break;
                    case MessageBoxResult.No:
                        browser.IsClosing = true;
                        this.Close();
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        public bool IsClosing = false;
    }

}
