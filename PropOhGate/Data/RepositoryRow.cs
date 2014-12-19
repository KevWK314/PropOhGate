namespace PropOhGate.Data
{
    public class RepositoryRow
    {
        public int RowId
        {
            get;
            private set;
        }

        public T Get<T>(RepositoryColumn<T> column)
        {
            return column.Get(RowId);
        }

        public void Set<T>(RepositoryColumn<T> column, T value)
        {
            column.Set(RowId, value);
        }

        internal void SetRow(int row)
        {
            RowId = row;
        }

        internal void Clear<T>(RepositoryColumn<T> column)
        {
            column.Clear(RowId);
        }
    }
}
