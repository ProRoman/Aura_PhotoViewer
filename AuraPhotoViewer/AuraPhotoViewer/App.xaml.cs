using System.Windows;
using AuraPhotoViewer.Modules.Common.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;

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
            // On start up publish opened image
            if (e.Args.Length >= 1)
            {
                bootstrapper.Container.Resolve<IEventAggregator>().GetEvent<OpenedImageEvent>().Publish(e.Args[0]);
            }
        }
    }
}
