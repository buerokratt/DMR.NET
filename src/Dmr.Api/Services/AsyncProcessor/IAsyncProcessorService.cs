namespace Dmr.Api.Services.AsyncProcessor
{
    public interface IAsyncProcessorService<TPayload>
    {
        /// <summary>
        /// Record the given request to be sent to the DMR API later
        /// </summary>
        /// <param name="payload">The payload object</param>
        void Enqueue(TPayload payload);

        /// <summary>
        /// Begin processing requests
        /// </summary>
        Task ProcessRequestsAsync();
    }
}