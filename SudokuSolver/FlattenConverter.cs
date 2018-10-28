namespace SudokuSolver
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class FlattenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var matrix = (object[,])value;
            var flat = new object[matrix.Length];
            var i = 0;
            for (var r = 0; r < matrix.GetLength(0); r++)
            {
                for (var c = 0; c < matrix.GetLength(1); c++)
                {
                    flat[i] = matrix[r, c];
                    i++;
                }
            }

            return flat;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
