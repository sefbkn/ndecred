using System.Collections.Generic;

namespace NDecred.Common
{
    public static class EnumerableExtensions
    {
        public static bool Contains<T>(this IEnumerable<T> array, T[] find)
        {
            var matchCount = 0;
            foreach (var element in array)
            {
                if (find[matchCount].Equals(element))
                    matchCount++;                
                if (matchCount == find.Length) return true;
            }

            return false;
        }
    }
}