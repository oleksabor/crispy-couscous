using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication1.Controllers;

namespace WebApplication1
{
	public class Program
	{
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();
            var host = CreateHostBuilder(config).Build();
            var cancel = new CancellationTokenSource();
            var tp = new ThreadHostedTimer(cancel.Token);
            host.Run();
            cancel.Cancel();
        }


        static IWebHostBuilder CreateHostBuilder(IConfigurationRoot config) =>
            new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration()
                .UseStartup<Startup>();
    }
}
