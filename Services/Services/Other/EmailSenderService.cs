
using System.Net.Mail;
using System.Net;
using System.Drawing;
using System.Net.Mime;
using System.IO;
using System.Drawing.Imaging;
using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Other
{
    public class EmailSenderService
    {
        public static void SendQrCodeToAdmins(@event result)
        {
            var qrCodeImg = QrGenerator.Generate(result.qrcode);

            string body = "EventCommit sends on attachments the QrCode belonging to event "+ result.title;
            string subject = "QRCode for event "+result.title;

            result.community.admins.ToList().ForEach(elem => {
                var mail = new Mail(subject, body, new MailAddress(elem.email, elem.name));
                mail.addImageAttach(qrCodeImg,"QrCode.jpg");
                SendEmail(mail);
              });
        }

        private static void SendEmail(Mail mail)
        {             

             var smtp = new SmtpClient
             {
                 Host = "smtp.gmail.com",
                 Port = 587,
                 EnableSsl = true,
                 DeliveryMethod = SmtpDeliveryMethod.Network,
                 Credentials = new NetworkCredential(mail.FromAddress.Address,mail.FromPass),
                 Timeout = 20000
             };

            using (var message = new MailMessage(mail.FromAddress, mail.ToAddress){ Subject = mail.Subject, Body = mail.Body})
            {
                if (mail.hasAttachment())
                {
                    foreach(Attachment a in mail.Attachments)
                    {
                        message.Attachments.Add(a);
                    }
                }                
                smtp.Send(message);
            }

        }

        public class Mail {

            public List<Attachment> Attachments;
            public MailAddress FromAddress;
            public string FromPass;
            public MailAddress ToAddress;
            public string Subject;
            public string Body;
            public string Image;
            public string ImageName;

            public Mail(string Subject, string Body, MailAddress to) {
                FromAddress = new MailAddress("eventcommit2016@gmail.com", "EventCommIT");
                FromPass = "IselPsG43EventCommit";
                this.Subject = Subject;
                this.Body = Body;
                ToAddress = to;
                Attachments = new List<Attachment>();
            }               
           

            public void addImageAttach(Bitmap qr, string ImageName)
            {
                MemoryStream memStream = new MemoryStream();
                qr.Save(memStream, ImageFormat.Jpeg);
                memStream.Position = 0;
                Attachments.Add(new Attachment(memStream, ImageName));
            }

            public bool hasAttachment()
            {
                return Attachments.Count != 0;
            }
        }

        
    }
}



