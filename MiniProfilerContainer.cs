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

        public MiniProfiler GetProfiler()
        {
            return profiler = profiler ?? MiniProfiler.StartNew($"container {Interlocked.Increment(ref index)}");
        }

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