using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.Persistence.Contexts
{
    public class ETicaretAPIDbContext : IdentityDbContext<AppUser,AppRole,string>
    {
        public ETicaretAPIDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
                            /* Table Per Hierarchy - kalıtımsal olarak birbirine bağlılar */
        public DbSet<Domain.Entities.File> Files { get; set; }
        public DbSet<ProductImageFile> ProductImageFiles { get; set; }
        public DbSet<InvoiceFile> InvoiceFiles { get; set; }
        //***
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<CompletedOrder> CompletedOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order>()
                .HasKey(b => b.Id);

            builder.Entity<Order>()
                .HasIndex(o => o.OrderCode)
                .IsUnique();

            builder.Entity<Order>()
                .HasOne(o => o.CompletedOrder)
                .WithOne(c => c.Order)
                .HasForeignKey<CompletedOrder>(c=>c.OrderId);

            builder.Entity<Basket>()
                .HasOne(b=> b.Order)
                .WithOne(o => o.Basket)
                .HasForeignKey<Order>(b=>b.Id);
            
            base.OnModelCreating(builder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var datas = ChangeTracker.Entries<BaseEntity>();
                foreach (var data in datas)
                {
                    _ = data.State switch
                    {
                        EntityState.Added => data.Entity.CreateDate = DateTime.UtcNow,
                        EntityState.Modified => data.Entity.UpdatedDate = DateTime.UtcNow,
                        _ => DateTime.UtcNow// silinmiş veride yukarıdakilerine girip hata vermesin diye boş birşey ekledik...
                    };
                }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
