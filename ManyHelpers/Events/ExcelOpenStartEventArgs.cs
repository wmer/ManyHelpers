using System;
using System.Collections.Generic;
using System.Text;

namespace ManyHelpers.Events {
    public class ExcelOpenStartEventArgs : EventArgs {
        public string FileName { get; private set; }

        public ExcelOpenStartEventArgs(string fileName) {
            FileName = fileName;
        }
    }

    public delegate void ExcelOpenStartEventHandler(object sender, ExcelOpenStartEventArgs e);
}
