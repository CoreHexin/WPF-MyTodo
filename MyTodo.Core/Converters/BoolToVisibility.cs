using System.Globalization;
using System.Windows;

namespace MyTodo.Core.Converters
{
    public class BoolToVisibility : BaseConverter<BoolToVisibility>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Hidden;
            return Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
