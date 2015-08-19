using AuraPhotoViewer.Modules.Common.Constants;
using AuraPhotoViewer.Modules.Views.ContentAndNavigation.View;
using AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace AuraPhotoViewer.Modules.Views.ContentAndNavigation
{
    public class ContentAndNavigationModule : IModule
    {
        private IUnityContainer _container;
        private IRegionManager _regionManager;

        [InjectionMethod]
        public void Initialize(IUnityContainer container,
                               IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterType<IContentAndNavigationViewModel, ContentAndNavigationViewModel>(
                new ContainerControlledLifetimeManager());
            IRegion region = _regionManager.Regions[RegionNames.ContentRegion];

            var windowInteractionView = _container.Resolve<ContentAndNavigationView>();
            region.Add(windowInteractionView, "ContentAndNavigationView");
            region.Activate(windowInteractionView);
        }
    }
}
