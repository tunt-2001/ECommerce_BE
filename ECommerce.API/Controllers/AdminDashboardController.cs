using ECommerce.Application.Features.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminDashboardController(IMediator mediator) => _mediator = mediator;

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] string period = "today")
    {
        var (startDate, endDate) = GetDateRange(period);
        var query = new GetDashboardStatsQuery { StartDate = startDate, EndDate = endDate };
        return Ok(await _mediator.Send(query));
    }

    [HttpGet("revenue-chart")]
    public async Task<IActionResult> GetRevenueChartData([FromQuery] string period = "last7days")
    {
        var (startDate, endDate) = GetDateRange(period);
        var query = new GetRevenueChartDataQuery { StartDate = startDate, EndDate = endDate };
        return Ok(await _mediator.Send(query));
    }

    // Hàm helper để tính toán khoảng thời gian
    private (DateTime, DateTime) GetDateRange(string period)
    {
        DateTime today = DateTime.Today;
        DateTime startDate = today;
        DateTime endDate = today.AddDays(1).AddTicks(-1); // Cuối ngày

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
        }
        return (startDate, endDate);
    }
}