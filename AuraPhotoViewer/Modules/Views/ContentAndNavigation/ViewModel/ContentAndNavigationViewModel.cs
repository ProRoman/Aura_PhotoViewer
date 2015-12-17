using AuraPhotoViewer.Modules.Common.Events;
using AuraPhotoViewer.Modules.Common.ViewModel;
using AuraPhotoViewer.Services.ImageProviders;
using log4net;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel
{
    public class ContentAndNavigationViewModel : ViewModelBase
    {
        #region Log4net

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Private fields

        private IEventAggregator _eventAggregator;

        private IImageProvider _imageProvider;

        private ObservableCollection<Thumbnail> _thumbnailCollection;
                
        private Thumbnail _selectedThumbnail;

        private string _selectedImage;

        #endregion

        #region Initialization

        [InjectionMethod]
        public void Initialize(IEventAggregator eventAggregator,
            [Dependency("LocalImageProvider")] IImageProvider imageProvider)
        {
            _eventAggregator = eventAggregator;
            _imageProvider = imageProvider;
            _eventAggregator.GetEvent<OpenedImageEvent>().Subscribe(LoadImages, ThreadOption.UIThread);
            ThumbnailCollection = new CollectionViewSource();
            _thumbnailCollection = new ObservableCollection<Thumbnail>();
            ThumbnailCollection.Source = _thumbnailCollection;
            ImageLeftCommand = new DelegateCommand(ImageLeftExecuted);
            ImageRightCommand = new DelegateCommand(ImageRightExecuted);
        }

        #endregion

        #region Presentation properties

        public CollectionViewSource ThumbnailCollection { get; set; }

        public Thumbnail SelectedThumbnail
        {
            get
            {
                return _selectedThumbnail;
            }
            set
            {
                _selectedThumbnail = value;
                if(_selectedThumbnail != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        new Action(() => { SelectedImage = _selectedThumbnail.ImageUri; }));
                }
            }
        }

        public string SelectedImage
        {
            get
            {
                return _selectedImage;
            }
            set
            {
                _selectedImage = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand ImageLeftCommand { get; set; }

        public DelegateCommand ImageRightCommand { get; set; }

        #endregion

        #region Private methods

        private async void LoadImages(string imagePath)
        {
            try
            {
                Log.Info("Images load");
                await
                    _imageProvider.LoadImagesAsync(imagePath,
                        new Progress<string>(image => _thumbnailCollection.Add(new Thumbnail {ImageUri = image})));
                Thumbnail selectedThumbnail =
                    _thumbnailCollection.First<Thumbnail>(thumbnail => thumbnail.ImageUri == imagePath);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                    new Action(() =>
                    {
                        ThumbnailCollection.View.MoveCurrentTo(selectedThumbnail);
                    }));
            }
            catch (Exception e)
            {
                Log.Error("Exception during images load", e);
            }
        }

        private void ImageLeftExecuted()
        {
            ThumbnailCollection.View.MoveCurrentToPrevious();            
        }

        private void ImageRightExecuted()
        {
            ThumbnailCollection.View.MoveCurrentToNext();
        }

        #endregion

    }
}
