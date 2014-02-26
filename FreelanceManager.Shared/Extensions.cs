using System;
using System.Collections.Generic;

namespace FreelanceManager
{
    public static class DateExtension
    {
        public static int GetNumericValue(this DateTime dateTime)
        {
            return dateTime.Year * 10000 +
                   dateTime.Month * 100 +
                   dateTime.Day;
        }
    }

    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> getter)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];

            var value = getter(key);

            dictionary.Add(key, value);

            return value;
            
        }
    }
}
