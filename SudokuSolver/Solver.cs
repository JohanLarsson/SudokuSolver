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
            bool foundSimple = false;
            var simples = Board.AllCells.Where(c => c.PossibleValues.Count == 1).ToArray();
            foreach (var cell in simples)
            {
                if (!cell.PossibleValues.Any())
                    return false;
                AddValue(cell, cell.PossibleValues.Single());
                foundSimple = true;
            }
            return foundSimple;
        }

        public bool FindIntersect()
        {
            bool foundOne = false;
            var cells = Board.AllCells.Where(c => !c.HasValue).ToArray();
            foreach (var cell in cells)
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
            return foundOne;
        }

        private bool AddIfIntersect(SudokuCell cell, IEnumerable<int> possibleValues, int possibleValue)
        {
            if (possibleValues.Count(x => x == possibleValue) != 1)
                return false;
            AddValue(cell, possibleValue);
            return true;
        }

        private void AddValue(SudokuCell cell, int value)
        {
            if (SolverResults.Any(x => x == SolverResult.Guessed))
            {
                cell.CalculatedAfterGuess = value;
            }
            else
            {
                cell.CalculatedValue = value;
            }
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

            if (Board.AllCells.Any(c => !c.HasValue && !c.PossibleValues.Any()))
            {
                _innerResults.Add(SolverResult.Error);
                return;
            }
            var remainingCells = Board.AllCells.Where(c => !c.HasValue).ToArray();
            if (!remainingCells.Any())
            {
                _innerResults.Add(SolverResult.Done);
                return;
            }
            int min = remainingCells.Min(x => x.PossibleValues.Count);
            SudokuCell guessCell = remainingCells.First(x => x.PossibleValues.Count == min);
            for (int index = 0; index < guessCell.PossibleValues.Count; index++)
            {
                SudokuBoard sudokuBoard = Board.Clone();
                sudokuBoard.Cells[guessCell.RowIndex, guessCell.ColumnIndex].GuessValue =
                    guessCell.PossibleValues[index];
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
