using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LevelBarApp.Converters
{
    public class MaxLevelToYConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts the value from the progress bar to the exact height. It is used to place the peak hold
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length < 4 || !(values[0] is float level) || !(values[1] is double progressBarHeight) ||
                !(values[2] is double minimumValue) || !(values[3] is double maximumValue))
            {
                return 0;
            }

            //Ratio between the level and maximum value
            double normalizedValue = (level - minimumValue) / (maximumValue - minimumValue);
            //Calculate height where the peakhold should be placed
            // it is 1-normalizedvalue because the bar is oriented vertically (rotation of 270 degrees)
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
