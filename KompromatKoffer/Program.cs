using System;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace KompromatKoffer
{
    public class Program
    {
        public static int Main(string[] args)
        {

            string FileName = "log.txt";
            string TargetPath;
            string SourcePath = System.IO.Directory.GetCurrentDirectory();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                TargetPath = SourcePath + @"/logs/";
            }
            else
            {
                TargetPath = SourcePath + @"\logs\";
            }

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(SourcePath, FileName);
            string targetFile = System.IO.Path.Combine(TargetPath, FileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(TargetPath))
            {
                System.IO.Directory.CreateDirectory(TargetPath);
            }

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
            targetFile,
            fileSizeLimitBytes: 10_000_000,
            rollOnFileSizeLimit: true,
            shared: true,
            flushToDiskInterval: TimeSpan.FromSeconds(1))
            .CreateLogger();
   
            try
            {
                Log.Information("===========> Starting web host " + DateTime.Now.ToString("dd.MM.yy - hh:mm"));

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
 
    }
}
