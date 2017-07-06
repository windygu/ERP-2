using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Tools
{
    /// <summary>
    /// 测试代码消耗时长
    /// </summary>
    public class AutoStopWatch : System.Diagnostics.Stopwatch, IDisposable
    {
        public AutoStopWatch()
        {
            Start();
        }

        public void Dispose()
        {
            Stop();
            Console.WriteLine("Elapsed: {0}", this.ElapsedMilliseconds);
        }

    }
}
