using System.Windows;
using log4net;

namespace CAC40Performance
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            base.OnStartup(e);
        }
    }
}
