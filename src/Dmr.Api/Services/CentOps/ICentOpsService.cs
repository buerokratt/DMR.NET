﻿namespace Dmr.Api.Services.CentOps
{
    /// <summary>
    /// Interface which describes CentOps functionality.
    /// </summary>
    public interface ICentOpsService
    {
        Task<Uri?> FetchEndpoint(string chatbotId);
    }
}