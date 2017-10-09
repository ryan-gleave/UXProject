using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


namespace ManagementClient
{
    /// <summary>
    /// Interaction logic for TestSummary.xaml
    /// </summary>
    public partial class TestSummary : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<UXProject.Data.Test> tests = null;
        public ObservableCollection<UXProject.Data.Test> Tests { get { return tests; } set { tests = value; OnPropertyChanged("Tests"); } }

        public TestSummary()
        {
            InitializeComponent();
            list.DataContext = this;
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
            //UXProject.BackgroundWorkerGUI workerGui = new UXProject.BackgroundWorkerGUI(worker);//display worker UI
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Tests = new ObservableCollection<UXProject.Data.Test>(UXProject.Data.DataAccessObject.GetAllTests());
            e.Result = true;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null)
            {
                (this.DataContext as MainWindow).SelectedViewModel = new ViewTest((sender as ListView).SelectedItem as UXProject.Data.Test);
            }
        }
        
        private void btnCreateTest_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as MainWindow).SelectedViewModel = new CreateTest();
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
