using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RMS.CentralApp.Infrastructure.Persistence;
using RMS.CentralApp.Infrastructure.Persistence.Entities;
using RMS.ServiceBusContracts.Store;

namespace RMS.CentralApp.Infrastructure.Consumers
{
    public class StoreProductSyncedEventConsumer :
        IConsumer<StoreProductCreatedEvent>,
        IConsumer<StoreProductUpdatedEvent>
    {
        private readonly CentralDbContext _db;
        private readonly IMapper _mapper;

        public StoreProductSyncedEventConsumer(CentralDbContext db, IMapper mapper)
            => (_db, _mapper) = (db, mapper);

        public async Task Consume(ConsumeContext<StoreProductCreatedEvent> ctx)
        {
            var entity = _mapper.Map<StoreProduct>(ctx.Message);

            await _db.Products.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task Consume(ConsumeContext<StoreProductUpdatedEvent> ctx)
        {
            var msg = ctx.Message;

            var product = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == msg.ProductId);

            if (product is null)
            {
                return;
            }

            _mapper.Map(msg, product);
            product.UpdatedOn = msg.ModifiedOn;

            await _db.SaveChangesAsync();
        }

    }
}
