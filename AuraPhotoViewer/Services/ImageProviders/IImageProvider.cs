using System;
using System.Threading.Tasks;

namespace AuraPhotoViewer.Services.ImageProviders
{
    public interface IImageProvider
    {
        Task LoadImagesAsync(string path, IProgress<string> progress);

        Task SaveImageAsync(string uri, double angle);
    }
}