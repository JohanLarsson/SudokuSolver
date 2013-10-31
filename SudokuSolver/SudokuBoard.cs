﻿using System;
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
