using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;

namespace WebApplication1.Controllers
{
    public class ThreadHostedTimer
    {
        public ThreadHostedTimer(CancellationToken token)
        {
            var timer = Task.Run(() => DoWork(null), token);
 
            random = new Random();
            this.token = token;
        }
        int callCount;
        Random random;
        private readonly CancellationToken token;

        void DoWork(object state)
        {
            while (!token.IsCancellationRequested)
            {
            var profiler = MiniProfiler.StartNew($"work {callCount++}");
            Sleep(1, 5, 20);
            using (profiler.Step("one"))
                Sleep(1, 5, 10);
            Sleep(2, 4, 5);
            using (profiler.StepIf("may be", 100))
                Sleep(2, 6, 30);
            using (profiler.Step("last"))
                Sleep(1, 5, 10);
            profiler.Stop();
            Thread.Sleep(2*1000);
            }
                
        }
    void Sleep(int p1, int p2, int ms)
    {
        Thread.Sleep(random.Next(p1, p2) * ms);
    }
    }

}