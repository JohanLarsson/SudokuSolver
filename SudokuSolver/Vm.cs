namespace SudokuSolver
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Windows.Input;
    using Ookii.Dialogs.Wpf;

    public class Vm : INotifyPropertyChanged
    {
        private bool notNewBoard;
        private List<Solver> solvers = new List<Solver>();
        private string fileName;
        private SudokuBoard board;
        private Solver solver;

        public Vm()
        {
            this.Board = new SudokuBoard();
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Board")
                {
                    if (this.notNewBoard)
                    {
                        return;
                    }

                    this.solver = new Solver(this.Board);
                }
            };
            this.solver = new Solver(this.Board);
            this.SaveCommand = new RelayCommand(o => this.Save());
            this.SaveAsCommand = new RelayCommand(o => this.SaveAs());
            this.OpenCommand = new RelayCommand(o => this.Open());
            this.NewCommand = new RelayCommand(o => this.New());
            this.NextCommand = new RelayCommand(o => this.Next(), o => !this.solver.IsDone);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SaveCommand { get; }

        public ICommand SaveAsCommand { get; }

        public SudokuBoard Board
        {
            get => this.board;
            set
            {
                if (Equals(value, this.board))
                {
                    return;
                }

                this.board = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand OpenCommand { get; }

        public ICommand NewCommand { get; }

        public ICommand NextCommand { get; }

        private void Open()
        {
            var dialog = new VistaOpenFileDialog { Filter = "Sudoku files|*.sud" };
            if (dialog.ShowDialog() == true)
            {
                this.fileName = dialog.FileName;
                var bf = new BinaryFormatter();
                using (var fileStream = File.OpenRead(this.fileName))
                {
                    var sudokuBoard = (SudokuBoard)bf.Deserialize(fileStream);
                    this.Board = sudokuBoard;
                }
            }
        }

        private void Save()
        {
            if (this.fileName == null)
            {
                var dialog = new VistaSaveFileDialog { Filter = "Sudoku files|*.sud" };
                if (dialog.ShowDialog() == true)
                {
                    this.fileName = dialog.FileName;
                }
                else
                {
                    return;
                }
            }

            using (var fileStream = File.OpenWrite(this.fileName))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fileStream, this.Board);
            }
        }

        private void Next()
        {
            this.solver = this.solver.Next();
            this.notNewBoard = true;
            this.Board = this.solver.Board;
            this.notNewBoard = false;
        }

        private void New()
        {
            this.fileName = null;
            this.Board = new SudokuBoard();
        }

        private void SaveAs()
        {
            this.fileName = null;
            this.Save();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
