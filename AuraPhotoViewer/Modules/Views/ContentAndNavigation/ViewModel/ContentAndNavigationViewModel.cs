using AuraPhotoViewer.Modules.Common.Events;
using AuraPhotoViewer.Modules.Common.ViewModel;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel
{
    public class ContentAndNavigationViewModel : ViewModelBase, IContentAndNavigationViewModel
    {
        #region Private fields

        private ImageSource _imageUri;
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

        public ImageSource ImageUri
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

        private async void UpdateImage(string imageUri)
        {
            var bmp = await System.Threading.Tasks.Task.Run(() =>
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.UriSource = new Uri(imageUri, UriKind.Absolute);
                    img.EndInit();
                    return img;
                }
            );
            Application.Current.Dispatcher.BeginInvoke(() => { ImageUri = new BitmapImage(bmp); }); 
        }

        #endregion

    }
}
