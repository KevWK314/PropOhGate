namespace PropOhGate.Converters
{
    public interface IByteConverter<T>
    {
        byte[] ToByteArray(T value);
        T FromByteArray(byte[] value, int offset);
    }
}
