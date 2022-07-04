using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Dmr.Api.Services.CentOps;
using Dmr.Api.Services.MessageForwarder;
using Dmr.Api.Services.MessageForwarder.Extensions;

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

            var dmrSettings = builder.Configuration.GetSection("DmrServiceSettings").Get<MessageForwarderSettings>();
            builder.Services.AddMessageForwarder(dmrSettings);

            _ = builder.Services.AddHostedService<ParticipantPoller>();
            _ = builder.Services.AddTransient<ICentOpsService, CentOpsService>();
            _ = builder.Services.AddSingleton<ConcurrentDictionary<string, Participant>>();

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