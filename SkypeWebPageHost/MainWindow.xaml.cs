using System.Windows;
using CefSharp;

namespace SkypeWebPageHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string user, string password)
        {
            InitializeComponent();

            Browser.IsBrowserInitializedChanged += (sender, args) =>
            {
                // if (!Browser.IsDisposed) Browser.ShowDevTools();
            };

            var skypeApp = new SkypeApp(this.Browser);
            skypeApp.Login(user, password);
        }
    }
}
