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
    /// Interaction logic for ConnectionChecker.xaml
    /// </summary>
    public partial class ConnectionChecker : Window
    {
        public ConnectionChecker()
        {
            InitializeComponent();

            #region Database BackgroundWorker
            BackgroundWorker dbConnectionTest = new BackgroundWorker();
            dbConnectionTest.DoWork += dbConnectionTest_DoWork;

            content.Children.Add(new UXProject.BackgroundWorkerGUI(dbConnectionTest)
            {
                WorkingMessage = "Testing database connection...",
                SuccessMessage = "Connection to database successful",
                FailMessage = "Connection to database failed",
            });

            dbConnectionTest.RunWorkerAsync();

            #endregion

            #region FTP BackgroundWorker
            BackgroundWorker ftpConnectionTest = new BackgroundWorker();
            ftpConnectionTest.DoWork += ftpConnectionTest_DoWork;

            content.Children.Add(new UXProject.BackgroundWorkerGUI(ftpConnectionTest)
            {
                WorkingMessage = "Testing FTP connection...",
                SuccessMessage = "Connection to FTP successful",
                FailMessage = "Connection to FTP failed",
            });

            ftpConnectionTest.RunWorkerAsync();

            #endregion

        }

        void dbConnectionTest_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = DataAccessObject.TestConnection();
        }

        void ftpConnectionTest_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = FTPClient.TestConnection();
        }

    }
}
