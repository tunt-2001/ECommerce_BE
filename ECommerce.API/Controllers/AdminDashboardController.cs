using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public AdminDashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] string period = "today")
    {
        var (startDate, endDate) = GetDateRangeFromPeriod(period);
        var stats = await _dashboardService.GetStatsAsync(startDate, endDate);
        return Ok(stats);
    }

    [HttpGet("revenue-chart")]
    public async Task<IActionResult> GetRevenueChartData([FromQuery] string period = "last7days")
    {
        var (startDate, endDate) = GetDateRangeFromPeriod(period);
        var chartData = await _dashboardService.GetRevenueChartDataAsync(startDate, endDate);
        return Ok(chartData);
    }

    /// <summary>
    /// Helper function to calculate date ranges based on a string period.
    /// </summary>
    private (DateTime, DateTime) GetDateRangeFromPeriod(string period)
    {
        DateTime today = DateTime.Today;
        DateTime startDate = today;
        DateTime endDate = today.AddDays(1).AddTicks(-1); // End of today

        switch (period.ToLower())
        {
            case "week":
                startDate = today.AddDays(-(int)today.DayOfWeek);
                break;
            case "month":
                startDate = new DateTime(today.Year, today.Month, 1);
                break;
            case "last7days":
                startDate = today.AddDays(-6);
                break;
            case "today":
            default:
                break;
        }
        return (startDate.Date, endDate);
    }
}