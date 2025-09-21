public class GetAllProductsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } // <-- THÊM THUỘC TÍNH NÀY
}