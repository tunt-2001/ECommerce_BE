// --- CÁC USING STATEMENTS CẦN THIẾT ---
using ECommerce.Application;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Data.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog; // 1. THÊM USING CHO SERILOG
using System.Text;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    // === KIỂM TRA KỸ ĐOẠN NÀY ===
    .WriteTo.File(
        "Logs/ecommerce-log-.txt", // Đường dẫn ghi file
        rollingInterval: RollingInterval.Day, // Tạo file mới mỗi ngày
        retainedFileCountLimit: 31,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // --- BẢO CHO ASP.NET CORE SỬ DỤNG SERILOG ---
    builder.Host.UseSerilog(); // <-- THAY THẾ LOGGER MẶC ĐỊNH

    // =================================================================
    // 1. CẤU HÌNH CÁC DỊCH VỤ (DEPENDENCY INJECTION)
    // =================================================================

    // --- CẤU HÌNH CORS ---
    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: MyAllowSpecificOrigins,
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:3000")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                          });
    });

    // --- KẾT NỐI DATABASE VÀ MAP INTERFACE ---
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

    // --- CẤU HÌNH IDENTITY ---
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

    // --- CẤU HÌNH XÁC THỰC BẰNG JWT ---
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

    // --- ĐĂNG KÝ MEDIATR ---
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(ECommerce.Application.AssemblyReference).Assembly)
    );

    // --- CÁC DỊCH VỤ KHÁC ---
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        // ... (code swagger của bạn giữ nguyên)
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme { Name = "Authorization", In = Microsoft.OpenApi.Models.ParameterLocation.Header, Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey, Scheme = "Bearer", BearerFormat = "JWT", Description = "Nhập JWT Token theo định dạng sau: Bearer [space] {token}" });
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement { { new Microsoft.OpenApi.Models.OpenApiSecurityScheme { Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } } });
    });
    builder.Services.AddAuthorization();

    // =================================================================
    // XÂY DỰNG ỨNG DỤNG
    // =================================================================
    var app = builder.Build();

    // --- SEED DATABASE ---
    await DbInitializer.SeedRolesAndAdminAsync(app);

    // =================================================================
    // CẤU HÌNH HTTP REQUEST PIPELINE (MIDDLEWARE)
    // =================================================================
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors(MyAllowSpecificOrigins);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}