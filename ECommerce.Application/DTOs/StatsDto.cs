namespace ECommerce.Application;

public class StatsDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int NewCustomers { get; set; }
}

public class RevenueByDateDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}