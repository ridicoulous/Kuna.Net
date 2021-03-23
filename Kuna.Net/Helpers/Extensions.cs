using System.Collections.Generic;

namespace Kuna.Net.Helpers
{
    public static class Extensions
    {
        public static string AsStringParameterOrNull<T>(this List<T> source) => AsStringParameterOrNull(source.ToArray());

        public static string AsStringParameterOrNull<T>(this T[] source)
        {
            return (source.Length == 0) ? null : string.Join(",", source);
        }
    }
}