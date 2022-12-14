using System.Collections.Generic;

namespace Convo
{
    public static class ContextDataDictionaryExtensions
    {
        public static void AddOrUpdate<T, U>(this Dictionary<T, U> data, T key, U val)
        {
            if (data.ContainsKey(key))
            {
                data[key] = val;
            }
            else
            {
                data.Add(key, val);
            }
        }
    }
}
