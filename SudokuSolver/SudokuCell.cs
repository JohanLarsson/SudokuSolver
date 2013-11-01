using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Media;
using SudokuSolver.Annotations;

namespace SudokuSolver
{
    [Serializable]
    public class SudokuCell : INotifyPropertyChanged
    {
        private SudokuBoard _board;

        public SudokuCell(SudokuBoard board, int row, int col)
        {
            _board = board;
            RowIndex = row;
            ColumnIndex = col;
        }


        public int RowIndex
        {
            get { return _rowIndex; }
            private set { _rowIndex = value; }
        }

        public int ColumnIndex
        {
            get { return _columnIndex; }
            private set { _columnIndex = value; }
        }

        [NonSerialized]
        private SudokuCell[] _row;

        public SudokuCell[] Row
        {
            get
            {
                if (_row == null)
                {
                    _row = new SudokuCell[_board.Cells.GetLength(0)];
                    for (int col = _board.Cells.GetLowerBound(1); col < _board.Cells.GetLength(1); col++)
                    {
                        _row[col] = _board.Cells[RowIndex, col];
                    }
                }
                return _row;
            }
        }
        [NonSerialized]
        private SudokuCell[] _column;
        public SudokuCell[] Column
        {
            get
            {
                if (_column == null)
                {
                    _column = new SudokuCell[_board.Cells.GetLength(0)];
                    for (int row = _board.Cells.GetLowerBound(0); row < _board.Cells.GetLength(0); row++)
                    {
                        _column[row] = _board.Cells[row, ColumnIndex];
                    }
                }
                return _column;
            }
        }
        [NonSerialized]
        private SudokuCell[] _cluster;
        public SudokuCell[] Cluster
        {
            get
            {
                if (_cluster == null)
                {
                    _cluster = new SudokuCell[9];
                    var zeroRow = GetMacroZero(RowIndex);
                    var zeroCol = GetMacroZero(ColumnIndex);
                    int index = 0;
                    for (int i = zeroRow; i < zeroRow + 3; i++)
                    {
                        for (int j = zeroCol; j < zeroCol + 3; j++)
                        {
                            _cluster[index] = _board.Cells[i, j];
                            index++;
                        }
                    }
                }

                return _cluster;
            }
        }

        public List<int> PossibleValues
        {
            get
            {
                if (HasValue)
                    return new List<int>();
                var range = Enumerable.Range(1, 9);
                var rowValues = Row.Where(x => x.HasValue).Select(x => x.Value);
                var colValues = Column.Where(x => x.HasValue).Select(x => x.Value);
                var clusterValues = Cluster.Where(x => x.HasValue).Select(x => x.Value);


                return
                    range
                        .Except(rowValues.Concat(colValues).Concat(clusterValues))
                        .ToList();
            }
        }

        private int GetMacroZero(int i)
        {
            int r = 3;
            if (i < 3)
                r = 0;
            if (i > 5)
                r = 6;
            return r;
        }
        public bool HasValue { get { return Number.HasValue || CalculatedValue.HasValue || GuessValue.HasValue; } }

        public int Value
        {
            get
            {
                if (Number.HasValue)
                    return Number.Value;
                if (CalculatedValue.HasValue)
                    return CalculatedValue.Value;
                if (GuessValue.HasValue)
                    return GuessValue.Value;

                throw new InvalidOperationException();
            }
        }

        private int? _number;
        private int _rowIndex;
        private int _columnIndex;
        [NonSerialized]
        private int? _calculatedValue;
        [NonSerialized]
        private int? _guessValue;

        public int? CalculatedValue
        {
            get { return _calculatedValue; }
            set
            {
                if (value == _calculatedValue) return;
                _calculatedValue = value;
                foreach (var cell in Row)
                {
                    cell.OnPropertyChanged("HasError");
                }
                foreach (var cell in Column)
                {
                    cell.OnPropertyChanged("HasError");
                }
                foreach (var cell in Cluster)
                {
                    cell.OnPropertyChanged("HasError");
                }
                OnPropertyChanged();
            }
        }

        public SolidColorBrush HasError
        {
            get
            {
                if (HasValue)
                    return Brushes.Black;
                return PossibleValues.Any()
                    ? Brushes.Black
                    : Brushes.Red;
            }
        }

        public SolidColorBrush Background
        {
            get
            {
                int rc = GetMacroZero(RowIndex);
                int cc = GetMacroZero(ColumnIndex);
                bool br = rc % 2 == 0;
                bool bc = cc % 2 == 0;
                bool b = br ^ bc;
                return b
                    ? Brushes.LightGray
                    : Brushes.White;
            }
        }

        public int? GuessValue
        {
            get { return _guessValue; }
            set
            {
                if (value == _guessValue) return;
                _guessValue = value;
                OnPropertyChanged();
            }
        }

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

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}