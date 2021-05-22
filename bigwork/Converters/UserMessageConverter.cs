using bigwork.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace bigwork.Converters
{
    public class UserMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.Equals(MainViewModel.Nname))
                {
                    return HorizontalAlignment.Right;
                }
                return HorizontalAlignment.Left;
            }
            return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
