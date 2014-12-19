using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using PropOhGate.Data;

namespace PropOhGate.Collection
{
    public class RepositoryObservableCollection<T> : INotifyCollectionChanged, IEnumerable<T>, IDisposable where T : RepositoryItem, new()
    {
        private readonly Repository _repository;
        private readonly List<T> _items = new List<T>();
        private readonly Dictionary<int, T> _itemsLookup = new Dictionary<int, T>();
        private readonly Queue<NotifyCollectionChangedEventArgs> _changes = new Queue<NotifyCollectionChangedEventArgs>();
        private readonly SynchronizationContext _syncContext;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public event NotifyCollectionChangedEventHandler CollectionChanged = (s, a) => { };

        public RepositoryObservableCollection(Repository repository)
        {
            _repository = repository;
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();

            InitialiseRows();
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_items)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    yield return _items[i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            _disposables.ForEach(d => d.Dispose());
        }

        private void InitialiseRows()
        {
            lock (_items)
            {
                _disposables.Add(_repository.SubscribeToRowAdded(RowAdded));
                _disposables.Add(_repository.SubscribeToRowRemoved(RowRemoved));
                _disposables.Add(_repository.SubscribeToCellUpdated(CellUpdated));
                _disposables.Add(_repository.SubscribeToReset(Reset));
                foreach (var row in _repository)
                {
                    RowAdded(row);
                }
            }
        }

        private void RowAdded(RepositoryRow row)
        {
            lock (_items)
            {
                var item = new T();
                item.SetRow(row);
                item.SetRepository(_repository);

                _items.Add(item);
                _itemsLookup[row.RowId] = item;

                AddCollectionChange(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        item));

                ProcessCollectionChanged();
            }
        }

        private void RowRemoved(RepositoryRow row)
        {
            lock (_items)
            {
                T item;
                if (_itemsLookup.TryGetValue(row.RowId, out item))
                {
                    var index = _items.IndexOf(item);
                    _items.RemoveAt(index);

                    AddCollectionChange(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Remove,
                            item, index));

                    ProcessCollectionChanged();
                }
            }
        }

        private void CellUpdated(RepositoryCell cell)
        {
            lock (_items)
            {
                T item;
                if (_itemsLookup.TryGetValue(cell.RowId, out item))
                {
                    item.OnPropertyChanged(_repository.GetColumn(cell.ColumnId));
                }
            }
        }

        private void Reset()
        {
            lock (_items)
            {
                _items.Clear();
                _itemsLookup.Clear();

                AddCollectionChange(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Reset));

                ProcessCollectionChanged();
            }
        }

        private void AddCollectionChange(NotifyCollectionChangedEventArgs args)
        {
            lock (_changes)
            {
                _changes.Enqueue(args);
            }
        }

        private void ProcessCollectionChanged()
        {
            Task.Run(
                () => _syncContext.Post(
                s =>
                    {
                        lock (_changes)
                        {
                            while (_changes.Count > 0)
                            {
                                CollectionChanged(this, _changes.Dequeue());
                            }
                        }
                    },
                null));
        }
    }
}
