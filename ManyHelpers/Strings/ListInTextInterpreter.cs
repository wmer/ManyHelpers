using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ManyHelpers.Strings {
    public class ListInTextInterpreter {
        public ListInTextInterpreter() {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }


        public string ListFromExcel(string strOriginal, string listlocation, KeyValuePair<string, string> filter = default) {
            if(string.IsNullOrEmpty(strOriginal)) return strOriginal;

            try {
                var trechoOriginal = IsolateTags(strOriginal); 

                if (!string.IsNullOrEmpty(trechoOriginal)) {
                    var filename = GetFileName(strOriginal);
                    var fileLocation = $"{listlocation}\\{filename}";

                    if (!string.IsNullOrEmpty(filename) && System.IO.File.Exists(fileLocation)) {
                        var excelHelper = new ManyHelpers.Excel.ExcelHelper();
                        var bOriginal = excelHelper.GetDataTableFromExcel(fileLocation).AsEnumerable();

                        if (bOriginal.Count() > 0 && filter.Key != null) {
                            bOriginal = bOriginal.Where(x => x.Field<string>(filter.Key) == filter.Value);
                        }

                        var conteudoTrecho = IsolateTags(trechoOriginal, false);

                        var strToRplace = "";

                        foreach (var item in bOriginal) {
                            var labels = item.Table.Columns;

                            var preText = conteudoTrecho;
                            foreach (var table in labels) {
                                if (conteudoTrecho.Contains($"[{table}]")) {
                                    preText = preText.Replace($"[{table}]", item[table.ToString()].ToString());
                                }
                            }

                            strToRplace += $"{preText} {Environment.NewLine}";
                        }

                        strOriginal = strOriginal.Replace(trechoOriginal, strToRplace);
                    }
                }
            } catch { }

            return strOriginal;
        }

        private string IsolateTags(string strOriginal, bool includeTag = true) {
            var content = "";
            var openTag = "<itemlist in=\"";
            var closeOpenTag = "\">";

            var countOpen = openTag.Count() + closeOpenTag.Count() + 1;

            var open = strOriginal.IndexOf(openTag);
            if (open > -1) {
                var filename = GetFileName(strOriginal);

                content = strOriginal.Substring(includeTag ? open : open + countOpen + filename.Count());
                var closeTag = "</itemlist>";
                var countClose = closeTag.Count();

                var close = content.IndexOf(closeTag);
                content = content.Substring(0, includeTag ? close + countClose : close);
            }

            return content;
        }

        private string GetFileName(string strOriginal) {
            var content = "";
            var openTag = "<itemlist in=\"";
            var countOpen = openTag.Count();

            var open = strOriginal.IndexOf(openTag);
            if (open > -1) {
                content = strOriginal.Substring(open + countOpen);
                var closeTag = "\">";

                var close = content.IndexOf(closeTag);
                content = content.Substring(0, close);
            }

            return content;
        }
    }
}
