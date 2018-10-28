namespace SudokuSolver
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Media;

    [Serializable]
    public class SudokuCell : INotifyPropertyChanged
    {
        private readonly SudokuBoard board;

        private int? number;
        private int rowIndex;
        private int columnIndex;
        [NonSerialized]
        private int? calculatedValue;
        [NonSerialized]
        private int? guessValue;

        private int? calculatedAfterGuess;

        [NonSerialized]
        private SudokuCell[] row;

        [NonSerialized]
        private SudokuCell[] column;
        [NonSerialized]
        private SudokuCell[] cluster;

        public SudokuCell(SudokuBoard board, int row, int col)
        {
            this.board = board;
            this.RowIndex = row;
            this.ColumnIndex = col;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public int RowIndex
        {
            get => this.rowIndex;
            private set
            {
                if (value == this.rowIndex)
                {
                    return;
                }

                this.rowIndex = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.PossibleValues));
                this.OnPropertyChanged(nameof(this.HasError));
                this.OnPropertyChanged(nameof(this.Background));
            }
        }

        public int ColumnIndex
        {
            get => this.columnIndex;
            private set
            {
                if (value == this.columnIndex)
                {
                    return;
                }

                this.columnIndex = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.PossibleValues));
                this.OnPropertyChanged(nameof(this.HasError));
                this.OnPropertyChanged(nameof(this.Background));
            }
        }

        public SudokuCell[] Row
        {
            get
            {
                if (this.row == null)
                {
                    this.row = new SudokuCell[this.board.Cells.GetLength(0)];
                    for (var col = this.board.Cells.GetLowerBound(1); col < this.board.Cells.GetLength(1); col++)
                    {
                        this.row[col] = this.board.Cells[this.RowIndex, col];
                    }

                    this.OnPropertyChanged(nameof(this.PossibleValues));
                    this.OnPropertyChanged(nameof(this.HasError));
                }

                return this.row;
            }
        }

        public SudokuCell[] Column
        {
            get
            {
                if (this.column == null)
                {
                    this.column = new SudokuCell[this.board.Cells.GetLength(0)];
                    for (var row = this.board.Cells.GetLowerBound(0); row < this.board.Cells.GetLength(0); row++)
                    {
                        this.column[row] = this.board.Cells[row, this.ColumnIndex];
                    }

                    this.OnPropertyChanged(nameof(this.PossibleValues));
                    this.OnPropertyChanged(nameof(this.HasError));
                }

                return this.column;
            }
        }

        public SudokuCell[] Cluster
        {
            get
            {
                if (this.cluster == null)
                {
                    this.cluster = new SudokuCell[9];
                    var zeroRow = this.GetMacroZero(this.RowIndex);
                    var zeroCol = this.GetMacroZero(this.ColumnIndex);
                    var index = 0;
                    for (var i = zeroRow; i < zeroRow + 3; i++)
                    {
                        for (var j = zeroCol; j < zeroCol + 3; j++)
                        {
                            this.cluster[index] = this.board.Cells[i, j];
                            index++;
                        }
                    }

                    this.OnPropertyChanged(nameof(this.PossibleValues));
                    this.OnPropertyChanged(nameof(this.HasError));
                }

                return this.cluster;
            }
        }

        public List<int> PossibleValues
        {
            get
            {
                if (this.HasValue)
                {
                    return new List<int>();
                }

                var range = Enumerable.Range(1, 9);
                var rowValues = this.Row.Where(x => x.HasValue).Select(x => x.Value);
                var colValues = this.Column.Where(x => x.HasValue).Select(x => x.Value);
                var clusterValues = this.Cluster.Where(x => x.HasValue).Select(x => x.Value);

                return
                    range
                        .Except(rowValues.Concat(colValues).Concat(clusterValues))
                        .ToList();
            }
        }

        public bool HasValue => this.Number.HasValue || this.CalculatedValue.HasValue || this.GuessValue.HasValue || this.CalculatedAfterGuess.HasValue;

        public int Value
        {
            get
            {
                if (this.Number.HasValue)
                {
                    return this.Number.Value;
                }

                if (this.CalculatedValue.HasValue)
                {
                    return this.CalculatedValue.Value;
                }

                if (this.GuessValue.HasValue)
                {
                    return this.GuessValue.Value;
                }

                if (this.CalculatedAfterGuess.HasValue)
                {
                    return this.CalculatedAfterGuess.Value;
                }

                throw new InvalidOperationException();
            }
        }

        public int? CalculatedValue
        {
            get => this.calculatedValue;
            set
            {
                if (value == this.calculatedValue)
                {
                    return;
                }

                this.calculatedValue = value;
                foreach (var cell in this.Row)
                {
                    cell.OnPropertyChanged(nameof(this.HasError));
                }

                foreach (var cell in this.Column)
                {
                    cell.OnPropertyChanged(nameof(this.HasError));
                }

                foreach (var cell in this.Cluster)
                {
                    cell.OnPropertyChanged(nameof(this.HasError));
                }

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.PossibleValues));
                this.OnPropertyChanged(nameof(this.HasValue));
                this.OnPropertyChanged(nameof(this.Value));
                this.OnPropertyChanged(nameof(this.HasError));
            }
        }

        public int? GuessValue
        {
            get => this.guessValue;
            set
            {
                if (value == this.guessValue)
                {
                    return;
                }

                this.guessValue = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.PossibleValues));
                this.OnPropertyChanged(nameof(this.HasValue));
                this.OnPropertyChanged(nameof(this.Value));
                this.OnPropertyChanged(nameof(this.HasError));
            }
        }

        public int? Number
        {
            get => this.number;
            set
            {
                if (value == this.number)
                {
                    return;
                }

                this.number = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.PossibleValues));
                this.OnPropertyChanged(nameof(this.HasValue));
                this.OnPropertyChanged(nameof(this.Value));
                this.OnPropertyChanged(nameof(this.HasError));
            }
        }

        public int? CalculatedAfterGuess
        {
            get => this.calculatedAfterGuess;
            set
            {
                if (value == this.calculatedAfterGuess)
                {
                    return;
                }

                this.calculatedAfterGuess = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.PossibleValues));
                this.OnPropertyChanged(nameof(this.HasValue));
                this.OnPropertyChanged(nameof(this.Value));
                this.OnPropertyChanged(nameof(this.HasError));
            }
        }

        public SolidColorBrush HasError
        {
            get
            {
                if (this.HasValue)
                {
                    return Brushes.Black;
                }

                return this.PossibleValues.Any()
                    ? Brushes.Black
                    : Brushes.Red;
            }
        }

        public SolidColorBrush Background
        {
            get
            {
                var rc = this.GetMacroZero(this.RowIndex);
                var cc = this.GetMacroZero(this.ColumnIndex);
                var br = rc % 2 == 0;
                var bc = cc % 2 == 0;
                var b = br ^ bc;
                return b
                    ? Brushes.LightGray
                    : Brushes.White;
            }
        }

        public override string ToString()
        {
            return string.Format("RowIndex: {0}, ColumnIndex: {1}, Number: {2}", this.RowIndex, this.ColumnIndex, this.Number);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private int GetMacroZero(int i)
        {
            var r = 3;
            if (i < 3)
            {
                r = 0;
            }

            if (i > 5)
            {
                r = 6;
            }

            return r;
        }
    }
}