using System;
using System.Text;

namespace PropOhGate.Converters
{
    public static class DefaultConverters
    {
        public static void SetupDefault(ConverterFactory factory)
        {
            factory.AddConverter(new IntConverter());
            factory.AddConverter(new DoubleConverter());
            factory.AddConverter(new DateTimeConverter());
            factory.AddConverter(new TimeSpanConverter());
            factory.AddConverter(new StringConverter());
            factory.AddConverter(new BooleanConverter());
        }
    }

    public class IntConverter : IByteConverter<int>
    {
        public byte[] ToByteArray(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public int FromByteArray(byte[] value, int offset)
        {
            if (value.Length == 0)
            {
                return default(int);
            }
            return BitConverter.ToInt32(value, offset);
        }
    }

    public class DoubleConverter : IByteConverter<double>
    {
        public byte[] ToByteArray(double value)
        {
            return BitConverter.GetBytes(value);
        }

        public double FromByteArray(byte[] value, int offset)
        {
            if (value.Length == 0)
            {
                return default(double);
            }
            return BitConverter.ToDouble(value, offset);
        }
    }

    public class DateTimeConverter : IByteConverter<DateTime>
    {
        public byte[] ToByteArray(DateTime value)
        {
            return BitConverter.GetBytes(value.Ticks);
        }

        public DateTime FromByteArray(byte[] value, int offset)
        {
            if (value.Length == 0)
            {
                return default(DateTime);
            }
            return new DateTime(BitConverter.ToInt64(value, offset));
        }
    }

    public class TimeSpanConverter : IByteConverter<TimeSpan>
    {
        public byte[] ToByteArray(TimeSpan value)
        {
            return BitConverter.GetBytes(value.Ticks);
        }

        public TimeSpan FromByteArray(byte[] value, int offset)
        {
            if (value.Length == 0)
            {
                return default(TimeSpan);
            }
            return new TimeSpan(BitConverter.ToInt64(value, offset));
        }
    }

    public class StringConverter : IByteConverter<string>
    {
        public byte[] ToByteArray(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public string FromByteArray(byte[] value, int offset)
        {
            return Encoding.UTF8.GetString(value, offset, value.Length - (offset));
        }
    }

    public class BooleanConverter : IByteConverter<bool>
    {
        public byte[] ToByteArray(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        public bool FromByteArray(byte[] value, int offset)
        {
            return BitConverter.ToBoolean(value, offset);
        }
    }
}
