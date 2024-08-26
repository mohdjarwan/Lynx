using Lynx.Core.Entities;
using Lynx.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Lynx.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IAssemplyMarker).Assembly);

            // Seed data
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, UserName = "Alice", Email = "Alice@gmailcom", Password = "", FirstName = "Alice", LastName = "john", TenantId = 1 },
                new User { Id = 2, UserName = "Bob", Email = "Bob@gmailcom", Password = "", FirstName = "Bob", LastName = "Hanks", TenantId = 2 },
                new User { Id = 3, UserName = "Tom", Email = "Tom@gmailcom", Password = "", FirstName = "Tom", LastName = "teves", TenantId = 1 },
                new User { Id = 4, UserName = "Leonardo", Email = "Leonardo@gmailcom", Password = "", FirstName = "Leonardo", LastName = "john", TenantId = 1 },
                new User { Id = 5, UserName = "Brian", Email = "Brian@gmailcom", Password = "", FirstName = "Brian", LastName = "Hanks", TenantId = 2 }

            );

            modelBuilder.Entity<Tenant>().HasData(
                new Tenant { Id = 1, Name = "GlobalTech" },
                new Tenant { Id = 2, Name = "BlueSkyTech" },
                new Tenant { Id = 1, Name = "AlphaIndustries" },
                new Tenant { Id = 1, Name = "TechGiant" }
                );
        }
    }
}