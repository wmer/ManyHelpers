using System;
using System.Collections.Generic;
using System.Text;

namespace ManyHelpers.Extras {
    public class ProgressHelper {

        public static double CalculeProgress(double actualLine, double totalLines) {
            var razao = (double)actualLine / (double)totalLines;
            var percent = razao * 100;
            percent = Math.Round(percent, 2);

            return percent;
        }
    }
}
