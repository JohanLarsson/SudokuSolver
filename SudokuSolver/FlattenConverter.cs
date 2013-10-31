using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SudokuSolver
{
    public class FlattenConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var matrix= (object[,]) value;
            var flat = new object[matrix.Length];
            int i = 0;
            for (int r = 0; r < matrix.GetLength(0); r++)
                for (int c = 0; c < matrix.GetLength(1); c++)
                {
                    flat[i] = matrix[r, c];
                    i++;

                }
            return flat;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
