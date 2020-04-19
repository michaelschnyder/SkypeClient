using System.Windows;
using CefSharp;

namespace SkypeWebPageHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Browser.IsBrowserInitializedChanged += (sender, args) =>
            {
                if (!Browser.IsDisposed) Browser.ShowDevTools();
            };
        }
    }
}
