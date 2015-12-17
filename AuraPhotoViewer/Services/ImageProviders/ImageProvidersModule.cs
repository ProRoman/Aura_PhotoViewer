using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace AuraPhotoViewer.Services.ImageProviders
{
    public class ImageProvidersModule : IModule
    {
        private IUnityContainer _container;

        [InjectionMethod]
        public void Initialize(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterType<IImageProvider, LocalImageProvider>("LocalImageProvider", new ContainerControlledLifetimeManager());
        }
    }
}