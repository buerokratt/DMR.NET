namespace Dmr.Api.Services.CentOps
{
    public interface ICentOps
    {
        Task<string> TryGetEndpoint(string chatbotId);
    }
}
