using AuraPhotoViewer.Modules.Common.Events;
using AuraPhotoViewer.Modules.Common.ViewModel;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel
{
    public class ContentAndNavigationViewModel : ViewModelBase
    {
        #region Private fields

        private ObservableCollection<Thumbnail> _thumbnailCollection;

        private IEventAggregator _eventAggregator;

        private Thumbnail _selectedThumbnail;

        private string _selectedImage;

        #endregion

        #region Initialization

        [InjectionMethod]
        public void Initialize(IEventAggregator eventAggregator)
        {            
            _eventAggregator = eventAggregator;
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

        private void LoadImages(string imagePath)
        {            
            try
            {
                string sourceDirectory = Path.GetDirectoryName(imagePath);
                List<string> extensions = new List<string> { ".jpg", ".png", ".bmp", ".tiff", ".gif", ".ico" };
                if (sourceDirectory != null)
                {
                    var images = Directory.EnumerateFiles(sourceDirectory, "*.*")
                        .Where(image => extensions.Any(ext =>
                        {
                            string extension = Path.GetExtension(image);                                         
                            return extension != null && ext == extension.ToLower();
                        }));
                    foreach (string image in images)
                    {
                        _thumbnailCollection.Add(new Thumbnail { ImageUri = image });
                    }
                }
                Thumbnail selectedThumbnail = _thumbnailCollection.First<Thumbnail>(thumbnail => thumbnail.ImageUri == imagePath);
                ThumbnailCollection.View.MoveCurrentTo(selectedThumbnail);
            }
            catch (Exception e)
            {
                // TODO add log
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
