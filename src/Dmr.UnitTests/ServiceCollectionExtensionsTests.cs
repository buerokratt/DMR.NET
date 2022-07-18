using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Dmr.Api.Services.MessageForwarder;
using System;
using Dmr.Api.Utils;
using Buerokratt.Common.CentOps;

namespace Dmr.UnitTests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void ServiceCollectionExtensionThrowsForNullSettings()
        {
            // Arrange
            var services = new ServiceCollection();

            //Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddMessageForwarder(services, null));
        }

        [Fact]
        public void ServiceCollectionExtensionsAreServicesAdded()
        {
            // Arrange
            var services = new ServiceCollection();
            var settings = new MessageForwarderSettings();

            // Act
            ServiceCollectionExtensions.AddMessageForwarder(services, settings);

            // Assert
            Assert.Contains(services, service => service.ServiceType.Name == "MessageForwarderSettings");
            Assert.Contains(services, service => service.ServiceType.Name == "AsyncProcessorSettings");
            Assert.Contains(services, service => service.ServiceType.Name.Contains("IAsyncProcessorService", StringComparison.InvariantCultureIgnoreCase));
        }


        [Fact]
        public void AddParticipantPollerThrowsforNullSettings()
        {
            // Arrange
            var services = new ServiceCollection();

            //Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddParticipantPoller(services, null));
        }

        [Fact]
        public void AddParticipantPollerSettingsAdded()
        {
            // Arrange
            var services = new ServiceCollection();
            var settings = new CentOpsServiceSettings();

            // Act
            ServiceCollectionExtensions.AddParticipantPoller(services, settings);

            // Assert
            Assert.Contains(services, service => service.ServiceType.Name == "CentOpsServiceSettings");
            Assert.Contains(services, service => service.ServiceType.Name == "IHostedService");
            Assert.Contains(services, service => service.ServiceType.Name == "ICentOpsService");
            Assert.Contains(services, service => service.ServiceType.Name.Contains("ConcurrentDictionary", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}




