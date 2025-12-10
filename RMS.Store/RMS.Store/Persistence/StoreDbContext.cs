using Microsoft.EntityFrameworkCore;
using RMS.Store.Persistence.Entities;

namespace RMS.Store.Persistence
{

    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

        public DbSet<StoreProduct> Products { get; set; }
    }

}
