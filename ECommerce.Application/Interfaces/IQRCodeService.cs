namespace ECommerce.Application.Interfaces;

public interface IQRCodeService
{
    string GenerateQRCodeBase64(decimal amount, string content);
}