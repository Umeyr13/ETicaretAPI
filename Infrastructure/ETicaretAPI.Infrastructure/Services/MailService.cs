﻿using ETicaretAPI.Application.Abstractions.Services;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ETicaretAPI.Infrastructure.Services
{
    public class MailService : IMailService
    {
        readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendMailAsync(new[] {to}, subject,body,isBodyHtml);
        }

        public async Task SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
        {
            MailMessage mail = new();
            mail.IsBodyHtml = isBodyHtml;
            foreach (var to in tos)
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.From = new(_configuration["Mail:Username"], "Mini E-Ticaret", System.Text.Encoding.UTF8);

            SmtpClient smtp = new ();
            smtp.Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]);
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Host = "smtp.gmail.com";
            await smtp.SendMailAsync(mail);
            
        }

        public async Task SendPasswordResetMailAsync(string to, string userId, string resetToken)
        {
            StringBuilder mail = new();
            mail.Append("Merhaba<br>Eğer yeni şifre talebinde bulunduysanız aşağıdaki linkten şifrenizi yenileyebilirsiniz.<br><strong><a target=\"_blank\" href=\"");
            mail.Append(_configuration["AngularClientUrl"]);
            mail.Append("/update-password/");
            mail.Append(userId);
            mail.Append("/");
            mail.Append(resetToken);
            mail.Append("\"> Yeni şifre talebi için tıklayınız...</a></strong><br><br><span style=\"font-size:12px;\"> Not: Eğer ki bu talep tarafınızca gerçekleştirilmediyse lütfen bu maili ciddiye almayınız. </span><br>Saygılarımızla... <br><br><br> Mini E-Ticaret");//a tag ını kapattık
            await SendMailAsync(to,"Şifre Yenileme Talebi", mail.ToString());
       
        }

        public async Task SendCompletedOrderMailAsync(string to, string orderCode, DateTime orderDate, string NameSurname)
        {
            string mail = $"Sayın {NameSurname} Merhaba <br>" +
                $"{orderDate} tarihinde vermiş olduğunuz {orderCode} kodlu siparişiniz tamamlanmış ve kargo firmasına verilmiştir..";
            await SendMailAsync(to, "Sipariş kargoya verildi..", mail);
        }
    }
}
