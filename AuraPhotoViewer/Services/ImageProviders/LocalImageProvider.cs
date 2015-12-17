using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AuraPhotoViewer.Services.ImageProviders
{
    public class LocalImageProvider : IImageProvider
    {
        private readonly List<string> _supportedImageExtensions = new List<string>
        {
            ".jpg",
            ".png",
            ".bmp",
            ".tiff",
            ".gif",
            ".ico"
        };

        public async Task LoadImagesAsync(string path, IProgress<string> progress)
        {
            await Task.Factory.StartNew(() =>
            {
                string sourceDirectory = Path.GetDirectoryName(path);
                if (sourceDirectory != null)
                {
                    var images = Directory.EnumerateFiles(sourceDirectory, "*.*")
                        .Where(image => _supportedImageExtensions.Any(ext =>
                        {
                            string extension = Path.GetExtension(image);
                            return extension != null && ext == extension.ToLower();
                        }));
                    foreach (string image in images)
                    {
                        progress.Report(image);
                    }
                }
            });
        }
    }
}