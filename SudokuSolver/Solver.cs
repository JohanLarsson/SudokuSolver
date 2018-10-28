namespace SudokuSolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Solver
    {
        private static int solverCount;

        private readonly Solver antecedent;
        private readonly List<SolverResult> innerResults = new List<SolverResult>();
        private readonly List<Solver> guessSolvers = new List<Solver>();
        private int guessIndex = 0;

        public Solver(SudokuBoard board)
        {
            this.Board = board;
        }

        public Solver(Solver antecedent, SudokuBoard board)
        {
            this.antecedent = antecedent;
            this.Board = board;
        }

        public SudokuBoard Board { get; }

        public List<SolverResult> SolverResults
        {
            get
            {
                if (this.antecedent != null)
                {
                    return this.antecedent.SolverResults.Concat(this.innerResults).ToList();
                }

                return this.innerResults;
            }
        }

        public bool IsDone => this.LastResult == SolverResult.Done;

        private SolverResult LastResult => this.SolverResults.Any()
            ? this.SolverResults.Last()
            : SolverResult.Unknown;

        public bool FindSimple()
        {
            var foundSimple = false;
            var simples = this.Board.AllCells.Where(c => c.PossibleValues.Count == 1).ToArray();
            foreach (var cell in simples)
            {
                if (!cell.PossibleValues.Any())
                {
                    return false;
                }

                this.AddValue(cell, cell.PossibleValues.Single());
                foundSimple = true;
            }

            return foundSimple;
        }

        public bool FindIntersect()
        {
            var foundOne = false;
            var cells = this.Board.AllCells.Where(c => !c.HasValue).ToArray();
            foreach (var cell in cells)
            {
                var allRowValues = cell.Row.SelectMany(x => x.PossibleValues).ToArray();
                var allColumnValues = cell.Column.SelectMany(x => x.PossibleValues).ToArray();
                var allClusterValues = cell.Cluster.SelectMany(x => x.PossibleValues).ToArray();
                foreach (var possibleValue in cell.PossibleValues)
                {
                    if (this.AddIfIntersect(cell, allRowValues, possibleValue))
                    {
                        foundOne = true;
                    }

                    if (this.AddIfIntersect(cell, allColumnValues, possibleValue))
                    {
                        foundOne = true;
                    }

                    if (this.AddIfIntersect(cell, allClusterValues, possibleValue))
                    {
                        foundOne = true;
                    }
                }
            }

            return foundOne;
        }

        public Solver Next()
        {
            if (this.LastResult == SolverResult.Unknown || this.LastResult == SolverResult.FoundOne || !this.innerResults.Any())
            {
                this.SolveStep();
                if (this.LastResult == SolverResult.Guessed)
                {
                    var guessSolver = this.guessSolvers[this.guessIndex];
                    this.guessIndex++;
                    return guessSolver;
                }

                return this;
            }

            if (this.LastResult == SolverResult.Done)
            {
                return this;
            }

            if (this.LastResult == SolverResult.Error)
            {
                return this.antecedent;
            }

            if (this.LastResult == SolverResult.Guessed)
            {
                if (this.guessIndex >= this.guessSolvers.Count)
                {
                    return this.antecedent;
                }

                var guessSolver = this.guessSolvers[this.guessIndex];
                this.guessIndex++;
                guessSolver.SolveStep();
                return guessSolver;
            }

            throw new Exception("message");
        }

        public void SolveStep()
        {
            if (this.FindSimple())
            {
                this.innerResults.Add(SolverResult.FoundOne);
                return;
            }

            if (this.FindIntersect())
            {
                this.innerResults.Add(SolverResult.FoundOne);
                return;
            }

            if (this.Board.AllCells.Any(c => !c.HasValue && !c.PossibleValues.Any()))
            {
                this.innerResults.Add(SolverResult.Error);
                return;
            }

            var remainingCells = this.Board.AllCells.Where(c => !c.HasValue).ToArray();
            if (!remainingCells.Any())
            {
                this.innerResults.Add(SolverResult.Done);
                return;
            }

            var min = remainingCells.Min(x => x.PossibleValues.Count);
            var guessCell = remainingCells.First(x => x.PossibleValues.Count == min);
            for (var index = 0; index < guessCell.PossibleValues.Count; index++)
            {
                var sudokuBoard = this.Board.Clone();
                sudokuBoard.Cells[guessCell.RowIndex, guessCell.ColumnIndex].GuessValue =
                    guessCell.PossibleValues[index];
                var solver = new Solver(this, sudokuBoard);
                this.guessSolvers.Add(solver);
                solverCount++;
            }

            this.innerResults.Add(SolverResult.Guessed);
        }

        private bool AddIfIntersect(SudokuCell cell, IEnumerable<int> possibleValues, int possibleValue)
        {
            if (possibleValues.Count(x => x == possibleValue) != 1)
            {
                return false;
            }

            this.AddValue(cell, possibleValue);
            return true;
        }

        private void AddValue(SudokuCell cell, int value)
        {
            if (this.SolverResults.Any(x => x == SolverResult.Guessed))
            {
                cell.CalculatedAfterGuess = value;
            }
            else
            {
                cell.CalculatedValue = value;
            }
        }
    }
}
