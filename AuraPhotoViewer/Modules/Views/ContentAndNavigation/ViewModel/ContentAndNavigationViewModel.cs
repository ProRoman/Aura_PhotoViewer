using AuraPhotoViewer.Modules.Common.Events;
using AuraPhotoViewer.Modules.Common.ViewModel;
using AuraPhotoViewer.Services.ImageProviders;
using log4net;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        private string _thumbnailsHeader;
                
        private Thumbnail _selectedThumbnail;

        private Picture _selectedImage;

        private bool _isImageSaving;

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

        public string ThumbnailsHeader
        {
            get
            {
                return _thumbnailsHeader;
            }
            set
            {
                _thumbnailsHeader = value;
                OnPropertyChanged();
            }
        }

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
                        new Action(() => { SelectedImage = new Picture { ImageUri = _selectedThumbnail.ImageUri }; }));
                }
            }
        }

        public Picture SelectedImage
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

        public bool IsImageSaving
        {
            get
            {
                return _isImageSaving;
            }
            set
            {
                _isImageSaving = value;
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
                    _imageProvider.LoadImagesAsync(Path.GetDirectoryName(imagePath),
                        new Progress<string>(image => _thumbnailCollection.Add(
                            new Thumbnail {ImageUri = image, FileName = Path.GetFileName(image)})));
                ThumbnailsHeader = String.Format("Gallery in {0}, {1} images", Path.GetDirectoryName(imagePath), _thumbnailCollection.Count);
                Thumbnail selectedThumbnail =
                    _thumbnailCollection.First(thumbnail => thumbnail.ImageUri == imagePath);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
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

        private async void ImageLeftExecuted()
        {
            await SaveImage();
            ThumbnailCollection.View.MoveCurrentToPrevious();
        }

        private async void ImageRightExecuted()
        {
            await SaveImage();
            ThumbnailCollection.View.MoveCurrentToNext();
        }

        private async Task SaveImage()
        {
            if (SelectedImage.Angle != 0)
            {
                Picture img = new Picture {ImageUri = SelectedImage.ImageUri, Angle = SelectedImage.Angle};
                int imgPos = ThumbnailCollection.View.CurrentPosition;
                IsImageSaving = true;
                await _imageProvider.SaveImageAsync(img.ImageUri, img.Angle);
                IsImageSaving = false;
                _thumbnailCollection.RemoveAt(imgPos);
                // delay to reclaim the deleted image memory
                await Task.Delay(2);
                _thumbnailCollection.Insert(imgPos, new Thumbnail { ImageUri = img.ImageUri });
                Thumbnail selectedThumbnail =
                    _thumbnailCollection.First(thumbnail => thumbnail.ImageUri == img.ImageUri);
                ThumbnailCollection.View.MoveCurrentTo(selectedThumbnail);
            }
        }

        #endregion

    }
}
