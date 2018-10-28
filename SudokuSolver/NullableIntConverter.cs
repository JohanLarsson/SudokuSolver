namespace SudokuSolver
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class NullableIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (string)value;
            if (int.TryParse(s, out var i))
            {
                return i;
            }

            return null;
        }
    }
}
