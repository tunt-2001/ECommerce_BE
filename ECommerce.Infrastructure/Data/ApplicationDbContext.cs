// --- CÁC USING STATEMENTS CẦN THIẾT ---
using ECommerce.Application.Interfaces; // Để thấy được IApplicationDbContext
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Data;

/// <summary>
/// Đại diện cho phiên làm việc với cơ sở dữ liệu.
/// Là cầu nối giữa các đối tượng (Entities) trong code và các bảng trong SQL Server.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    // --- CONSTRUCTOR ---
    // Nhận các tùy chọn cấu hình (ví dụ: chuỗi kết nối) từ Program.cs
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // --- CÁC DBSET TƯƠNG ỨNG VỚI CÁC BẢNG TRONG DATABASE ---
    // Mỗi DbSet<T> đại diện cho một bảng chứa các đối tượng kiểu T.
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }


    /// <summary>
    /// Ghi đè phương thức này để cấu hình chi tiết cho model bằng Fluent API.
    /// Phương thức này được EF Core gọi một lần khi model được tạo lần đầu tiên.
    /// </summary>
    /// <param name="modelBuilder">Đối tượng dùng để xây dựng và cấu hình model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình cho Entity 'Product' (đã có)
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Price).HasColumnType("decimal(18, 2)");
        });

        // Cấu hình cho Entity 'OrderDetail' (đã có)
        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.Property(od => od.Price).HasColumnType("decimal(18, 2)");
        });

        // === THÊM CẤU HÌNH NÀY VÀO ===
        // Cấu hình cho Entity 'Order'
        modelBuilder.Entity<Order>(entity =>
        {
            // Chỉ định rõ kiểu dữ liệu cho cột TotalAmount
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(18, 2)");
        });
    }


    /// <summary>
    /// Ghi đè (override) phương thức SaveChangesAsync để đảm bảo nó khớp chính xác
    /// với "hợp đồng" đã định nghĩa trong interface IApplicationDbContext.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}