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
    public class MiniProfilerContainer 
    {
        MiniProfiler profiler;
        public MiniProfilerContainer()
        {}

        static long index = 0; 

        object instanceLock = new object();

        public MiniProfiler GetProfiler()
        {
            if (profiler == null)
            lock (instanceLock)
            if (profiler == null)
            profiler = MiniProfiler.StartNew($"contained {Interlocked.Increment(ref index)}");
            return profiler;
        }

        public string Name => GetProfiler().Name;

        public IDisposable Step(string name)
        {
            return GetProfiler().Step(name);
        }

        public void Inline(Action action, string name)
        {
            GetProfiler().Inline(() => { action(); return 0;}, name);
        }
    }
}