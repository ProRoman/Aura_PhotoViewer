using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        public async Task LoadImagesAsync(string sourceDirectory, IProgress<string> progress)
        {
            await Task.Factory.StartNew(() =>
            {
                if (sourceDirectory == null)
                {
                    return;
                }
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
            });
        }

        public async Task SaveImageAsync(string uri, double angle)
        {
            await Task.Factory.StartNew(() =>
            {
                Bitmap img = (Bitmap)Bitmap.FromFile(uri);
                ImageFormat imgfrmt = img.RawFormat;
                img.RotateFlip(GetImageRotate(angle));
                if (File.Exists(uri))
                {
                    File.Delete(uri);
                }
                img.Save(uri, imgfrmt);
            });
        }

        private RotateFlipType GetImageRotate(double angle)
        {
            switch ((int)angle)
            {
                case 90:
                case -270:
                    return RotateFlipType.Rotate90FlipNone;
                case 180:
                case -180:
                    return RotateFlipType.Rotate180FlipNone;
                case 270:
                case -90:
                    return RotateFlipType.Rotate270FlipNone;
                default:
                    throw new ArgumentException("The agle rotate is invalid");
            }
        }
    }
}