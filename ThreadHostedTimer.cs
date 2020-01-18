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
        public ThreadHostedTimer(CancellationToken token, Func<MiniProfilerContainer> factory)
        {
            var timer = Task.Run(() => DoWork(null), token);

            this.token = token;
            this.factory = factory;
        }
        int callCount;
        private readonly CancellationToken token;
        private readonly Func<MiniProfilerContainer> factory;

        void DoWork(object state)
        {
            while (!token.IsCancellationRequested)
            {
                var profiler = MiniProfiler.StartNew($"work {callCount++}");
                Slowpoke.Sleep(1, 5, 20);
                using (profiler.Step("one"))
                    Slowpoke.Sleep(1, 5, 10);
                Slowpoke.Sleep(2, 4, 5);
                using (profiler.StepIf("may be", 100))
                    Slowpoke.Sleep(2, 6, 30);
                using (profiler.Step("last"))
                    Slowpoke.Sleep(1, 5, 10);
                var inner = new UnitOfWork(factory().GetProfiler()).calInside(callCount);
                profiler.AddProfilerResults(inner);
                profiler.Stop();
                Thread.Sleep(5 * 1000);
            }

        }

    }
}

