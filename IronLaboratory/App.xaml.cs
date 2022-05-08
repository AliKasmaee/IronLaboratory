using System.Globalization;
using System.Threading;
using System.Windows;

namespace IronLaboratory
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow mainWindow = new MainWindow();

            SplashWindow splash = new SplashWindow();
            splash.Show();

            Thread.Sleep(1500);

            var culture = new CultureInfo("en-US");

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            mainWindow.Show();
            splash.Close();
        }
    }
}
