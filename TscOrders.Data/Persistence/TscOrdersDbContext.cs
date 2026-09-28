using Microsoft.EntityFrameworkCore;
using TscOrders.Core.Abstraction.Domain.Entities;
using TscOrders.Data.Persistence.Configurations;

namespace TscOrders.Data.Persistence;

public sealed class TscOrdersDbContext : DbContext
{
    public TscOrdersDbContext(DbContextOptions<TscOrdersDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
    }
}
