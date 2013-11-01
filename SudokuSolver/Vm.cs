using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;
using SudokuSolver.Annotations;

namespace SudokuSolver
{
    public class Vm : INotifyPropertyChanged
    {
        public Vm()
        {
            Board = new SudokuBoard();
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName =="Board")
                {
                    _solver = new Solver(Board);
                }
            };
            _solver = new Solver(Board);
            SaveCommand = new RelayCommand(o => Save());
            SaveAsCommand = new RelayCommand(o=> SaveAs());
            OpenCommand = new RelayCommand(o => Open());
            NewCommand = new RelayCommand(o => New());
            NextCommand = new RelayCommand(o=>Next());
        }

        private void Next()
        {
            if(_solver.FindSimple())
                return;
            if(_solver.FindIntersect())
                return;

        }

        private void New()
        {
            _fileName = null;
            Board= new SudokuBoard();
        }

        private void SaveAs()
        {
            _fileName = null;
            Save();
        }

        public SudokuBoard Board
        {
            get { return _board; }
            set
            {
                if (Equals(value, _board)) return;
                _board = value;
                OnPropertyChanged();
            }
        }

        private string _fileName;
        private readonly string _filter = "Sudoku files|*.sud";
        private SudokuBoard _board;
        private Solver _solver;

        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        private void Save()
        {
            if (_fileName == null)
            {
                var dialog = new VistaSaveFileDialog { Filter = _filter };
                if (dialog.ShowDialog() == true)
                {
                    _fileName = dialog.FileName;
                }
                else
                {
                    return;
                }

            }
            using (FileStream fileStream = File.OpenWrite(_fileName))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fileStream, Board);
            }
        }

        public ICommand OpenCommand { get; private set; }
        private void Open()
        {
            var dialog = new VistaOpenFileDialog { Filter = _filter };
            if (dialog.ShowDialog() == true)
            {
                _fileName = dialog.FileName;
                var bf = new BinaryFormatter();
                using (var fileStream = File.OpenRead(_fileName))
                {
                    var sudokuBoard = (SudokuBoard)bf.Deserialize(fileStream);
                    Board = sudokuBoard;
                }
            }
        }

        public ICommand NewCommand { get; private set; }

        public ICommand NextCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

        }

    }
}

