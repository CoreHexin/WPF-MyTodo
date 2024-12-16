using System.Globalization;

namespace MyTodo.Core.Converters
{
    public class IntToStatusConverter : BaseConverter<IntToStatusConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
                return "[待办]";
            return "[已完成]";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
