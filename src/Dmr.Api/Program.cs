using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Buerokratt.Common.CentOps;
using Buerokratt.Common.CentOps.Interfaces;
using Buerokratt.Common.CentOps.Models;
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen();

            var sectionName = "DmrServiceSettings";

            var dmrSettings = builder.Configuration.GetSection(sectionName).Get<MessageForwarderSettings>();
            builder.Services.AddMessageForwarder(dmrSettings);

            var centOpsSettings = builder.Configuration.GetSection(sectionName).Get<CentOpsServiceSettings>();
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