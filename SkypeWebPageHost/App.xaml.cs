using System;
using System.Windows;

namespace SkypeWebPageHost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length == 2)
            {
                MainWindow wnd = new MainWindow();

                var skypeApp = new SkypeApp(wnd.Browser);
                skypeApp.Login(e.Args[0], e.Args[1]);


                wnd.Show();
                base.OnStartup(e);
                return;
            }

            Environment.Exit(0);

        }
    }
}
