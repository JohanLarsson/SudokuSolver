using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class SudokuBoard
    {
        public SudokuBoard()
        {
            Numbers = new SudokuCell[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Numbers[i, j] = new SudokuCell(this, i, j);
                }
            }
        }
        public SudokuCell[,] Numbers { get; set; }

    }
}
