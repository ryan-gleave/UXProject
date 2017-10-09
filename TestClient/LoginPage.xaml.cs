using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core;
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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void btn_Enter_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker loginWorker = new BackgroundWorker();
            loginWorker.DoWork += loginWorker_DoWork;
            loginWorker.RunWorkerCompleted += loginWorker_RunWorkerCompleted;
            loginWorker.RunWorkerAsync(txt_userNumber.Text);

            btn_Enter.IsEnabled = false;
            txt_error.Text = "";
        }

        void loginWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string errorText = "";

            if (e.Result is Participant)
            {
                LoggedIn(this, new LoggedInEventArgs(e.Result as Participant));
            }
            else
            {
                errorText = e.Result as string;
            }

            txt_error.Text = errorText;
            btn_Enter.IsEnabled = true;
        }

        void loginWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = DataAccessObject.GetParticipant(e.Argument as string);
                if (e.Result == null)
                    e.Result = "Incorrect participant number";
                else if ((e.Result as Participant).Test == null) //should never be true
                    e.Result = "Your participant number is not associated with any test";
            }
            catch (EntityException ex)
            {
                e.Result = "Connection to database could not be established";
                Console.WriteLine(ex.Message);
            }
        }

        private void txt_userNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Reset the error message
            if (txt_error != null && txt_error.Text.StartsWith("Incorrect"))
                txt_error.Text = "";
        }

        public delegate void LoggedInHandler(object sender, LoggedInEventArgs e);
        public event LoggedInHandler LoggedIn;

        private void txt_userNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                btn_Enter_Click(null, null);
        }
    }

    public class LoggedInEventArgs : EventArgs
    {
        private Participant participant;
        public Participant Participant { get { return participant; } }

        public LoggedInEventArgs(Participant user)
        {
            this.participant = user;
        }
    }

     
}
