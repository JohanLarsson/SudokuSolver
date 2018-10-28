namespace SudokuSolver
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SudokuBoard
    {
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

        public SudokuCell[,] Cells { get; set; }

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
    }
}
