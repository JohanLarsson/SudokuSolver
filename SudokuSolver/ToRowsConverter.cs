using System;
using System.Globalization;
using System.Windows.Data;

namespace SudokuSolver
{
    public class ToRowsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var matrix = (object[,])value;
            return matrix.GetLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}