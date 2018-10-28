namespace SudokuSolver
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// http://stackoverflow.com/questions/16812020/how-to-bind-an-2d-array-bool-to-a-wpf-datagrid-one-way
    /// </summary>
    public class MatrixToDataViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var array = value as SudokuCell[,];
            if (array == null)
            {
                return null;
            }

            var rows = array.GetLength(0);
            var columns = array.GetLength(1);

            var t = new DataTable();
            for (var c = 0; c < columns; c++)
            {
                t.Columns.Add(new DataColumn(c.ToString()));
            }

            for (var r = 0; r < rows; r++)
            {
                var newRow = t.NewRow();
                for (var c = 0; c < columns; c++)
                {
                    var v = array[r, c];
                    newRow[c] = v;
                }

                t.Rows.Add(newRow);
            }

            return t.DefaultView;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
