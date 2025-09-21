using ECommerce.Application.Interfaces;
using QRCoder; // Using từ thư viện đã cài
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ECommerce.Infrastructure.Services;

public class QRCodeService : IQRCodeService
{
    public string GenerateQRCodeBase64(decimal amount, string content)
    {
        // Đây là ví dụ tạo payload cho VietQR, bạn cần thay bằng thông tin thật
        // Bạn có thể tìm hiểu thêm về chuẩn payload của VietQR
        var payload = $"00020101021238570010A000000727012700069704080111STK_CUA_BAN0208QRIBFTTA03037045303{amount:0}540{content.Length:00}{content}5802VN6304XXXX";

        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q))
        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
        {
            byte[] qrCodeImage = qrCode.GetGraphic(20);

            // Chuyển mảng byte của ảnh thành chuỗi Base64
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
        }
    }
}