using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Dmr.Api.Services.MessageForwarder.Extensions;
using Dmr.Api.Services.MessageForwarder;
using System;

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
            var settings = new MessageForwarderSettings { ClassifierUri = new Uri("http://classifier"), CentOpsUri = new Uri("http://centops") };

            //Act
            ServiceCollectionExtensions.AddMessageForwarder(services, settings);

            //Assert
            Assert.Contains(services, service => service.ServiceType.Name == "MessageForwarderSettings");
            Assert.Contains(services, service => service.ServiceType.Name == "AsyncProcessorSettings");
            Assert.Contains(services, service => service.ServiceType.Name.Contains("IAsyncProcessorService", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}




