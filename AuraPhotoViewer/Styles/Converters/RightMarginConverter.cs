using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AuraPhotoViewer.Styles.Converters
{
    public class RightMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Thickness(0, 0, ((Thickness)value).Right, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
