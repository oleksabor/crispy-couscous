
using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Profiling;

namespace WebApplication1.Controllers
{  
    public class UnitOfWork
    {
        private readonly MiniProfiler profiler;

        public UnitOfWork(MiniProfiler profiler)
        {
            this.profiler = profiler;
        }
        public MiniProfiler calInside(int idx)
        {
            profiler.Inline(() => Slowpoke.Sleep(2, 5, 20), "inside inline");
            using (profiler.Step("inside step"))
                Slowpoke.Sleep(2, 4, 10);
            // mp.Stop();
            return profiler;
        }
    }
}