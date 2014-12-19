using System;
using System.Collections.Generic;

namespace PropOhGate.Converters
{
    public class ConverterFactory
    {
        private readonly Dictionary<Type, dynamic> _converters = new Dictionary<Type, dynamic>();

        public void AddConverter<T>(IByteConverter<T> converter)
        {
            _converters[typeof(T)] = converter;
        }

        public object GetConverter(Type type)
        {
            var converter = TryGetConverter(type);
            if (converter == null)
            {
                throw new ApplicationException("No converter found for type :" + type);
            }
            return converter;
        }

        public IByteConverter<T> GetConverter<T>()
        {
            var converter = TryGetConverter(typeof(T));
            if (converter == null)
            {
                throw new ApplicationException("No converter found for type :" + typeof(T));
            }
            return converter;
        }

        private dynamic TryGetConverter(Type type)
        {
            dynamic converter;
            _converters.TryGetValue(type, out converter);
            return converter;
        }

        public static ConverterFactory CreateDefault()
        {
            var factory = new ConverterFactory();
            DefaultConverters.SetupDefault(factory);
            return factory;
        }
    }
}
