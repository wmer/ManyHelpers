using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ManyHelpers.Events {
    public class ExcelOpenEndEventArgs : EventArgs {
        public string FileName { get; private set; }
        public string TotalTime { get; private set; }

        public ExcelOpenEndEventArgs(string fileName, string totalTime) {
            FileName = fileName;
            TotalTime = totalTime;
        }
    }


    public delegate void ExcelOpenEndEventHandler(object sender, ExcelOpenEndEventArgs e);
}
