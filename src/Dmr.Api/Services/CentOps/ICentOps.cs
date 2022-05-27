namespace Dmr.Api.Services.CentOps
{
    /// <summary>
    /// Interface which describes CentOps functionality.
    /// </summary>
    public interface ICentOps
    {
        Task<Uri?> TryGetEndpoint(string chatbotId);
    }
}
