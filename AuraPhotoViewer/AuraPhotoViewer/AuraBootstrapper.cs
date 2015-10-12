using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Modularity;
using Prism.Unity;
using System.Windows;

namespace AuraPhotoViewer
{
    /// Basic stages of the bootstrapping process:
    ///     1. Create a Logger Facade.
    ///     2. Create and Configure a Module Catalog.
    ///     3. Create and Configure the Unity Container. This will return a new instance of the UnityContainer.
    ///     4. Confiure Default Region Adapter Mappings.
    ///     5. Configure Default Region Behaviours.
    ///     6. Register Framework Exception Types.
    ///     7. Create the Shell.
    ///     8. Initialize Shell.
    ///     9. Initialize Modules.
    /// 
    class AuraBootstrapper : UnityBootstrapper
    {
        protected override ILoggerFacade CreateLogger()
        {
            return new Log4NetPrismFacade();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            // Specify that the configuration file is the source for ModuleCatalog.
            return new ConfigurationModuleCatalog();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }
    }
}
