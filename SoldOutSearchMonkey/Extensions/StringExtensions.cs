using System.Linq;

namespace SoldOutSearchMonkey.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsAnyOf(this string s, string[] strings)
        {
            return strings.ToList().FirstOrDefault( st => s.Contains(st) ) != null;
        }
    }
}
