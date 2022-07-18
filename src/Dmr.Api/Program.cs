using System.Diagnostics.CodeAnalysis;
using Buerokratt.Common.CentOps;
using Dmr.Api.Services.MessageForwarder;
using Dmr.Api.Utils;

namespace Dmr.Api
{
    [ExcludeFromCodeCoverage] // This is not solution code, no need for unit tests
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            _ = builder.Services.AddControllers();
            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen();

            // Add the Message Forwarder
            var dmrSettings = builder.Configuration.GetSection(MessageForwarderSettings.SectionName).Get<MessageForwarderSettings>();
            builder.Services.AddMessageForwarder(dmrSettings);

            // Add the Participant Poller and related services.
            var centOpsSettings = builder.Configuration.GetSection(MessageForwarderSettings.SectionName).Get<CentOpsServiceSettings>();
            builder.Services.AddParticipantPoller(centOpsSettings);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI();
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseAuthorization();

            _ = app.MapControllers();

            app.Run();
        }
    }
}