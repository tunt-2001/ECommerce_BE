using ECommerce.Application;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardService(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<StatsDto> GetStatsAsync(DateTime start, DateTime end)
    {
        var ordersInPeriod = _context.Orders
            .Where(o => o.OrderDate >= start && o.OrderDate <= end && o.Status != "Canceled");

        var totalRevenue = await ordersInPeriod.SumAsync(o => o.TotalAmount);
        var totalOrders = await ordersInPeriod.CountAsync();
        var newCustomers = await _userManager.Users
            .CountAsync(u => u.CreationTime >= start && u.CreationTime <= end);

        return new StatsDto
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            NewCustomers = newCustomers
        };
    }

    public async Task<List<RevenueByDateDto>> GetRevenueChartDataAsync(DateTime start, DateTime end)
    {
        var revenueDataFromDb = await _context.Orders
            .Where(o => o.OrderDate >= start && o.OrderDate <= end && o.Status != "Canceled")
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                Revenue = g.Sum(o => o.TotalAmount)
            })
            .OrderBy(r => r.Date)
            .ToListAsync();

        var result = revenueDataFromDb
            .Select(r => new RevenueByDateDto
            {
                Date = r.Date.ToString("yyyy-MM-dd"),
                Revenue = r.Revenue
            })
            .ToList();

        return result;
    }
}