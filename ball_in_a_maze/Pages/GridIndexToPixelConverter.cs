using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ball_in_a_maze
{
    /// <summary>
    /// this converter takes the index of an array and converts it to an absolute coordinate in a page
    /// </summary>
    public class GridIndexToPixelConverter : IValueConverter
    {
        public GridIndexToPixelConverter(double PageMax, int FieldMax)
        {
            // jsut save the parameters for the calculation
            pageMax = PageMax;
            fieldMax = FieldMax;
        }

        private double pageMax { get; set; }
        private double fieldMax { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value is the index in the array 
            // first calculate the balls position in the field as a percentage
            // then multiply with the pixels of the page to get an absolute position
            return (double)value /fieldMax * pageMax;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
