using AuraPhotoViewer.Modules.Common.Events;
using AuraPhotoViewer.Modules.Common.ViewModel;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;

namespace AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel
{
    public class ContentAndNavigationViewModel : ViewModelBase, IContentAndNavigationViewModel
    {
        #region Private fields

        private string _imageUri;
        private IEventAggregator _eventAggregator;

        #endregion

        #region Initialization

        [InjectionMethod]
        public void Initialize(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenedImageEvent>().Subscribe(UpdateImage, ThreadOption.UIThread);
        }

        #endregion

        #region Presentation properties

        public string ImageUri
        {
            get { return _imageUri; }
            set
            {
                _imageUri = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Private methods

        private void UpdateImage(string imageUri)
        {
            ImageUri = imageUri;
        }

        #endregion

    }
}
