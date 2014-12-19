using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using PropOhGate.Converters;
using PropOhGate.Message;

namespace PropOhGate.Data
{
    public abstract class Repository : IEnumerable<RepositoryRow>
    {
        private readonly RepositoryColumn<bool> _isActiveColumn;
        private readonly List<RepositoryColumn> _columns = new List<RepositoryColumn>();
        private readonly List<RepositoryRow> _rows = new List<RepositoryRow>();
        private readonly Queue<RepositoryRow> _availableRows = new Queue<RepositoryRow>();
        private readonly ConverterFactory _converters = ConverterFactory.CreateDefault();
        private readonly object _lockObject = new object();

        private readonly Subject<RepositoryRow> _rowAdded = new Subject<RepositoryRow>();
        private readonly Subject<RepositoryRow> _rowRemoved = new Subject<RepositoryRow>();
        private readonly Subject<RepositoryCell> _cellUpdated = new Subject<RepositoryCell>();
        private readonly Subject<Unit> _reset = new Subject<Unit>();

        protected Repository()
        {
            _isActiveColumn = CreateColumn<bool>("IsActive");
            _isActiveColumn.CellUpdated(IsActiveCellUpdated);
        }

        public IEnumerator<RepositoryRow> GetEnumerator()
        {
            lock (_lockObject)
            {
                foreach (var row in _rows)
                {
                    if (row.Get(_isActiveColumn))
                    {
                        yield return row;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public RepositoryRow AddRow()
        {
            lock (_lockObject)
            {
                RepositoryRow newRow;
                if (_availableRows.Count > 0)
                {
                    newRow = _availableRows.Dequeue();
                }
                else
                {
                    newRow = AddNewRow();
                }
                newRow.Set(_isActiveColumn, true);
                return newRow;
            }
        }

        public void RemoveRow(RepositoryRow row)
        {
            lock (_lockObject)
            {
                _availableRows.Enqueue(row);
                row.Set(_isActiveColumn, false);
            }
        }

        public RepositoryRow GetRow(int rowId)
        {
            lock (_lockObject)
            {
                if (rowId >= _rows.Count)
                {
                    throw new ArgumentOutOfRangeException("rowId");
                }
                return _rows[rowId];
            }
        }

        public bool IsActive(int rowId)
        {
            lock (_lockObject)
            {
                return GetRow(rowId).Get(_isActiveColumn);
            }
        }

        public RepositoryColumn GetColumn(int columnId)
        {
            lock (_lockObject)
            {
                if (columnId >= _columns.Count)
                {
                    throw new ArgumentOutOfRangeException("columnId");
                }
                return _columns[columnId];
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var row in _rows)
            {
                if (row.Get(_isActiveColumn))
                {
                    _columns.ForEach(c => sb.Append(c.ToString(row.RowId)).Append("\t"));
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

        public IDisposable SubscribeToRowAdded(Action<RepositoryRow> rowAdded)
        {
            return _rowAdded.Subscribe(rowAdded);
        }

        public IDisposable SubscribeToRowRemoved(Action<RepositoryRow> rowRemoved)
        {
            return _rowRemoved.Subscribe(rowRemoved);
        }

        public IDisposable SubscribeToCellUpdated(Action<RepositoryCell> cellUpdated)
        {
            return _cellUpdated.Subscribe(cellUpdated);
        }

        public IDisposable SubscribeToReset(Action reset)
        {
            return _reset.Subscribe(u => reset());
        }

        public void AddConverter<T>(IByteConverter<T> converter)
        {
            _converters.AddConverter(converter);
        }

        public void Clear()
        {
            _columns.ForEach(c => c.Clear());
            _rows.Clear();
            _reset.OnNext(Unit.Default);
        }

        public string GetRepositoryHash()
        {
            var sb = new StringBuilder();
            _columns.ForEach(c => sb.Append(string.Format("|{0}_{1}", c.Name, c.Type)));
            sb.Append("|");
            return sb.ToString();
        }

        internal void Update(byte[] data)
        {
            if (data != null && data.Length >= Messages.UpdateMetadataLength)
            {
                var col = BitConverter.ToInt32(data, Messages.ColumnIndex);
                var row = BitConverter.ToInt32(data, Messages.RowIndex);
                CreateRowFromId(row);
                _columns[col].SetData(row, data);
            }
        }

        internal RepositoryCell[] GetAllData()
        {
            lock (_lockObject)
            {
                return _columns.SelectMany(c => c.GetCells()).ToArray();
            }
        }

        protected RepositoryColumn<T> CreateColumn<T>(string name)
        {
            lock (_lockObject)
            {
                var converter = _converters.GetConverter<T>();
                if (converter == null)
                {
                    throw new InvalidOperationException("No converter found");
                }

                var column = new RepositoryColumn<T>(_columns.Count, name, converter);
                _columns.Add(column);
                column.CellUpdated(cellUpdate => _cellUpdated.OnNext(cellUpdate));

                return column;
            }
        }

        private void CreateRowFromId(int rowId)
        {
            lock (_lockObject)
            {
                while (_rows.Count <= rowId)
                {
                    AddNewRow();
                }
            }
        }

        private RepositoryRow AddNewRow()
        {
            var newRow = new RepositoryRow();
            newRow.SetRow(_rows.Count);
            _rows.Add(newRow);
            return newRow;
        }

        private void IsActiveCellUpdated(RepositoryCell cell)
        {
            lock (_lockObject)
            {
                if (_isActiveColumn.Get(cell.RowId))
                {
                    _rowAdded.OnNext(GetRow(cell.RowId));
                }
                else
                {
                    _rowRemoved.OnNext(GetRow(cell.RowId));
                }
            }
        }
    }
}
