using System;
using System.Collections.Generic;
using System.Text;

namespace ManyHelpers.Events {
    public class ExcelReaderEventHub {
        public static event ExcelOpenStartEventHandler OpenStart;
        public static event ExcelOpenProgressEventHandler OpenProgress;
        public static event ExcelOpenEndEventHandler OpenEnd;
        public static event ExcelOpenErrorEventHandler OpenError;
        public static event ExcelSaveErrorEventHandler SaveError;

        public static void OnOpenStart(object sender, ExcelOpenStartEventArgs e) {
            OpenStart?.Invoke(sender, e);
        } 

        public static void OnOpenProgress(object sender, ExcelOpenProgressEventArgs e) {
            OpenProgress?.Invoke(sender, e);
        }
        public static void OnOpenEnd(object sender, ExcelOpenEndEventArgs e) {
            OpenEnd?.Invoke(sender, e);
        }
        public static void OnOpenError(object sender, ExcelOpenErrorEventArgs e) {
            OpenError?.Invoke(sender, e);
        }
        public static void OnSaveError(object sender, ExcelSaveErrorEventArgs e) {
            SaveError?.Invoke(sender, e);
        }
    }
}
