using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ManyHelpers.Web {
    public class HttpHelper {
        public static string QueryBuilder(Dictionary<string, string> parameters) {
            var builder = new UriBuilder();
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);

            foreach (var iten in parameters) {
                query[iten.Key] = iten.Value;
            }

            string url = query.ToString();

            return url;
        }
    }
}
