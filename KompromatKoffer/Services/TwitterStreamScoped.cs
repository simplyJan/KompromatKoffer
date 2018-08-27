using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KompromatKoffer.Services
{
    internal class TwitterStreamScoped : IHostedService
    {
        private readonly ILogger _logger;

        public TwitterStreamScoped(IServiceProvider services,
            ILogger<TwitterStreamScoped> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "===========> TwitterStreamScopedService is starting.");

            DoWork();

            return Task.CompletedTask;
        }

        private void DoWork()
        {
            _logger.LogInformation(
                "===========> TwitterStreamScopedService is working. " + DateTime.Now.ToString("dd.MM.yy - hh:mm"));

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<TwitterStreamService>();
                
                scopedProcessingService.DoWork();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "===========> TwitterStreamScopedService is stopping.");

            return Task.CompletedTask;
        }
    }
}
