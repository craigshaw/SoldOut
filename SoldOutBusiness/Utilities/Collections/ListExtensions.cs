using System.Collections.Generic;

namespace SoldOutBusiness.Utilities.Collections
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> source)
        {
            return source == null || source.Count == 0;
        }
    }
}
