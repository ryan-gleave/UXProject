using Awesomium.Core;
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
using System.Windows.Shapes;

namespace TestClient
{
    /// <summary>
    /// Interaction logic for AwesomiumBrowserWindow.xaml
    /// </summary>
    public partial class AwesomiumBrowserWindow : Window, INotifyPropertyChanged
    {
        private string navigationPath = "";
        public string NavigationPath { get { return navigationPath; } set { navigationPath = value; } }
        
        public void Pause()
        {
            this.IsEnabled = false;
            pauseOverlay.Visibility = System.Windows.Visibility.Visible;
        }

        public void Resume()
        {
            this.IsEnabled = true;
            pauseOverlay.Visibility = System.Windows.Visibility.Hidden;
        }

        //1. Textbox should only update the source of the webBrowser when enter is clicked (low priority) - should it even be enabled at all?
        //2. Webpage render speed is quite slow
        //3. Loading icon? To show when a page is being loaded or when it is frozen
        //4. Show the target URL (when highlighting buttons)
        //5. You can execute javascript and supply custom css, can you layer a javascript file over the top that listens to all webpage events being fired?
        public AwesomiumBrowserWindow(string url)
        {
            WebSession session = WebCore.CreateWebSession(new WebPreferences()
            {
                SmoothScrolling = true,
                EnableGPUAcceleration = false,
                //CustomCSS = "body { background-color : rgb(153, 255, 204); }"
            });

            InitializeComponent();

            string rUrl = resolveUrl(url);
            if (rUrl == null) throw new Exception("Invalid URL, please check with the test administrator");

            webControl.WebSession = session;
            webControl.Source = new Uri(rUrl);//Binding from property to Source parameter on webControl?
            webControl.AddressChanged += webControl_AddressChanged;
            webControl.DocumentReady += webControl_DocumentReady;
        }

        private void webControl_DocumentReady(object sender, DocumentReadyEventArgs e)
        {
            // When ReadyState is Ready, you can execute JavaScript against
            // the DOM but all resources are not yet loaded. Wait for Loaded.
            if (e.ReadyState == DocumentReadyState.Ready)
                return;

            if (!webControl.IsLive) return;

            JSValue result = webControl.CreateGlobalJavascriptObject("app");
            if (result.IsObject)
            {
                JSObject appObject = result;
                appObject.BindAsync("recordClick", recordClick);
            }

            string s = "var elements = document.getElementsByTagName('a');for (var i = 0, len = elements.length; i < len; i++) {elements[i].onclick = function () {app.recordClick();}}";
            webControl.ExecuteJavascript(s);
        }

        private void recordClick(object obj, JavascriptMethodEventArgs jsMethodArgs)
        {
            //record analytics ie. Number of <a> links clicked or so...
        }
        
        void webControl_AddressChanged(object sender, UrlEventArgs e)
        {
            Awesomium.Windows.Controls.WebControl window = sender as Awesomium.Windows.Controls.WebControl;
            btnBack.IsEnabled = window.CanGoBack();
            btnForward.IsEnabled = window.CanGoForward();
            navigationPath += (!navigationPath.Equals("") ? "," : "") + this.getUrl();
        }
        
        private string resolveUrl(string url)
        {
            string newUrl = url;
            if (!url.StartsWith("http://")) newUrl = "http://" + url;
            if (!Uri.IsWellFormedUriString(newUrl, UriKind.Absolute)) return null;
            return newUrl;
        }

        public string getUrl()
        {
            return webControl.Source.AbsoluteUri;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if(webControl.CanGoBack())
                webControl.GoBack();
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            if (webControl.CanGoForward())
                webControl.GoForward();
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

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                webControl.Focus();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            webControl.Reload(false);//true?
        }
                
        public bool IsClosing = false;
    }
}
