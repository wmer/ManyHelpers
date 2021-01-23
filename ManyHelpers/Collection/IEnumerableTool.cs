using System;
using System.Collections.Generic;
using System.Text;

namespace ManyHelpers.Collection {
    public class IEnumerableTool {
        public static IEnumerable<int> Range(int de, int para) {
            var list = new List<int>();

            for (int i = de; i <= para; i++) {
                list.Add(i);
            }

            return list;
        }
    }
}
