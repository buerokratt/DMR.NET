using Dmr.Api.Services.AsyncProcessor.Extensions;

namespace Dmr.Api.Services.AsyncProcessor
{
    /// <summary>
    /// A background hosted service that periodically triggers the request processor
    /// </summary>
    public sealed class AsyncProcessorHostedService<TPayload> : IHostedService, IDisposable
    {
        private readonly IAsyncProcessorService<TPayload> service;
        private readonly AsyncProcessorSettings config;
        private readonly ILogger<AsyncProcessorHostedService<TPayload>> logger;
        private readonly Timer timer;

        public bool IsRunning { get; private set; }

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
            IsRunning = true;
            StartTimer();

            logger.AsyncProcessorStateChange("AsyncProcessor Started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            IsRunning = false;
            StopTimer();

            logger.AsyncProcessorStateChange("AsyncProcessor Stopped");

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
                throw new ArgumentNullException(nameof(state));
            }

            if (state is not AsyncProcessorHostedService<TPayload> self)
            {
                throw new ArgumentException($"Unable to start processor - state doesn't derive from AsyncProcessorHostedService<TPayload>");
            }

            self.StopTimer();

            try
            {
                await self.service.ProcessRequestsAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                self.logger.AsyncProcessorFailed(ex);
                throw;
            }
            finally
            {
                if (self.IsRunning)
                {
                    self.StartTimer();
                }
            }
        }

        public void Dispose()
        {
            timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
