using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StyleCart.Domain.Entities;
using StyleCart.Infrastructure.Identity;

namespace StyleCart.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>()
            .Property(product => product.BasePrice)
            .HasPrecision(18, 2);

        builder.Entity<ProductVariant>()
            .Property(variant => variant.Price)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .Property(order => order.TotalAmount)
            .HasPrecision(18, 2);

        builder.Entity<OrderItem>()
            .Property(orderItem => orderItem.UnitPrice)
            .HasPrecision(18, 2);

        builder.Entity<Payment>()
            .Property(payment => payment.Amount)
            .HasPrecision(18, 2);

        builder.Entity<ProductVariant>()
            .HasIndex(variant => variant.SKU)
            .IsUnique();

        builder.Entity<Inventory>()
            .Property(inventory => inventory.RowVersion)
            .IsRowVersion();

        builder.Entity<Inventory>()
            .HasOne(inventory => inventory.ProductVariant)
            .WithOne(variant => variant.Inventory)
            .HasForeignKey<Inventory>(inventory => inventory.ProductVariantId);

        builder.Entity<Payment>()
            .HasOne(payment => payment.Order)
            .WithOne(order => order.Payment)
            .HasForeignKey<Payment>(payment => payment.OrderId);
    }
}