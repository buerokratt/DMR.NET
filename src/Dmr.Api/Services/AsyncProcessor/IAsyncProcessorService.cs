namespace Dmr.Api.Services.AsyncProcessor
{
    public interface IAsyncProcessorService<TPayload>
    {
        /// <summary>
        /// Record the given request to be sent to the DMR API later
        /// </summary>
        /// <param name="request">The request object</param>
        void Enqueue(TPayload request);

        /// <summary>
        /// Begin processing requests
        /// </summary>
        Task ProcessRequestsAsync();
    }
}