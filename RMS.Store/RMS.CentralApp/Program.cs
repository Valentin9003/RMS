using MassTransit;
using MassTransit.Monitoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using RMS.CentralApp.Infrastructure.Consumers;
using RMS.CentralApp.Infrastructure.Persistence;
using RMS.CentralApp.Mapping;
using RMS.ServiceBusContracts.CentrallApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => x.SwaggerDoc("v1", new OpenApiInfo { Title = "Central", Version = "v1" }));
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(CentralMappingProfile));
builder.Services.AddDbContext<CentralDbContext>(x => x.UseInMemoryDatabase("rms_centralapp"));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StoreProductSyncedEventConsumer>();
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:User"]);
            h.Password(builder.Configuration["RabbitMq:Pass"]);
        });

        cfg.Message<CentralAppProductSyncedEvent>(m =>
        {
            m.SetEntityName("RMSSyncExchange");
        });

        cfg.Publish<CentralAppProductSyncedEvent>(p =>
        {
            p.ExchangeType = "topic";
        });
        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService("CentralApp"))
                .WithMetrics(b => b
                .AddMeter(InstrumentationOptions.MeterName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddPrometheusExporter());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Central"));
app.MapPrometheusScrapingEndpoint("/metrics");
app.MapControllers();
app.Run();
