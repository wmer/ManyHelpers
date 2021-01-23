using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManyHelpers.Prompt {
    public class ConsoleHelper {
        private static string _previousStr;

        public static void WriteInPreviousLine(string str, int line = 1) {
            var strSize = 200;
            if (!string.IsNullOrEmpty(_previousStr)) {
                strSize = _previousStr.Count();
            }


            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - line);

            char[] blankline = new char[strSize];
            Console.WriteLine(blankline, 0, strSize);

            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - line);
            Console.WriteLine(str);

            _previousStr = str;
        } 
    }
}
