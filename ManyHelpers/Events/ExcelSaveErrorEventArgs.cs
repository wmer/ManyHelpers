using System;
using System.Collections.Generic;
using System.Text;

namespace ManyHelpers.Events {
    public class ExcelSaveErrorEventArgs : EventArgs {
        public string FileName { get; private set; }
        public string Message { get; private set; }
        public string StackTrace { get; private set; }

        public ExcelSaveErrorEventArgs(string fileName, string message, string stackTrace) {
            FileName = fileName;
            Message = message;
            StackTrace = stackTrace;
        }
    }


    public delegate void ExcelSaveErrorEventHandler(object sender, ExcelSaveErrorEventArgs e);
}
