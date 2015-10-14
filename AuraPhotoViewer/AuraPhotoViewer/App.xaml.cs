using AuraPhotoViewer.Modules.Common.Events;
using Microsoft.Practices.Unity;
using Prism.Events;
using System.Windows;
using log4net;

namespace AuraPhotoViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Log4net

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Info("App starts");
            base.OnStartup(e);
            Log.Info("Bootstrapper starts");
            // Start Bootstrapper            
            var bootstrapper = new AuraBootstrapper();
            bootstrapper.Run();
            Application.Current.Exit += AppExitHandler;
            // On start up publish opened image
            if (e.Args.Length >= 1)
            {
                bootstrapper.Container.Resolve<IEventAggregator>().GetEvent<OpenedImageEvent>().Publish(e.Args[0]);
            }            
        }

        private void AppExitHandler(object sender, ExitEventArgs exitEventArgs)
        {
            Log.Info("App shuts down");
        }
    }
}
