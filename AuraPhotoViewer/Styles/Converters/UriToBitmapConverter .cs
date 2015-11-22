using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AuraPhotoViewer.Styles.Converters
{
    public class UriToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.DecodePixelHeight = 200;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            RenderOptions.SetBitmapScalingMode(bi, BitmapScalingMode.LowQuality);
            bi.UriSource = new Uri(value.ToString());
            bi.EndInit();
            return bi;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}