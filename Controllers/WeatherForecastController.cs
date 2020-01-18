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
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly MiniProfilerContainer profilerContainer;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, MiniProfilerContainer profilerContainer)
        {
            _logger = logger;
            this.profilerContainer = profilerContainer;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
			var scoped = MiniProfiler.Current;
			scoped.Inline(() => Slowpoke.Sleep(3, 4, 20), "slow with MiniProvider.Current");
            try
            {

                using (profilerContainer.Step("forecasting"))
                {
                    Thread.Sleep(15);
                    var rng = new Random();
                    using (profilerContainer.Step("returning"))
                    {
                        Thread.Sleep(70);

                        return Enumerable.Range(1, 5).Select(index => CreateForecast(index, rng))
                        .ToArray();
                    }
                }
            }
            finally
            {
                scoped.AddProfilerResults(profilerContainer.GetProfiler());
            }
        }

        public WeatherForecast CreateForecast(int index, Random rng)
        {
            var d = DateTime.Now.AddDays(index);
            using (MiniProfiler.Current.Step($"forecast for {d.DayOfWeek}"))
            {
                Thread.Sleep(index * 80);
                return new WeatherForecast
                {
                    Date = d,
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                };
            }
        }
    }
}
