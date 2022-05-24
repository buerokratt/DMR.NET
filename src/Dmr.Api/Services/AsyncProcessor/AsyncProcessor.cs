using System.Collections.Concurrent;
using Dmr.Api.Services.AsyncProcessor;

public abstract class AsyncProcessorService<TPayload, TSettings>
    : IAsyncProcessorService<TPayload>
    where TSettings : AsyncProcessorSettings
{
    protected readonly HttpClient httpClient;
    protected readonly ILogger logger;

    protected readonly ConcurrentQueue<TPayload> requests = new ConcurrentQueue<TPayload>();

    public AsyncProcessorService(IHttpClientFactory httpClientFactory, TSettings config, ILogger logger)
    {
        if (httpClientFactory == null)
        {
            throw new ArgumentNullException(nameof(httpClientFactory));
        }
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        httpClient = httpClientFactory.CreateClient(config.ClientName);
        this.logger = logger;
    }

    public void Enqueue(TPayload payload)
    {
        requests.Enqueue(payload);
    }

    public async Task ProcessRequestsAsync()
    {
        while (this.requests.TryDequeue(out var request))
        {
            await this.ProcessRequest(request).ConfigureAwait(true);
        }
    }

    public abstract Task ProcessRequest(TPayload payload);
}