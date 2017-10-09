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

namespace UXProject
{
    /// <summary>
    /// Interaction logic for BackgroundWorkerGUI.xaml
    /// </summary>
    public partial class BackgroundWorkerGUI : UserControl, INotifyPropertyChanged
    {
        private string workingMessage = "Working...";
        public string WorkingMessage { get { return workingMessage; } set { workingMessage = value; text.Text = value; } }
        private string successMessage = "Work successful";
        public string SuccessMessage { get { return successMessage; } set { successMessage = value; } }
        private string failMessage = "Work failed";
        public string FailMessage { get { return failMessage; } set { failMessage = value; } }

        public BackgroundWorkerGUI(BackgroundWorker worker)
        {
            InitializeComponent();

            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }
        
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool res = false;
            if (e.Result != null && e.Result is bool) res = (bool)e.Result;
            text.Text = res ? SuccessMessage : FailMessage;
            icon.Foreground = res ? Brushes.Green : Brushes.Red;
            icon.Icon = res ? FontAwesome.WPF.FontAwesomeIcon.CheckCircle : FontAwesome.WPF.FontAwesomeIcon.TimesCircle;
            icon.Spin = false;
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
