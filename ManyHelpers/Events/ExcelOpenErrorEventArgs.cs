using System;
using System.Collections.Generic;
using System.Text;

namespace ManyHelpers.Events {
    public class ExcelOpenErrorEventArgs : EventArgs {
        public string FileName { get; private set; }
        public string Message { get; private set; }
        public string StackTrace { get; private set; }

        public ExcelOpenErrorEventArgs(string fileName, string message, string stackTrace) {
            FileName = fileName;
            Message = message;
            StackTrace = stackTrace;
        }
    }


    public delegate void ExcelOpenErrorEventHandler(object sender, ExcelOpenErrorEventArgs e);
}
