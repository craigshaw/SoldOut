using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SoldOutTests.Extensions
{
    public static class IEnumerableTestExtensions
    {
        public static void AssertSequenceIsEqual<T>(this IEnumerable<T> actual, params T[] expected)
        {
            List<T> copy = new List<T>(actual);
            Assert.AreEqual(expected.Length, copy.Count, "Expected counts to be equal");

            for (int i = 0; i < copy.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(expected[i], copy[i]))
                {
                    Assert.Fail(string.Format("Sequences differ at index {0}. Expected {1}, got {2}",
                        i, expected[i], copy[i]));
                }
            }
        }
    }
}
