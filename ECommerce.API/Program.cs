// --- CÁC USING STATEMENTS CẦN THIẾT ---
using ECommerce.API.Hubs;
using ECommerce.API.Services;
using ECommerce.Application;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Data.Seed;
using ECommerce.Infrastructure.Services;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog; 
using System.Security.Cryptography;
using System.Text;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
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

    builder.Host.UseSerilog(); 

    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: MyAllowSpecificOrigins,
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:3000")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials();
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

    // --- CẤU HÌNH DỊCH VỤ HANGFIRE ---
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

    builder.Services.AddHangfireServer();

    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<IDashboardService, DashboardService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IAdminOrderService, AdminOrderService>();
    builder.Services.AddScoped<IProfileService, ProfileService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IAdminMarketingService, AdminMarketingService>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme { Name = "Authorization", In = Microsoft.OpenApi.Models.ParameterLocation.Header, Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey, Scheme = "Bearer", BearerFormat = "JWT", Description = "Nhập JWT Token theo định dạng sau: Bearer [space] {token}" });
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement { { new Microsoft.OpenApi.Models.OpenApiSecurityScheme { Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } } });
    });
    builder.Services.AddAuthorization();
    builder.Services.AddScoped<IQRCodeService, QRCodeService>();
    builder.Services.AddSignalR();
    builder.Services.AddScoped<INotificationService, NotificationService>();

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
    app.UseStaticFiles();
    app.UseCors(MyAllowSpecificOrigins);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[]
        {
        new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            RequireSsl = false,
            Users = new []
            {
                new BasicAuthAuthorizationUser
                {
                    Login = "admin",
                    Password = SHA1.HashData(Encoding.UTF8.GetBytes("admin"))
                }
            }
        })
    }
    });

    app.MapHub<NotificationHub>("/notificationHub"); 

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