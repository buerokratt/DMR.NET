namespace Dmr.Api.Services.CentOps
{
    public interface ICentOps
    {
        Task<Uri?> TryGetEndpoint(string chatbotId);
    }
}
