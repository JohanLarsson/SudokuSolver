using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Documents;
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

        public List<int> PossibleValues
        {
            get
            {
                if (HasValue)
                    return new List<int>();
                var range = Enumerable.Range(1, 9);
                var rowValues = Row.Where(x => x.HasValue).Select(x => x.Value);
                var colValues = Column.Where(x => x.HasValue).Select(x => x.Value);
                return
                    range
                        .Except(rowValues.Concat(colValues))
                        .ToList();
            }
        }
        public bool HasValue { get { return Number.HasValue || GuessedValue.HasValue || TempGuessValue.HasValue; } }

        public int Value
        {
            get
            {
                if (Number.HasValue)
                    return Number.Value;
                if (GuessedValue.HasValue)
                    return GuessedValue.Value;
                if (TempGuessValue.HasValue)
                    return TempGuessValue.Value;

                throw new InvalidOperationException();
            }
        }

        private int? _number;
        private int _rowIndex;
        private int _columnIndex;
        [NonSerialized]
        private int? _guessedValue;
        [NonSerialized]
        private int? _tempGuessValue;

        public int? GuessedValue
        {
            get { return _guessedValue; }
            set
            {
                if (value == _guessedValue) return;
                _guessedValue = value;
                OnPropertyChanged();
            }
        }

        public int? TempGuessValue
        {
            get { return _tempGuessValue; }
            set
            {
                if (value == _tempGuessValue) return;
                _tempGuessValue = value;
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