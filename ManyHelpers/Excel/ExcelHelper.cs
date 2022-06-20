using ManyHelpers.Collection;
using ManyHelpers.DataTables;
using ManyHelpers.Events;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ManyHelpers.Excel {
    public class ExcelHelper {
        public event ExcelOpenStartEventHandler OpenStart;
        public event ExcelOpenProgressEventHandler OpenProgress;
        public event ExcelOpenEndEventHandler OpenEnd;
        public event ExcelOpenErrorEventHandler OpenError;
        public event ExcelSaveErrorEventHandler SaveError;

        public void WritCell(string path, string value, int row, int column = 1, int workSheet = 0) {
            DataTable tbl = new DataTable();
            try {
                var watchGlobal = Stopwatch.StartNew();

                var fileInfo = new FileInfo(path);

                using (ExcelPackage excelPackage = new ExcelPackage(fileInfo)) {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[workSheet];

                    worksheet.Cells[row, column].Value = value;

                    excelPackage.Save();
                }


                watchGlobal.Stop();
                var elapsedMsGlobal = watchGlobal.ElapsedMilliseconds;
                var tGlobal = TimeSpan.FromMilliseconds(elapsedMsGlobal);

                string finalTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                        tGlobal.Hours,
                                        tGlobal.Minutes,
                                        tGlobal.Seconds,
                                        tGlobal.Milliseconds);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                ExcelReaderEventHub.OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
            }

        }

        public void WriteRow(string path, DataTable value, int row, int column = 1, int workSheet = 0) {
            DataTable tbl = new DataTable();
            try {
                var watchGlobal = Stopwatch.StartNew();

                var fileInfo = new FileInfo(path);

                using (ExcelPackage excelPackage = new ExcelPackage(fileInfo)) {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[workSheet];
                    worksheet.Cells[row, column].LoadFromDataTable(value, false);

                    excelPackage.Save();
                }


                watchGlobal.Stop();
                var elapsedMsGlobal = watchGlobal.ElapsedMilliseconds;
                var tGlobal = TimeSpan.FromMilliseconds(elapsedMsGlobal);

                string finalTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                        tGlobal.Hours,
                                        tGlobal.Minutes,
                                        tGlobal.Seconds,
                                        tGlobal.Milliseconds);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                ExcelReaderEventHub.OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
            }

        }

        public void WriteColumn(string path, DataTable value, int column = 1, int workSheet = 0) {
            DataTable tbl = new DataTable();
            try {
                var watchGlobal = Stopwatch.StartNew();

                var fileInfo = new FileInfo(path);

                using (ExcelPackage excelPackage = new ExcelPackage(fileInfo)) {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[workSheet];
                    var lastow = worksheet.Dimension.End.Row;
                    worksheet.Cells[lastow++, column].LoadFromDataTable(value, false);

                    excelPackage.Save();
                }


                watchGlobal.Stop();
                var elapsedMsGlobal = watchGlobal.ElapsedMilliseconds;
                var tGlobal = TimeSpan.FromMilliseconds(elapsedMsGlobal);

                string finalTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                        tGlobal.Hours,
                                        tGlobal.Minutes,
                                        tGlobal.Seconds,
                                        tGlobal.Milliseconds);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                ExcelReaderEventHub.OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
            }

        }

        public DataTable GetDataTableFromExcel(string path, int workSheet = 0, bool hasHeader = true) {
            DataTable tbl = new DataTable();
            try {
                var watchGlobal = Stopwatch.StartNew();

                var fileInfo = new FileInfo(path);

                ExcelReaderEventHub.OnOpenStart(null, new ExcelOpenStartEventArgs(path));
                OnOpenStart(null, new ExcelOpenStartEventArgs(path));

                using (var pck = new OfficeOpenXml.ExcelPackage()) {
                    using (var stream = File.OpenRead(path)) {
                        pck.Load(stream);
                    }

                    var ws = pck.Workbook.Worksheets[workSheet];

                    tbl = CreateDataTable(path, hasHeader, fileInfo, ws);
                }


                watchGlobal.Stop();
                var elapsedMsGlobal = watchGlobal.ElapsedMilliseconds;
                var tGlobal = TimeSpan.FromMilliseconds(elapsedMsGlobal);

                string finalTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                        tGlobal.Hours,
                                        tGlobal.Minutes,
                                        tGlobal.Seconds,
                                        tGlobal.Milliseconds);


                ExcelReaderEventHub.OnOpenEnd(null, new ExcelOpenEndEventArgs(path, finalTime));
                OnOpenEnd(null, new ExcelOpenEndEventArgs(path, finalTime));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                ExcelReaderEventHub.OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
                OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
            }


            return tbl;
        }

        public Dictionary<string, DataTable> GetDataTableFromAllWorksheets(string path, int workSheet = 0, bool hasHeader = true) {
            var dts = new Dictionary<string, DataTable>();
            try {
                var watchGlobal = Stopwatch.StartNew();

                var fileInfo = new FileInfo(path);

                ExcelReaderEventHub.OnOpenStart(null, new ExcelOpenStartEventArgs(path));
                OnOpenStart(null, new ExcelOpenStartEventArgs(path));

                using (var pck = new OfficeOpenXml.ExcelPackage()) {
                    using (var stream = File.OpenRead(path)) {
                        pck.Load(stream);
                    }

                    var worksheets = pck.Workbook.Worksheets;

                    foreach (var ws in worksheets) {
                        dts[ws.Name.Trim()] = CreateDataTable(path, hasHeader, fileInfo, ws);
                    }
                }


                watchGlobal.Stop();
                var elapsedMsGlobal = watchGlobal.ElapsedMilliseconds;
                var tGlobal = TimeSpan.FromMilliseconds(elapsedMsGlobal);

                string finalTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                        tGlobal.Hours,
                                        tGlobal.Minutes,
                                        tGlobal.Seconds,
                                        tGlobal.Milliseconds);


                ExcelReaderEventHub.OnOpenEnd(null, new ExcelOpenEndEventArgs(path, finalTime));
                OnOpenEnd(null, new ExcelOpenEndEventArgs(path, finalTime));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                ExcelReaderEventHub.OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
                OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
            }


            return dts;
        }

        private DataTable CreateDataTable(string path, bool hasHeader, FileInfo fileInfo, ExcelWorksheet ws) {
            DataTable tbl = new DataTable();

            foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column]) {
                tbl.Columns.Add(hasHeader ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");
            }
            var startRow = hasHeader ? 2 : 1;
            for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++) {
                try {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();

                    foreach (var cell in wsRow) {
                        if (cell.Start.Column <= tbl.Columns.Count) {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                    }

                    var razao = (double)rowNum / (double)ws.Dimension.End.Row;
                    var percent = razao * 100;

                    percent = Math.Round(percent, 2);


                    ExcelReaderEventHub.OnOpenProgress(null, new ExcelOpenProgressEventArgs(path, ws.Dimension.End.Row, rowNum, $"Abrindo {fileInfo.Name} {percent}% concluido ..."));
                    OnOpenProgress(null, new ExcelOpenProgressEventArgs(path, ws.Dimension.End.Row, rowNum, $"Abrindo {fileInfo.Name} {percent}% concluido ..."));
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                    ExcelReaderEventHub.OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
                    OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
                }
            }

            return tbl;
        }

        public DataTable GetDataTableFromCSV(string path, int workSheet = 0, bool hasHeader = true) {
            DataTable dt = new DataTable();
            try {
                var ci1 = new CultureInfo("pt-BR");

                var fileInfo = new FileInfo(path);
                ExcelReaderEventHub.OnOpenStart(null, new ExcelOpenStartEventArgs(path));
                OnOpenStart(null, new ExcelOpenStartEventArgs(path));

                StreamReader file = new StreamReader(path, Encoding.GetEncoding(ci1.TextInfo.ANSICodePage));
                var lines = file.ReadToEnd().Split(new char[] { '\n' });
                var totalLines = lines.Count();

                StreamReader sr = new StreamReader(path, Encoding.GetEncoding(ci1.TextInfo.ANSICodePage));
                string[] headers = sr.ReadLine().Split(';');

                var h = 0;
                foreach (string header in headers) {
                    dt.Columns.Add(hasHeader ? header : $"column{h}");
                    h++;
                }

                var l = 0;

                while (!sr.EndOfStream) {
                    try {
                        string[] rows = Regex.Split(sr.ReadLine(), ";(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++) {
                            var collum = dt.Columns[i].ColumnName;
                            dr[i] = rows[i];
                        }
                        dt.Rows.Add(dr);

                        var razao = (double)l / (double)totalLines;
                        var percent = razao * 100;

                        percent = Math.Round(percent, 2);


                        ExcelReaderEventHub.OnOpenProgress(null, new ExcelOpenProgressEventArgs(path, totalLines, l, $"Abrindo {fileInfo.Name} {percent}% concluido ..."));
                        OnOpenProgress(null, new ExcelOpenProgressEventArgs(path, totalLines, l, $"Abrindo {fileInfo.Name} {percent}% concluido ..."));
                    } catch (Exception e) {
                        Debug.WriteLine(e.Message);
                        ExcelReaderEventHub.OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
                        OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
                    }

                    l++;
                }
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                ExcelReaderEventHub.OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
                OnOpenError(null, new ExcelOpenErrorEventArgs(path, e.Message, e.StackTrace));
            }

            return dt;
        }


        public bool SaveBase<T>(IEnumerable<T> result, string workSheetName, string fileName) {
            try {
                DataTable dt = result.ToDataTable();

                using (ExcelPackage pck = new ExcelPackage(new FileInfo(fileName))) {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(workSheetName);
                    ws.Cells["A1"].LoadFromDataTable(dt, true);
                    pck.Save();
                }

            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                ExcelReaderEventHub.OnSaveError(null, new ExcelSaveErrorEventArgs(fileName, e.Message, e.StackTrace));
                OnSaveError(null, new ExcelSaveErrorEventArgs(fileName, e.Message, e.StackTrace));
            }

            return true;
        }

        public bool SaveBase(DataTable dt, string workSheetName, string fileName) {
            try {
                using (ExcelPackage pck = new ExcelPackage(new FileInfo(fileName))) {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(workSheetName);
                    ws.Cells["A1"].LoadFromDataTable(dt, true);
                    pck.Save();
                }

            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                ExcelReaderEventHub.OnSaveError(null, new ExcelSaveErrorEventArgs(fileName, e.Message, e.StackTrace));
                OnSaveError(null, new ExcelSaveErrorEventArgs(fileName, e.Message, e.StackTrace));
            }

            return true;
        }

        public void SaveBase<T>(string fileName, IEnumerable<T> result, Action<ExcelPackage, DataTable> opt) {
            try {
                var i = 1;

                var parts = result.SplitList(800000).ToList();

                foreach (var prt in parts) {
                    try {
                        if (i > 1) {
                            var filInfo = new FileInfo(fileName);
                            var dir = filInfo.Directory;
                            var name = filInfo.Name;
                            fileName = $"{dir}\\Part_{i}_{name}";
                        }

                        Console.WriteLine($"Salvando {fileName}...");

                        var json = JsonConvert.SerializeObject(prt, Formatting.Indented);
                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));

                        using (ExcelPackage pck = new ExcelPackage(new FileInfo(fileName))) {
                            opt.Invoke(pck, dt);
                            pck.Save();
                        }


                        Console.WriteLine($"{fileName} foi salvo!!");
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                        ExcelReaderEventHub.OnSaveError(null, new ExcelSaveErrorEventArgs(fileName, e.Message, e.StackTrace));
                        OnSaveError(null, new ExcelSaveErrorEventArgs(fileName, e.Message, e.StackTrace));
                    }


                    i++;
                }

            } catch (Exception e) {
                Console.WriteLine($"Um erro ocorreu: {e.Message}");
                ExcelReaderEventHub.OnSaveError(null, new ExcelSaveErrorEventArgs(fileName, e.Message, e.StackTrace));
                OnSaveError(null, new ExcelSaveErrorEventArgs(fileName, e.Message, e.StackTrace));
            }
        }

        public bool CreateEmpty(string workSheetName, string fileName) {
            try {
                using (ExcelPackage pck = new ExcelPackage(new FileInfo(fileName))) {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(workSheetName);
                    ws.Cells["A1"].Value = "";
                    pck.Save();
                }

            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                ExcelReaderEventHub.OnSaveError(null, new ExcelSaveErrorEventArgs(fileName, e.Message, e.StackTrace));
                OnSaveError(null, new ExcelSaveErrorEventArgs(fileName, e.Message, e.StackTrace));
            }

            return true;
        }


        public void OnOpenStart(object sender, ExcelOpenStartEventArgs e) {
            OpenStart?.Invoke(sender, e);
        }

        public void OnOpenProgress(object sender, ExcelOpenProgressEventArgs e) {
            OpenProgress?.Invoke(sender, e);
        }
        public void OnOpenEnd(object sender, ExcelOpenEndEventArgs e) {
            OpenEnd?.Invoke(sender, e);
        }
        public void OnOpenError(object sender, ExcelOpenErrorEventArgs e) {
            OpenError?.Invoke(sender, e);
        }
        public void OnSaveError(object sender, ExcelSaveErrorEventArgs e) {
            SaveError?.Invoke(sender, e);
        }

    }
}
