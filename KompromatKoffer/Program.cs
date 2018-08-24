using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace KompromatKoffer
{
    public class Program
    {
        public static string path = System.IO.Directory.GetCurrentDirectory();

        public static string dataDirectory = @"\logs\";

        public static string dataDirectoryLinux = @"/logs";

        public static string logFilePath;

        public static int Main(string[] args)
        {


            CreateCheckDirectoryLog();

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
            logFilePath+"kklog.txt",
            fileSizeLimitBytes: 1_000_000,
            rollOnFileSizeLimit: true,
            shared: true,
            flushToDiskInterval: TimeSpan.FromSeconds(1))
            .CreateLogger();
   
            try
            {
                Log.Information("Starting web host");

                Log.Information("LogFIlePath: " + logFilePath);

                var host = CreateWebHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                }

                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();

        public static void CreateCheckDirectoryLog()
        {
            try
            {
                // Determine whether the directory exists.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    if (System.IO.Directory.Exists(dataDirectoryLinux))
                    {

                        logFilePath = path + dataDirectoryLinux + "/";
                        return;

                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(path + dataDirectory))
                    {
                        logFilePath = path + dataDirectory;
                        return;
                    }
                }

                // Try to create the directory.


                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(path + dataDirectoryLinux);
                }
                else
                {
                    System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(path + dataDirectory);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("...directory creation failed: {0}...", e.ToString());
            }

        }
    }
}
