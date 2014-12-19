using System.ComponentModel;
using PropOhGate.Data;

namespace PropOhGate.Collection
{
    public abstract class RepositoryItem : INotifyPropertyChanged
    {
        private RepositoryRow _row;

        public event PropertyChangedEventHandler PropertyChanged = (s, a) => { };

        internal void SetRow(RepositoryRow row)
        {
            _row = row;
        }

        public abstract void SetRepository(Repository repository);

        public T Get<T>(RepositoryColumn<T> column)
        {
            return column.Get(_row.RowId);
        }

        public void Set<T>(RepositoryColumn<T> column, T value)
        {
            column.Set(_row.RowId, value);
        }

        public virtual void OnPropertyChanged(RepositoryColumn column)
        {
            OnPropertyChanged(column.Name);
        }

        protected virtual void OnPropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
