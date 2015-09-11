using System.Collections.ObjectModel;

namespace AuraPhotoViewer.Modules.Views.ContentAndNavigation.ViewModel
{
    public interface IContentAndNavigationViewModel
    {
        ObservableCollection<Thumbnail> ThumbnailCollection { get; set; }
    }
}