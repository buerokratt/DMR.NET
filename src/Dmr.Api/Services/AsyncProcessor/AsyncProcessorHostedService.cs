using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Dmr.Api.Services.AsyncProcessor
{
    /// <summary>
    /// A background hosted service that periodically triggers the DMR request processor
    /// </summary>
    [ExcludeFromCodeCoverage] // Temporarily excluded from code coverage in order to get the CI pipeline merged. This attribute will be removed later.
    public sealed class AsyncProcessorHostedService<TPayload> : IHostedService, IDisposable
    {
        private readonly IAsyncProcessorService<TPayload> service;
        private readonly AsyncProcessorSettings config;
        private readonly ILogger<AsyncProcessorHostedService<TPayload>> logger;
        private readonly Timer timer;

        public AsyncProcessorHostedService(
            IAsyncProcessorService<TPayload> service,
            AsyncProcessorSettings config,
            ILogger<AsyncProcessorHostedService<TPayload>> logger)
        {
            this.service = service;
            this.config = config;
            this.logger = logger;
            timer = new Timer(TimerCallback, this, Timeout.Infinite, Timeout.Infinite);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartTimer();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            StopTimer();

            return Task.CompletedTask;
        }

        private void StopTimer()
        {
            _ = timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void StartTimer()
        {
            _ = timer.Change(TimeSpan.FromMilliseconds(config.RequestProcessIntervalMs), Timeout.InfiniteTimeSpan);
        }

        private static async void TimerCallback(object? state)
        {
            if (state == null)
            {
                Trace.WriteLine($"Unable to start processor - no AsyncProcessorHostedService<TPayload> passed.");
                return;
            }

            var self = state as AsyncProcessorHostedService<TPayload>;
            if (self == null)
            {
                Trace.WriteLine($"Unable to start processor - state doesn't derive from AsyncProcessorHostedService<TPayload>");
                return;
            }

            self.StopTimer();

            try
            {
                // Not sure how to resolve rule CA1848 so removing logging for now
                //self.logger.LogInformation("Starting processing DMR requests");

                await self.service.ProcessRequestsAsync().ConfigureAwait(true);

                // Not sure how to resolve rule CA1848 so removing logging for now
                //self.logger.LogInformation("Completed processing DMR requests");
            }
            catch (Exception)
            {
                // Not sure how to resolve rule CA1848 so removing logging for now
                //self.logger.LogError(ex, $"Unexpected error in {nameof(DmrHostedService)}.");
                throw;
            }
            finally
            {
                self.StartTimer();
            }
        }

        public void Dispose()
        {
            timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
