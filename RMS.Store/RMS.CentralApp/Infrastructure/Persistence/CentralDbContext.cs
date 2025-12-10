using Microsoft.EntityFrameworkCore;
using RMS.CentralApp.Infrastructure.Persistence.Entities;

namespace RMS.CentralApp.Infrastructure.Persistence
{

    public class CentralDbContext : DbContext
    {
        public CentralDbContext(DbContextOptions<CentralDbContext> options) : base(options) { }

        public DbSet<StoreProduct> Products { get; set; }
    }

}
