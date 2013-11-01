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
                    if (_results.Any(x => x == SolverResult.Guessed))
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
                    var allRowValues = cell.Row.SelectMany(x => x.PossibleValues).ToArray();
                    var allColumnValues = cell.Column.SelectMany(x => x.PossibleValues).ToArray();
                    var allClusterValues = cell.Cluster.SelectMany(x => x.PossibleValues).ToArray();
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
                if (_results.Any(x => x == SolverResult.Guessed))
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

        public Solver Next()
        {
            if (!_results.Any())
            {
                SolveStep();
                return this;
            }
            SolverResult last = _results.Last();
            if (last == SolverResult.FoundOne)
            {
                SolveStep();
                return this;
            }

            if (last == SolverResult.Done)
            {
                return this;
            }
            if (last == SolverResult.Error)
            {
                return this;
            }
            if (last == SolverResult.Guessed)
            {
                foreach (var tempSolver in _tempSolvers)
                {
                    switch (tempSolver._results.LastOrDefault())
                    {
                        case SolverResult.Done:
                            return tempSolver;
                        case SolverResult.Error:
                            continue;
                        case SolverResult.FoundOne:
                            return tempSolver.Next();
                        case SolverResult.Guessed:
                            tempSolver.SolveStep();
                            return tempSolver;
                        case SolverResult.Unknown:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            throw new Exception(string.Format(@"message"));

        }

        private static int _solverCount;

        private readonly List<SolverResult> _innerResults = new List<SolverResult>();
        private  List<SolverResult> _results
        {
            get
            {
                if (_antecedent != null)
                    return _antecedent._results.Concat(_innerResults).ToList();
                return _innerResults;
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
                _tempSolvers.Add(solver);
                _solverCount++;
            }
            _innerResults.Add(SolverResult.Guessed);
            return;
        }

        private readonly List<Solver> _tempSolvers = new List<Solver>();
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
