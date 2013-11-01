using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Solver
    {
        private readonly Solver _antecedent;

        public Solver(SudokuBoard board)
        {
            Board = board;
        }

        public Solver(Solver antecedent, SudokuBoard board)
        {
            _antecedent = antecedent;
            Board = board;
        }

        public SudokuBoard Board { get; private set; }
        public bool FindSimple()
        {
            bool foundOne = false;
            foreach (var cell in Board.Cells)
            {
                if (cell.PossibleValues.Count == 1)
                {
                    if (SolverResults.Any(x => x == SolverResult.Guessed))
                    {
                        cell.GuessValue = cell.PossibleValues.Single();
                    }
                    else
                    {
                        cell.CalculatedValue = cell.PossibleValues.Single();
                    }
                    foundOne = true;
                }
            }
            return foundOne;
        }

        public bool FindIntersect()
        {
            bool foundOne = false;
            foreach (var cell in Board.Cells)
            {
                if (!cell.HasValue)
                {
                    var allRowValues = cell.Row.SelectMany(x => x.PossibleValues);
                    var allColumnValues = cell.Column.SelectMany(x => x.PossibleValues);
                    var allClusterValues = cell.Cluster.SelectMany(x => x.PossibleValues);
                    foreach (var possibleValue in cell.PossibleValues)
                    {
                        if (AddIfIntersect(cell, allRowValues, possibleValue))
                        {
                            foundOne = true;
                        }
                        if (AddIfIntersect(cell, allColumnValues, possibleValue))
                        {
                            foundOne = true;
                        }
                        if (AddIfIntersect(cell, allClusterValues, possibleValue))
                        {
                            foundOne = true;
                        }
                    }

                }
            }
            return foundOne;
        }

        private bool AddIfIntersect(SudokuCell cell, IEnumerable<int> cells, int possibleValue)
        {
            if (cells.Count(x => x == possibleValue) == 1)
            {
                if (SolverResults.Any(x => x == SolverResult.Guessed))
                {
                    cell.GuessValue = possibleValue;
                }
                else
                {
                    cell.CalculatedValue = possibleValue;
                }

                return true;
            }
            return false;
        }

        public Solver Solve()
        {
            while (true)
            {
                //Solver first;
                //if (Next(out first)) return first;
            }
        }

        private SolverResult LastResult
        {
            get
            {
                return SolverResults.Any()
                    ? SolverResults.Last()
                    : SolverResult.Unknown;
            }
        }

        public Solver Next()
        {
            if (LastResult == SolverResult.Unknown || LastResult == SolverResult.FoundOne || !_innerResults.Any())
            {
                SolveStep();
                if (LastResult == SolverResult.Guessed)
                {
                    var guessSolver = _guessSolvers[_guessIndex];
                    _guessIndex++;
                    return guessSolver;
                }
                return this;
            }

            if (LastResult == SolverResult.Done)
            {
                return this;
            }
            if (LastResult == SolverResult.Error)
            {
                return _antecedent;
            }
            if (LastResult == SolverResult.Guessed)
            {

                if (_guessIndex >= _guessSolvers.Count)
                    return _antecedent;
                var guessSolver = _guessSolvers[_guessIndex];
                _guessIndex++;
                guessSolver.SolveStep();
                return guessSolver;
            }
            throw new Exception(string.Format(@"message"));

        }

        private int _guessIndex = 0;

        private static int _solverCount;

        private readonly List<SolverResult> _innerResults = new List<SolverResult>();
        public List<SolverResult> SolverResults
        {
            get
            {
                if (_antecedent != null)
                    return _antecedent.SolverResults.Concat(_innerResults).ToList();
                return _innerResults;
            }
        }

        public bool IsDone
        {
            get
            {
                return LastResult == SolverResult.Done;
            }
        }

        public void SolveStep()
        {
            if (FindSimple())
            {
                _innerResults.Add(SolverResult.FoundOne);
                return;
            }
            if (FindIntersect())
            {
                _innerResults.Add(SolverResult.FoundOne);
                return;
            }
            var remainingCells = new List<SudokuCell>();
            foreach (var cell in Board.Cells)
            {
                if (cell.HasValue)
                    continue;
                if (!cell.PossibleValues.Any())
                {
                    _innerResults.Add(SolverResult.Error);
                    return;
                }
                remainingCells.Add(cell);
            }
            if (!remainingCells.Any())
            {
                _innerResults.Add(SolverResult.Done);
                return;
            }
            int min = remainingCells.Min(x => x.PossibleValues.Count);
            SudokuCell sudokuCell = remainingCells.First(x => x.PossibleValues.Count == min);
            for (int index = 0; index < sudokuCell.PossibleValues.Count; index++)
            {
                SudokuBoard sudokuBoard = Board.Clone();
                sudokuBoard.Cells[sudokuCell.RowIndex, sudokuCell.RowIndex].GuessValue =
                    sudokuCell.PossibleValues[index];
                var solver = new Solver(this, sudokuBoard);
                _guessSolvers.Add(solver);
                _solverCount++;
            }
            _innerResults.Add(SolverResult.Guessed);
            return;
        }

        private readonly List<Solver> _guessSolvers = new List<Solver>();
    }

    public enum SolverResult
    {
        Done,
        Error,
        FoundOne,
        Guessed,
        Unknown
    }
}
