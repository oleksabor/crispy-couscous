using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Controllers;

namespace WebApplication1
{
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> _logger;
        private readonly IServiceProvider provider;
        private Timer _timer;

        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceProvider provider)
        {
            _logger = logger;
            this.provider = provider;
            //http://localhost:55694/mini-profiler-resources/results-index
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(6));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = provider.CreateScope()) //like when asp.net controller's method is executed
            {
                var profiler = scope.ServiceProvider.GetService<MiniProfilerContainer>().GetProfiler();
                using (profiler.Step($"DoWork {executionCount}"))
                {
                    var count = Interlocked.Increment(ref executionCount);
                    Thread.Sleep(60);
                    _logger.LogInformation(
                        "Timed Hosted Service is working. Count: {Count}", count);
                }
                var uow = new UnitOfWork(MiniProfiler.StartNew("uow"));
                profiler.AddProfilerResults(uow.calInside(executionCount));
                profiler.Stop();
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
