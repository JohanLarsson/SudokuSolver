namespace SudokuSolver
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class ToColumnsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var matrix = (object[,])value;
            return matrix.GetLength(1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}