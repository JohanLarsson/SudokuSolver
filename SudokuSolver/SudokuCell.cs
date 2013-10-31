using System.ComponentModel;
using System.Runtime.CompilerServices;
using SudokuSolver.Annotations;

namespace SudokuSolver
{
    public class SudokuCell : INotifyPropertyChanged
    {
        public int? Number { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}