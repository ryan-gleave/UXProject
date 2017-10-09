using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ViewTest.xaml
    /// </summary>
    public partial class ViewTest : UserControl, INotifyPropertyChanged
    {
        private UXProject.Data.Test test = null;

        private ObservableCollection<TaskResultViewModel> tasks = null;
        public ObservableCollection<TaskResultViewModel> Tasks { get { return tasks; } set { tasks = value; OnPropertyChanged("Tasks"); } }

        public ViewTest(UXProject.Data.Test test)
        {
            InitializeComponent();
            list.DataContext = this;

            this.test = test;

            try
            {
                //Get tasks and task results
                List<TaskResultViewModel> tresults = new List<TaskResultViewModel>();
                foreach (UXProject.Data.Task task in UXProject.Data.DataAccessObject.GetTaskResults(test))
                {
                    tresults.Add(new TaskResultViewModel(task));
                }

                Tasks = new ObservableCollection<TaskResultViewModel>(tresults);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error loading that test");
                Console.WriteLine(ex.Message);
            }
        }

        public class TaskResultViewModel
        {
            public string Description { get; set; }
            public string CompletionRate { get; set; }
            public string AbandonRate { get; set; }
            public string AverageTimeOnTask { get; set; }
            public string AveragePageVisits { get; set; }

            public TaskResultViewModel(UXProject.Data.Task task)
            {
                Description = task.Description;
                int successRate = 0;
                List<int> pageVisits =new List<int>();
                foreach (UXProject.Data.TaskResult res in task.TaskResults)
                {
                    //Apply success criteria and save number of successes
                    if (res.Abandoned == false)
                    {
                        string resUrl = res.URL.ToLower();
                        string corUrl = task.CorrectURL.ToLower();
                        switch (task.ComparisonType)
                        {
                            case "CONTAINS":
                                if (resUrl.Contains(corUrl)) successRate++;
                                break;
                            case "EQUALS":
                                if (resUrl.Equals(corUrl)) successRate++;
                                break;
                            case "REGEX":
                                Regex regex = new Regex(corUrl);
                                if (regex.IsMatch(resUrl)) successRate++;
                                break;
                        }
                    }

                    //Path analysis
                    string[] path = res.NavigationPath.Split(',');
                    pageVisits.Add(path.GetLength(0));
                }

                CompletionRate = "Completion Rate: " + ((float)successRate / (float)task.TaskResults.Count).ToString("0.00%");
                AbandonRate = "Abandon Rate: " + ((float)task.TaskResults.Sum(i=>i.Abandoned?1:0) / (float)task.TaskResults.Count).ToString("0.00%");
                AverageTimeOnTask = "Avg. Time Taken: " + Math.Round(task.TaskResults.Average(i => i.TimeTaken.TotalSeconds)).ToString() + " seconds";
                AveragePageVisits = "Avg. Page Visits: " + pageVisits.Average();
                
            }
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

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UXProject.Data.Task selectedTask = (sender as ListView).SelectedItem as UXProject.Data.Task;
            if (selectedTask != null)
            {
                (this.DataContext as MainWindow).SelectedViewModel = new ViewTaskResults(selectedTask);
            }
        }
    }
}
