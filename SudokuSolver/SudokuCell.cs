using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SudokuSolver.Annotations;

namespace SudokuSolver
{
    public class SudokuCell : INotifyPropertyChanged
    {
        private readonly SudokuBoard _board;

        public SudokuCell(SudokuBoard board, int row, int col)
        {
            _board = board;
            RowIndex = row;
            ColumnIndex = col;
        }

        public int RowIndex { get; private set; }
        public int ColumnIndex { get; private set; }
        private SudokuCell[] _row;
        public SudokuCell[] Row
        {
            get
            {
                if (_row == null)
                {
                    _row = new SudokuCell[_board.Numbers.GetLength(0)];
                    for (int col = _board.Numbers.GetLowerBound(1); col < _board.Numbers.GetLength(1); col++)
                    {
                        _row[col] = _board.Numbers[RowIndex, col];
                    }
                }
                return _row;
            }
        }

        private SudokuCell[] _column;
        public SudokuCell[] Column
        {
            get
            {
                if (_column == null)
                {
                    _column = new SudokuCell[_board.Numbers.GetLength(0)];
                    for (int row = _board.Numbers.GetLowerBound(0); row < _board.Numbers.GetLength(0); row++)
                    {
                        _column[row] = _board.Numbers[ row,ColumnIndex];
                    }
                }
                return _column;
            }
        }

        private int? _number;


        public int? Number
        {
            get { return _number; }
            set
            {
                if (value == _number) return;
                _number = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return string.Format("RowIndex: {0}, ColumnIndex: {1}, Number: {2}", RowIndex, ColumnIndex, Number);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}