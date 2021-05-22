using bigwork.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace bigwork.Converters
{
    public class MessageMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == MainViewModel.Nname)
            {
                return new Thickness(0, 10, 15, 0);
            }
            return new Thickness(15, 10, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
