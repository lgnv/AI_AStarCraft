using System.Collections.Generic;

namespace AI_AStarCraft.Helpers
{
    public static class IEnumerableExtensions
    {
        public static string StrJoin<T>(this IEnumerable<T> source, string separator)
            => string.Join(separator, source);
    }
}