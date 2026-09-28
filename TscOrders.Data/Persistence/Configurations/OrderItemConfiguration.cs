using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TscOrders.Core.Abstraction.Domain.Entities;

namespace TscOrders.Data.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.OrderId).IsRequired();
        builder.Property(item => item.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(item => item.Quantity).IsRequired();
        builder.Property(item => item.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();

        builder.Ignore(item => item.TotalPrice);
    }
}
