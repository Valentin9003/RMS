using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RMS.ServiceBusContracts.CentrallApp;
using RMS.Store.Persistence;
using RMS.Store.Persistence.Entities;
using System;
using System.Threading.Tasks;

namespace RMS.Store.Consumers;

public class CentralToStoreProductSyncConsumer : IConsumer<CentralAppProductSyncedEvent>
{
    private readonly StoreDbContext _db;
    private readonly IMapper _mapper;

    public CentralToStoreProductSyncConsumer(StoreDbContext db, IMapper mapper)
        => (_db, _mapper) = (db, mapper);

    public async Task Consume(ConsumeContext<CentralAppProductSyncedEvent> ctx)
    {
        var msg = ctx.Message;

        switch (msg.State)
        {
            case ProductState.Created:
            case ProductState.Updated:
                await SyncUpsert(msg);
                break;

            case ProductState.Deleted:
                await SyncDelete(msg.ProductId);
                break;
        }
    }

    private async Task SyncUpsert(CentralAppProductSyncedEvent msg)
    {
        var entity = await _db.Products.FirstOrDefaultAsync(x => x.Id == msg.ProductId);

        if (entity is null)
        {
            entity = _mapper.Map<StoreProduct>(msg.Payload);
            entity.Id = msg.ProductId; // override auto id
            _db.Products.Add(entity);
        }
        else
        {
            _mapper.Map(msg.Payload, entity);
        }

        entity.UpdatedOn = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    private async Task SyncDelete(Guid productId)
    {
        var entity = await _db.Products.FirstOrDefaultAsync(x => x.Id == productId);
        if (entity is null) return;

        _db.Products.Remove(entity);
        await _db.SaveChangesAsync();
    }
}
