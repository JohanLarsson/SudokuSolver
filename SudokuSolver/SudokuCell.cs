using System.ComponentModel;
using System.Runtime.CompilerServices;
using SudokuSolver.Annotations;

namespace SudokuSolver
{
    public class SudokuCell : INotifyPropertyChanged
    {
        private int? _number;

        public int? Number
        {
            get { return _number; }
            set
            {
                if (value == _number) return;
                _number = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}