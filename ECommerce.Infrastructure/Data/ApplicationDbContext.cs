using ECommerce.Application.Interfaces;
using ECommerce.Domain.Common; 
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IEntityWithGuidId).IsAssignableFrom(entityType.ClrType))
            {
                // Cấu hình cho thuộc tính "Id" của entity đó
                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid>("Id")
                    // Bảo SQL Server hãy sử dụng hàm NEWSEQUENTIALID() để tạo giá trị mặc định.
                    // Đây là cách cực kỳ hiệu quả để tạo Guid tuần tự, giúp giảm phân mảnh index.
                    .HasDefaultValueSql("NEWSEQUENTIALID()");
            }
        }

        // === CẤU HÌNH KIỂU DỮ LIỆU DECIMAL (Giữ nguyên) ===
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.Property(od => od.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(18, 2)");
        });
    }


    /// <summary>
    /// Ghi đè phương thức SaveChangesAsync để tự động gán COMB Guid từ phía C#
    /// cho các entity mới được thêm vào.
    /// Đây là một lớp bảo vệ thứ hai, đảm bảo entity luôn có ID trước khi lưu.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Lọc ra tất cả các entry đang ở trạng thái "Added" (sắp được INSERT)
        // và có implement interface IEntityWithGuidId
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IEntityWithGuidId && e.State == EntityState.Added);

        foreach (var entityEntry in entries)
        {
            var idProperty = entityEntry.Property("Id");

            // Nếu ID hiện tại là Guid rỗng (chưa được gán)
            if ((Guid)idProperty.CurrentValue == Guid.Empty)
            {
                // Gọi hàm Generate() từ lớp CombGuid helper để tạo một ID mới
                idProperty.CurrentValue = CombGuid.Generate();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}