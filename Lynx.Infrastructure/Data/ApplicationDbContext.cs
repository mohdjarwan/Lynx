using Lynx.Core.Entities;
using Lynx.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Lynx.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();

        foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
        {
            if (entry.Entity is Common commonEntity)
            {
                entry.Property(nameof(Common.CreatedBy)).IsModified = false;
                entry.Property(nameof(Common.CreatedDate)).IsModified = false;
            }
        }
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }


    public ApplicationDbContext(DbContextOptions options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*      modelBuilder
                  .Entity<User>()
                  .Property(e => e.CreatedBy)
                  .ValueGeneratedOnUpdate()
                  .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);            // Interceptors.            
                  modelBuilder.Entity<User>()
                  .Property(e => e.CreatedDate)
                  .ValueGeneratedOnUpdate()
                  .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);*/            // Interceptors. 
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IAssemplyMarker).Assembly);
        // modelBuilder.Entity<User>().HasQueryFilter(p =>!p.IsDeleted);
    }
}