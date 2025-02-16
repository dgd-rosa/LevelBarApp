using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LevelBarApp.Converters
{
    internal class MaxLevelToYConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length < 4 || !(values[0] is float maxLevel) || !(values[1] is double progressBarHeight) ||
                !(values[2] is double minimumValue) || !(values[3] is double maximumValue))
            {
                return 0;
            }

            double normalizedValue = (maxLevel - minimumValue) / (maximumValue - minimumValue);
            return progressBarHeight * (1 - normalizedValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
