using System;
using System.Globalization;
using System.Windows.Data;

namespace RobotWeld2.AppModel
{
    public class IntersectStyleConverter : IValueConverter
    {
        public object Convert(Object value, Type targerType, Object parameter, CultureInfo culture)
        {
            int ivar = (int)value;
            if (int.TryParse(parameter.ToString(), out int nums) && nums == ivar)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
