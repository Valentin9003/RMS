using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RMS.Store.Consumers;
using RMS.Store.Mapping;
using RMS.Store.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => x.SwaggerDoc("v1", new OpenApiInfo { Title = "Store", Version = "v1" }));
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddAutoMapper(typeof(StoreMappingProfile));

builder.Services.AddDbContext<StoreDbContext>(x => x.UseInMemoryDatabase("rms_store"));


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CentralToStoreProductSyncConsumer>();
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:User"]);
            h.Password(builder.Configuration["RabbitMq:Pass"]);
        });

        var storeId = "billa-lyulin";

        cfg.ReceiveEndpoint($"store-sync-{storeId}", e =>
        {
            e.ConfigureConsumeTopology = false;

            e.Bind("RMSSyncExchange", bind =>
            {
                bind.RoutingKey = $"store-sync-{storeId}";
                bind.ExchangeType = "topic";
            });

            e.ConfigureConsumer<CentralToStoreProductSyncConsumer>(ctx);
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Store"));
app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();

