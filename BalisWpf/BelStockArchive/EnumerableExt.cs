using System;
using System.Collections.Generic;

namespace BalisWpf
{
    public static class EnumerableExt
    {
        public static void Do<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
        }
    }
}