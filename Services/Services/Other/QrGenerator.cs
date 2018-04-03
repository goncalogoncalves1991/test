using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Services.Other
{
    public class QrGenerator
    {
        public static Bitmap Generate(string secret)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(secret, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return qrCodeImage;
            //qrCodeImage.Save("C:\\Users\\Gonca\\Desktop\\qrcode.jpg", ImageFormat.Jpeg);
        }

    }


}
