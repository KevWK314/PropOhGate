using System;
using System.Collections.Generic;
using System.Linq;
using PropOhGate.Converters;
using PropOhGate.Message;

namespace PropOhGate.Data
{
    public abstract class RepositoryCell
    {
        protected RepositoryCell(int colId, int rowId)
        {
            ColumnId = colId;
            RowId = rowId;
        }

        public int ColumnId
        {
            get;
            private set;
        }

        public int RowId
        {
            get;
            private set;
        }

        public abstract byte[] Data { get; }
    }

    public class RepositoryCell<T> : RepositoryCell
    {
        private readonly IByteConverter<T> _converter;
        private readonly List<byte> _byteValue = new List<byte>(2);
        private T _value;

        public RepositoryCell(int colId, int rowId, IByteConverter<T> converter)
            : base(colId, rowId)
        {
            _converter = converter;
            _byteValue.AddRange(MessageType.CellUpdate.TypeId);
            _byteValue.AddRange(BitConverter.GetBytes(colId));
            _byteValue.AddRange(BitConverter.GetBytes(rowId));
        }

        public T Value
        {
            get { return _value; }
        }

        public override byte[] Data
        {
            get
            {
                return _byteValue.ToArray();
            }
        }

        public void Clear()
        {
            _value = default(T);
            _byteValue.RemoveRange(Messages.ValueIndex, _byteValue.Count - Messages.UpdateMetadataLength);
        }

        public void UpdateValue(T value)
        {
            _value = value;
            _byteValue.RemoveRange(Messages.ValueIndex, _byteValue.Count - Messages.UpdateMetadataLength);
            _byteValue.AddRange(_converter.ToByteArray(value));
        }

        internal void UpdateData(byte[] data)
        {
            _value = _converter.FromByteArray(data, Messages.ValueIndex);
            _byteValue.RemoveRange(Messages.ValueIndex, _byteValue.Count - Messages.UpdateMetadataLength);
            _byteValue.AddRange(data.Skip(Messages.UpdateMetadataLength));
        }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
}
