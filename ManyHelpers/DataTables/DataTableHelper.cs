using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ManyHelpers.DataTables {
    public static class DataTableHelper {
        public static DataTable? ToDataTable<T>(this IEnumerable<T> list) {
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            var dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
            return dt;
        }

        public static IEnumerable<T>? ToIEnumerable<T>(this DataTable dt) {
            var json = JsonConvert.SerializeObject(dt, Formatting.Indented);
            var list = (IEnumerable<T>)JsonConvert.DeserializeObject(json, (typeof(IEnumerable<T>)));
            return list;
        }

        public static Dictionary<string, object>? ToDictionary(this DataTable dt) {
            var json = JsonConvert.SerializeObject(dt, Formatting.Indented);
            var list = (Dictionary<string, object>)JsonConvert.DeserializeObject(json, (typeof(Dictionary<string, object>)));
            return list;
        }
    }
}
