using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace LE.Web
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // Bootstrap logger covers host start-up (before DI is built); the host
            // then switches to the logger built from configuration below.
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting LE.Web host");
                CreateHostBuilder(args).Build().Run();
                Log.Information("LE.Web host stopped cleanly");
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "LE.Web host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    // Config keys (appsettings "Serilog" section, optional):
                    //   Serilog:MinimumLevel          (default Information)
                    //   Serilog:File:path             (default logs/le-web-.log; rolling day)
                    //   Serilog:File:retainedFileCountLimit (default 31)
                    //   Serilog:File:outputTemplate   (optional)
                    var fileSection = hostingContext.Configuration.GetSection("Serilog:File");
                    string logPath = fileSection["path"];
                    if (string.IsNullOrWhiteSpace(logPath))
                        logPath = Path.Combine("logs", "le-web-.log");

                    string minLevel = hostingContext.Configuration["Serilog:MinimumLevel"];
                    LogEventLevel level = LogEventLevel.Information;
                    if (!string.IsNullOrWhiteSpace(minLevel) && !Enum.TryParse(minLevel, true, out level))
                        level = LogEventLevel.Information;

                    loggerConfiguration
                        .MinimumLevel.Is(level)
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("System", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.File(logPath,
                            rollingInterval: RollingInterval.Day,
                            retainedFileCountLimit: int.TryParse(fileSection["retainedFileCountLimit"], out int limit) ? limit : 31,
                            outputTemplate: fileSection["outputTemplate"] ?? "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                            shared: true);
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
