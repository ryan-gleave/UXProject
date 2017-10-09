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

namespace ManagementClient
{
    /// <summary>
    /// Interaction logic for ViewTaskResults.xaml
    /// </summary>
    public partial class ViewTaskResults : UserControl
    {
        private UXProject.Data.Task task = null;

        public ViewTaskResults(UXProject.Data.Task task)
        {
            InitializeComponent();

            this.task = task;

            BackgroundWorker loadResults = new BackgroundWorker();
            loadResults.DoWork += loadResults_DoWork;
            loadResults.RunWorkerAsync();
        }

        void loadResults_DoWork(object sender, DoWorkEventArgs e)
        {
            List<UXProject.Data.TaskResult> results = UXProject.Data.DataAccessObject.GetAllTaskResults(task);
        }
    }
}
