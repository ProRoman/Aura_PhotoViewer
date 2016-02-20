using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AuraPhotoViewer.Styles.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;
            if ((bool) parameter)
            {
                return boolValue ? Visibility.Visible : Visibility.Hidden;
            }
            return boolValue ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}