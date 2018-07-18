using System;
using System.Windows;
using System.Windows.Data;

namespace Manager.Converters
{
    [ValueConversion(typeof(Boolean), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            if ((bool)value == true)
            {
                return Visibility.Visible;
            }
            else
            {
                return (string)parameter == "Collapsed" ? Visibility.Collapsed : Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}