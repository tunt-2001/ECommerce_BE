// File: ECommerce.Infrastructure/Data/Seed/DbInitializer.cs
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Data.Seed;

public static class DbInitializer
{
    public static async Task SeedRolesAndAdminAsync(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // --- TẠO ROLES ---
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // --- TẠO TÀI KHOẢN ADMIN MẶC ĐỊNH (ĐÃ SỬA LẠI ĐỂ DÙNG USERNAME RIÊNG) ---

            // 1. Định nghĩa các thông tin của admin
            var adminUserName = "admin"; // Đây sẽ là tên đăng nhập
            var adminEmail = "trongtu28032001@gmail.com"; // Đây là email liên lạc
            var adminPassword = "Admin@123";

            // 2. Kiểm tra xem UserName đã tồn tại chưa
            if (await userManager.FindByNameAsync(adminUserName) == null)
            {
                // 3. Tạo đối tượng ApplicationUser với UserName và Email riêng biệt
                var adminUser = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true, // Coi như email đã được xác thực
                    FullName = "Administrator"
                };

                // 4. Tạo người dùng mới với mật khẩu đã định nghĩa
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    // 5. Gán quyền "Admin" cho tài khoản này
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}