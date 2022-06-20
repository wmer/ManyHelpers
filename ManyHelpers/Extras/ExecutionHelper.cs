using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManyHelpers.Extras {
    public class ExecutionHelper {
        public string CalculeTime(Action instrution) {
            var finalTime = "";
            var watchGlobal = System.Diagnostics.Stopwatch.StartNew();

            instrution.Invoke();

            watchGlobal.Stop();
            var elapsedMsGlobal = watchGlobal.ElapsedMilliseconds;
            var tGlobal = TimeSpan.FromMilliseconds(elapsedMsGlobal);
            finalTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                    tGlobal.Hours,
                                    tGlobal.Minutes,
                                    tGlobal.Seconds,
                                    tGlobal.Milliseconds);

            return finalTime;
        }

        public async Task<string> CalculeTime(Func<Task> instrution) {
            var finalTime = "";
            var watchGlobal = System.Diagnostics.Stopwatch.StartNew();

            await instrution.Invoke();

            watchGlobal.Stop();
            var elapsedMsGlobal = watchGlobal.ElapsedMilliseconds;
            var tGlobal = TimeSpan.FromMilliseconds(elapsedMsGlobal);
            finalTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                   tGlobal.Hours,
                                   tGlobal.Minutes,
                                   tGlobal.Seconds,
                                   tGlobal.Milliseconds);

            return finalTime;
        }
    }
}
