using Dmr.Api.Services.AsyncProcessor.Extensions;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Dmr.Api.Services.AsyncProcessor
{
    public abstract class AsyncProcessorService<TPayload, TSettings> : IAsyncProcessorService<TPayload>
        where TSettings : AsyncProcessorSettings
    {
        protected ILogger Logger { get; }

        protected HttpClient HttpClient { get; }

        protected TSettings Config { get; }

        protected ConcurrentQueue<TPayload> Requests { get; } = new();

        protected AsyncProcessorService(IHttpClientFactory httpClientFactory, TSettings config, ILogger logger)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            HttpClient = httpClientFactory.CreateClient(config.ClientName);
            Logger = logger;
            Config = config;
        }

        public void Enqueue(TPayload payload)
        {
            Requests.Enqueue(payload);
        }

        public async Task ProcessRequestsAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var processedRequests = 0;
            while (Requests.TryDequeue(out var request))
            {
                await ProcessRequestAsync(request).ConfigureAwait(false);
                processedRequests++;
            }

            if (processedRequests > 0)
            {
                Logger.AsyncProcessorTelemetry(processedRequests, stopwatch.ElapsedMilliseconds);
            }
        }

        public abstract Task ProcessRequestAsync(TPayload payload);
    }
}