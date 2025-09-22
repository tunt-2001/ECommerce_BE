using ECommerce.Application.Interfaces;
using QRCoder; 
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ECommerce.Infrastructure.Services;

public class QRCodeService : IQRCodeService
{
    public string GenerateQRCodeBase64(decimal amount, string content)
    {
        // payload VietQR
        var payload = $"00020101021238570010A000000727012700069704080111STK_CUA_BAN0208QRIBFTTA03037045303{amount:0}540{content.Length:00}{content}5802VN6304XXXX";

        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q))
        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
        {
            byte[] qrCodeImage = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
        }
    }
}