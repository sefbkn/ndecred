using System.Collections.Generic;

namespace NDecred.Common
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines if a set of values exists in an enumerable
        /// </summary>
        /// <param name="array"></param>
        /// <param name="set"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Contains<T>(this IEnumerable<T> array, T[] set)
        {
            var matchCount = 0;
            foreach (var element in array)
            {
                if (set[matchCount].Equals(element))
                    matchCount++;
                else
                    matchCount = 0;
                
                if (matchCount == set.Length) return true;
            }

            return false;
        }
    }
}