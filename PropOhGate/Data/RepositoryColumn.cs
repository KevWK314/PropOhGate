using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using PropOhGate.Converters;

namespace PropOhGate.Data
{
    public abstract class RepositoryColumn
    {
        internal RepositoryColumn(int columnId, string name)
        {
            Name = name;
            ColumnId = columnId;
        }

        public int ColumnId
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public abstract Type Type { get; }

        public abstract string ToString(int row);

        internal abstract void Clear();

        internal abstract void Clear(int row);

        internal abstract RepositoryCell[] GetCells();

        internal abstract void SetData(int rowId, byte[] data);
    }

    public class RepositoryColumn<T> : RepositoryColumn
    {
        private readonly IByteConverter<T> _converter;
        private readonly List<RepositoryCell<T>> _cells = new List<RepositoryCell<T>>();
        private readonly Subject<RepositoryCell> _cellUpdated = new Subject<RepositoryCell>();
 
        internal RepositoryColumn(int columnId, string name, IByteConverter<T> converter)
            : base(columnId, name)
        {
            _converter = converter;
        }

        public override Type Type
        {
            get { return typeof(T); }
        }

        public IDisposable CellUpdated(Action<RepositoryCell> cellUpdated)
        {
            return _cellUpdated.Subscribe(cellUpdated);
        }

        public override string ToString(int rowId)
        {
            if (rowId < _cells.Count)
            {
                var val = _cells[rowId];
                return val != null ? _cells[rowId].ToString() : string.Empty;
            }
            return string.Empty;
        }

        internal T Get(int row)
        {
            lock (_cells)
            {
                return row < _cells.Count ? _cells[row].Value : default(T);
            }
        }

        internal void Set(int rowId, T value)
        {
            lock (_cells)
            {
                if (rowId >= _cells.Count)
                {
                    _cells
                        .AddRange(Enumerable.Range(_cells.Count, rowId - _cells.Count + 1)
                        .Select(r => new RepositoryCell<T>(ColumnId, r, _converter)));
                }
                _cells[rowId].UpdateValue(value);
                Updated(rowId);
            }
        }

        internal override void SetData(int rowId, byte[] data)
        {
            lock (_cells)
            {
                if (rowId >= _cells.Count)
                {
                    _cells
                        .AddRange(Enumerable.Range(_cells.Count, rowId - _cells.Count + 1)
                        .Select(r => new RepositoryCell<T>(ColumnId, r, _converter)));
                }
                _cells[rowId].UpdateData(data);
                Updated(rowId);
            }
        }

        internal override void Clear()
        {
            lock (_cells)
            {
                _cells.Clear();
            }
        }

        internal override void Clear(int rowId)
        {
            lock (_cells)
            {
                if (_cells.Count > rowId)
                {
                    _cells[rowId].Clear();
                }
            }
        }

        internal override RepositoryCell[] GetCells()
        {
            lock (_cells)
            {
                return _cells.ToArray();
            }
        }

        private void Updated(int rowId)
        {
            if(_cellUpdated.HasObservers)
            {
                _cellUpdated.OnNext(_cells[rowId]);
            }
        }
    }
}

