using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using UXProject.Data;

namespace TestClient
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : UserControl, INotifyPropertyChanged
    {
        public delegate void TestStartedHandler(object sender, TestEventArgs e);
        public event TestStartedHandler TestStarted;

        public delegate void TestPausedHandler(object sender, TestEventArgs e);
        public event TestPausedHandler TestPaused;

        public delegate void TestResumedHandler(object sender, TestEventArgs e);
        public event TestResumedHandler TestResumed;

        public delegate void TestEndedHandler(object sender, TestEventArgs e);
        public event TestEndedHandler TestEnded;

        public delegate void NextTaskHandler(object sender, TaskCompletedEventArgs e);
        public event NextTaskHandler NextTask;

        TestState currentTest;

        private bool testInProgress = false;
        public bool TestInProgress { get { return testInProgress; } }

        public Overlay(Test test)
        {
            InitializeComponent();
            currentTest = new TestState(test);
            testSummary.DataContext = currentTest;
            testInfo.DataContext = currentTest;
            this.DataContext = this;
        }
        
        private void btnBeginTest_Click(object sender, RoutedEventArgs e)
        {
            testSummary.Visibility = System.Windows.Visibility.Collapsed;
            testInfo.Visibility = System.Windows.Visibility.Visible;
            testInProgress = true;
            TestStarted(this, new TestEventArgs());
        }

        private void btnNextTask_Click(object sender, RoutedEventArgs e)
        {
            TaskCompletedEventArgs args = new TaskCompletedEventArgs();
            args.Task = currentTest.getCurrentTask();
            NextTask(this, args);

            currentTest.CompleteTask();//todo: save the task variables

            if (currentTest.Complete)
                EndTest();
        }

        //Invoked from exiting, end test button, or next task button
        public void EndTest(bool exitOnEnd = false)
        {
            testInfo.Visibility = System.Windows.Visibility.Collapsed;
            finishPage.Visibility = System.Windows.Visibility.Visible;
            testInProgress = false;
            TestEnded(this, new TestEventArgs(exitOnEnd));
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void btnAbandonTask_Click(object sender, RoutedEventArgs e)
        {
            TaskCompletedEventArgs args = new TaskCompletedEventArgs();
            args.Task = currentTest.getCurrentTask();
            args.Abandoned = true;
            NextTask(this, args);

            currentTest.CompleteTask();//todo: save the task variables

            if (currentTest.Complete)
                EndTest();
        }

        private bool paused = false;

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (paused)
            {
                btnAbandonTask.IsEnabled = true;
                btnNextTask.IsEnabled = true;
                TestResumed(this, new TestEventArgs());
            }
            else
            {
                btnAbandonTask.IsEnabled = false;
                btnNextTask.IsEnabled = false;
                TestPaused(this, new TestEventArgs());
            }

            paused = !paused;
            //btnPause.Content = paused ? "Resume" : "Pause";
            (btnPause.Content as PackIconMaterial).Kind = paused ? PackIconMaterialKind.PlayCircleOutline : PackIconMaterialKind.PauseCircleOutline;
        }
    }

    public class TestState : INotifyPropertyChanged
    {
        //Functionality
        private Test test;
        private List<UXProject.Data.Task> tasks;
        private int currentTask = 0;
        private bool complete = false;

        //Bindable Properties
        public string CurrentTask { get { return "Task " + (currentTask + 1) + "/" + TotalTasks; } }
        public string CurrentTaskDescription { get { return currentTask < tasks.Count ? this.tasks[currentTask].Description : ""; } }
        public int TotalTasks { get { return tasks.Count; } }
        public bool Complete { get { return complete; } }
        public string URL { get { return test.URL; } }
        
        public TestState(Test test)
        {
            this.test = test;
            this.tasks = DataAccessObject.GetTasks(test).OrderBy(i => i.TaskId).ToList();
        }

        public void CompleteTask()
        {
            currentTask += 1;
            OnPropertyChanged("CurrentTask");
            OnPropertyChanged("CurrentTaskDescription");
            if (currentTask >= tasks.Count) complete = true;
        }
        
        public UXProject.Data.Task getCurrentTask()
        {
            return tasks[currentTask];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
