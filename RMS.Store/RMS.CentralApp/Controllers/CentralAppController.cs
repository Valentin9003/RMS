using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMS.CentralApp.Models;
using RMS.CentralApp.Infrastructure.Persistence;
using RMS.CentralApp.Infrastructure.Persistence.Entities;
using RMS.ServiceBusContracts.CentrallApp;

namespace RMS.CCentral.Controllers;

[ApiController]
[Route("api/central")]
public class CentralAppController(CentralDbContext db, IMapper mapper, IPublishEndpoint publisher) : ControllerBase
{
    [HttpGet("{store}/{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Get(string store, Guid id)
    {
        var product = await db.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.Store == store);

        return product is null
            ? NotFound()
            : Ok(mapper.Map<ProductResponse>(product));
    }

    [HttpPost("{store}")]
    public async Task<ActionResult<ProductResponse>> Create(string store, [FromBody] ProductRequest request)
    {
        var product = mapper.Map<StoreProduct>(request);
        product.ProductId = Guid.NewGuid();
        product.Store = store;
        product.CreatedOn = product.UpdatedOn = DateTime.UtcNow;

        db.Products.Add(product);
        await db.SaveChangesAsync();

        await publisher.Publish(
            new CentralAppProductSyncedEvent
            {
                ProductId = product.ProductId,
                State = ProductState.Created,
                Payload = mapper.Map<CentrallAppProductSyncedEventPayload>(product)
            },
            ctx => ctx.SetRoutingKey($"store-sync-{store}")
        );

        return Ok(mapper.Map<ProductResponse>(product));
    }

    [HttpPut("{store}/{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Update(string store, Guid id, [FromBody] ProductRequest request)
    {
        var product = await db.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.Store == store);

        return product is null
            ? NotFound()
            : await UpdateProduct(store, product, request);
    }

    async Task<ActionResult<ProductResponse>> UpdateProduct(string store, StoreProduct p, ProductRequest req)
    {
        mapper.Map(req, p);
        p.Store = store;
        p.UpdatedOn = DateTime.UtcNow;
        await db.SaveChangesAsync();

        await publisher.Publish(
            new CentralAppProductSyncedEvent
            {
                ProductId = p.ProductId,
                State = ProductState.Updated,
                Payload = mapper.Map<CentrallAppProductSyncedEventPayload>(p)
            },
            ctx => ctx.SetRoutingKey($"store-sync-{store}")
        );

        return Ok(mapper.Map<ProductResponse>(p));
    }

    [HttpDelete("{store}/{id:guid}")]
    public async Task<ActionResult> Delete(string store, Guid id)
    {
        var product = await db.Products.FirstOrDefaultAsync(x => x.ProductId == id && x.Store == store);

        return product is null
            ? NotFound()
            : await DeleteProduct(store, id, product);
    }

    async Task<ActionResult> DeleteProduct(string store, Guid id, StoreProduct p)
    {
        db.Products.Remove(p);
        await db.SaveChangesAsync();

        await publisher.Publish(
            new CentralAppProductSyncedEvent
            {
                ProductId = id,
                State = ProductState.Deleted,
                Payload = null
            },
            ctx => ctx.SetRoutingKey($"store-sync-{store}")
        );

        return Ok();
    }
}
