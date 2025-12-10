namespace RMS.Store.Controllers
{
    using AutoMapper;
    using MassTransit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using RMS.ServiceBusContracts.Store;
    using RMS.Store.Models;
    using RMS.Store.Persistence;
    using RMS.Store.Persistence.Entities;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/products")]
    public class StoreProductsController : ControllerBase
    {
        private readonly StoreDbContext _db;
        private readonly IPublishEndpoint _publisher;
        private readonly IMapper _mapper;

        private const string Store = "billa-lyulin"; // For demo

        public StoreProductsController(StoreDbContext db, IPublishEndpoint publisher, IMapper mapper)
        {
            _db = db;
            _publisher = publisher;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StoreProductRequest request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<StoreProduct>(request);

            product.Id = Guid.NewGuid();
            product.CreatedOn = product.UpdatedOn = DateTime.UtcNow;

            await _db.Products.AddAsync(product, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await _publisher.Publish(new StoreProductCreatedEvent(
                product.Id, 
                Store, 
                product.Name, 
                product.Description,
                product.Price,
                product.MinPrice, 
                product.UpdatedOn));

            return Ok(product.Id);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] StoreProductRequest request)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            _mapper.Map(request, product);

            product.UpdatedOn = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _publisher.Publish(new StoreProductUpdatedEvent(
                product.Id, 
                Store, 
                product.Name, 
                product.Description,
                product.Price,
                product.MinPrice, 
                product.UpdatedOn));

            return Ok();
        }
    }
}
