using System.Windows;
using log4net;

namespace RoutePlaner_Rafael_elias
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Configure log4net using the App.config file
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}