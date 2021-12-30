using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManyHelpers.Collection {
    public static class IEnumerableExtension {
        public static List<List<T>> SplitList<T>(this IEnumerable<T> locations, int nSize = 30) {
            return locations
                        .Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index / nSize)
                        .Select(x => x.Select(v => v.Value).ToList())
                        .ToList();
        }

        public static string TostringList(this IEnumerable<string> list, string separator = ",", string surrounders = "") {
            var strArray = list.Select(x => $"{surrounders}{x}{surrounders}").ToArray();
            var str = string.Join(separator, strArray);

            return str;
        }
    }
}
