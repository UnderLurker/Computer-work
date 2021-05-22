using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace bigwork.Converters
{
    public class OnLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush br = new SolidColorBrush(Color.FromRgb(255, 165, 0));
            if (value != null)
            {
                if (bool.TryParse(value.ToString(), out bool result))
                {
                    if (result)
                    {
                        return new SolidColorBrush(Color.FromRgb(65, 105, 225));
                    }
                    return br;
                }
            }
            return br;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
