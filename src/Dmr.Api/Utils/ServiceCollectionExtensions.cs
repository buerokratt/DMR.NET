using Buerokratt.Common.AsyncProcessor;
using Buerokratt.Common.CentOps;
using Buerokratt.Common.CentOps.Interfaces;
using Buerokratt.Common.CentOps.Models;
using Buerokratt.Common.Models;
using Dmr.Api.Services.MessageForwarder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;

namespace Dmr.Api.Utils
{
    /// <summary>
    /// Extension class to help add all services related to the DMR
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Helper extension to configure IOC container with <see cref="MessageForwarderService"/> Service and associated services.
        /// </summary>
        /// <param name="services">The services collection that <see cref="MessageForwarderService"/> and related services will be added to.</param>
        /// <param name="settings">A settings object for the <see cref="MessageForwarderService"/></param>
        public static void AddMessageForwarder(this IServiceCollection services, MessageForwarderSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _ = services.AddHttpClient(settings.ClientName, client =>
            {
                client.Timeout = TimeSpan.FromMilliseconds(settings.HttpRequestTimeoutMs);
            });

            services.TryAddSingleton(settings);
            services.TryAddSingleton(settings as AsyncProcessorSettings);
            services.TryAddSingleton<IAsyncProcessorService<Message>, MessageForwarderService>();
            _ = services.AddHostedService<AsyncProcessorHostedService<Message>>();
        }

        /// <summary>
        /// Helper extension to configure IOC container with <see cref="ParticipantPoller"/> Service and associated services.
        /// </summary>
        /// <param name="services">The services collection that <see cref="ParticipantPoller"/> and related services will be added to.</param>
        /// <param name="settings">A settings object for the <see cref="ParticipantPoller"/></param>
        public static void AddParticipantPoller(this IServiceCollection services, CentOpsServiceSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _ = services.AddSingleton(settings);
            _ = services.AddHostedService<ParticipantPoller>();
            _ = services.AddTransient<ICentOpsService, CentOpsService>();
            _ = services.AddSingleton<ConcurrentDictionary<string, Participant>>();
        }
    }
}
