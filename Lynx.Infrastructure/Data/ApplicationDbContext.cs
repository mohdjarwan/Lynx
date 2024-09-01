using Lynx.Core.Entities;
using Lynx.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lynx.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions  options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IAssemplyMarker).Assembly);
            modelBuilder.Entity<User>().HasQueryFilter(p =>!p.IsDeleted);
        }
    }
}