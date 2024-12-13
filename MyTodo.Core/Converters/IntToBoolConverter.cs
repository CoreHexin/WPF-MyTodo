using System.Globalization;

namespace MyTodo.Core.Converters
{
    public class IntToBoolConverter : BaseConverter<IntToBoolConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 1)
                return true;
            return false;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return 1;
            return 0;
        }
    }
}
