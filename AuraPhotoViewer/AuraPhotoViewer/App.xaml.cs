using System.Windows;

namespace AuraPhotoViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Start Bootstrapper
            var bootstrapper = new AuraBootstrapper();
            bootstrapper.Run();
        }
    }
}
