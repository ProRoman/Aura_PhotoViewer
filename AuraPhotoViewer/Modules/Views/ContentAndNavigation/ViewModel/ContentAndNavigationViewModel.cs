using System;
using System.Collections.ObjectModel;
using System.IO;
using AuraPhotoViewer.Modules.Common.Events;
using AuraPhotoViewer.Modules.Common.ViewModel;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;

namespace AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel
{
    public class ContentAndNavigationViewModel : ViewModelBase, IContentAndNavigationViewModel
    {
        #region Private fields
        
        private IEventAggregator _eventAggregator;

        #endregion

        #region Initialization

        [InjectionMethod]
        public void Initialize(IEventAggregator eventAggregator)
        {
            ThumbnailCollection = new ObservableCollection<Thumbnail>();
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenedImageEvent>().Subscribe(LoadImages, ThreadOption.UIThread);
        }

        #endregion

        #region Presentation properties

        public ObservableCollection<Thumbnail> ThumbnailCollection { get; set; }

        #endregion

        #region Private methods

        private void LoadImages(string sourceDirectory)
        {
            // TODO extract sourceDirectory from image path
            try
            {
                var images = Directory.EnumerateFiles(sourceDirectory, "*.jpg");
                foreach (string image in images)
                {
                    ThumbnailCollection.Add(new Thumbnail { ImageUri = image });
                }

            }
            catch (Exception e)
            {
                // TODO add log
            }
        }

        #endregion

    }
}
