using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Solver
    {
        private readonly SudokuBoard _board;

        public Solver(SudokuBoard board)
        {
            _board = board;
        }

        public bool FindSimple()
        {
            bool foundOne = false;
            foreach (var cell in _board.Cells)
            {
                if (cell.PossibleValues.Count == 1)
                {
                    cell.GuessedValue = cell.PossibleValues.Single();
                    foundOne = true;
                }
            }
            return foundOne;
        }

        public bool FindIntersect()
        {
            bool foundOne = false;
            foreach (var cell in _board.Cells)
            {
                if (!cell.HasValue)
                {
                    var allRowValues = cell.Row.SelectMany(x => x.PossibleValues).ToArray();
                    var allColumnValues = cell.Column.SelectMany(x => x.PossibleValues).ToArray();
                    foreach (var possibleValue in cell.PossibleValues)
                    {
                        if (allColumnValues.Count(x => x == possibleValue) == 1)
                        {
                            cell.GuessedValue = possibleValue;
                            foundOne = true;
                        }
                        if (allRowValues.Count(x => x == possibleValue) == 1)
                        {
                            cell.GuessedValue = possibleValue;
                            foundOne = true;
                        }
                    }

                }
            }
            return foundOne;
        }
    }
}
