using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1
{
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer;
        MiniProfiler profiler;

        public TimedHostedService(ILogger<TimedHostedService> logger)
        {
            _logger = logger;

            profiler = MiniProfiler.Current ?? MiniProfiler.StartNew("TimedHostedService");
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (profiler.Step($"DoWork {executionCount}"))
            {
                var count = Interlocked.Increment(ref executionCount);
                Thread.Sleep(60);
                _logger.LogInformation(
                    "Timed Hosted Service is working. Count: {Count}", count);
            }
            if ((executionCount % 10) == 0)
            {
                _logger.LogInformation(profiler.RenderPlainText());
                profiler.Stop();

                profiler = MiniProfiler.StartNew($"TimedHostedService {executionCount}");
            }
            
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
