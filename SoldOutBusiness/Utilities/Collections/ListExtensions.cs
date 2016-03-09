﻿using System;
using System.Collections.Generic;

namespace SoldOutBusiness.Utilities.Collections
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> source)
        {
            return source == null || source.Count == 0;
        }

        public static IEnumerable<T> SelectNestedChildren<T>
           (this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (T item in source)
            {
                yield return item;
                foreach (T subItem in SelectNestedChildren(selector(item), selector))
                {
                    yield return subItem;
                }
            }
        }
    }
}
