namespace SudokuSolver
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class SudokuBoard : INotifyPropertyChanged
    {
        private SudokuCell[,] cells;

        public SudokuBoard()
        {
            this.Cells = new SudokuCell[9, 9];
            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    this.Cells[i, j] = new SudokuCell(this, i, j);
                }
            }
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public SudokuCell[,] Cells
        {
            get => this.cells;
            set
            {
                if (ReferenceEquals(value, this.cells))
                {
                    return;
                }

                this.cells = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.AllCells));
            }
        }

        public IEnumerable<SudokuCell> AllCells
        {
            get
            {
                foreach (var sudokuCell in this.Cells)
                {
                    yield return sudokuCell;
                }
            }
        }

        public SudokuBoard Clone()
        {
            var board = new SudokuBoard();
            foreach (var cell in this.AllCells)
            {
                board.Cells[cell.RowIndex, cell.ColumnIndex] = new SudokuCell(board, cell.RowIndex, cell.ColumnIndex)
                {
                    Number = cell.Number,
                    CalculatedValue = cell.CalculatedValue,
                    GuessValue = cell.GuessValue,
                    CalculatedAfterGuess = cell.CalculatedAfterGuess,
                };
            }

            return board;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
