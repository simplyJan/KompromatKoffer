
using KompromatKoffer.Areas.Database.Model;
using KompromatKoffer.Models;
using KompromatKoffer.Pages;
using LiteDB;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Exceptions;
using Tweetinvi.Models;

namespace KompromatKoffer.Services
{
    internal class BackupService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        public BackupService(ILogger<BackupService> logger)
        {
            _logger = logger;
        }

        public PaginatedList<TwitterUserDailyModel> TwitterUserDailyModel { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("====> BackupService is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(Config.Parameter.DBBackupSpawn));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("====> BackupService Service - " + DateTime.Now.ToString("dd.MM.yy - hh:mm"));

            Task.Delay(Config.Parameter.TaskDelay);

            try
            {
                var dbLastBackup = Config.Parameter.DBLastBackup;


                if (dbLastBackup.AddHours(Config.Parameter.DBBackupInterval) < DateTime.Now)
                {
                    string TargetPath;

                    string FileName = "TwitterData.db";
                    string SourcePath = System.IO.Directory.GetCurrentDirectory();
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        TargetPath = SourcePath + @"/dbbackup/";
                    }
                    else
                    {
                        TargetPath = SourcePath + @"\dbbackup\";
                    }

                    // Use Path class to manipulate file and directory paths.
                    string sourceFile = System.IO.Path.Combine(SourcePath, FileName);
                    string destFile = System.IO.Path.Combine(TargetPath, DateTime.Now.ToString("dd-MM-yy-hh-mm")+FileName);

                    // To copy a folder's contents to a new location:
                    // Create a new target folder, if necessary.
                    if (!System.IO.Directory.Exists(TargetPath))
                    {
                        System.IO.Directory.CreateDirectory(TargetPath);
                    }

                    // To copy a file to another location and 
                    // overwrite the destination file if it already exists.
                    System.IO.File.Copy(sourceFile, destFile, true);
  
                }

            }
            catch (TwitterException ex)
            {
                _logger.LogInformation("Twitter has problems..." + ex);
            }
            catch (ArgumentException ex)
            {
                _logger.LogInformation("ArgumentException..." + ex);

            }
            catch (LiteException ex)
            {
                _logger.LogInformation("LiteDB Exception..." + ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception..." + ex);
            }


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("====> BackupService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();


        }

    }
}

