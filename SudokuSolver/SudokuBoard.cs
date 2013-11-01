using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;

namespace SudokuSolver
{
    [Serializable]
    public class SudokuBoard
    {
        public SudokuBoard()
        {
            Cells = new SudokuCell[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Cells[i, j] = new SudokuCell(this, i, j);
                }
            }
        }


        public SudokuCell[,] Cells { get; set; }

        public IEnumerable<SudokuCell> AllCells
        {
            get
            {
                foreach (var sudokuCell in Cells)
                {
                    yield return sudokuCell;
                }
            }
        }

        public SudokuBoard Clone()
        {
            var board = new SudokuBoard();
            foreach (var sudokuCell in Cells)
            {
                board.Cells[sudokuCell.RowIndex, sudokuCell.ColumnIndex] = new SudokuCell(board, sudokuCell.RowIndex,
                    sudokuCell.ColumnIndex)
                {
                    Number = sudokuCell.Number,
                    CalculatedValue = sudokuCell.CalculatedValue,
                    GuessValue = sudokuCell.GuessValue
                };
            }
            return board;
        }
    }
}
