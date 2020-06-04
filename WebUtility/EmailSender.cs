using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace DrBAE.WdmServer.WebUtility
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<EmailOptions> options, IWebHostEnvironment env)
        {
            _set = options.Value;
            _env = env;
        }

        public IWebHostEnvironment _env { get; }
        public EmailOptions _set { get; } //set only via Secret Manager

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_set.SenderName, _set.Sender));
                mimeMessage.To.Add(new MailboxAddress(email));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart("html")
                {
                    Text = message
                };

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    if (_env.IsDevelopment())
                    {
                        // The third parameter is useSSL (true if the client should make an SSL-wrapped
                        // connection to the server; otherwise, false).
                        await client.ConnectAsync(_set.MailServer, _set.MailPort, false);
                    }
                    else await client.ConnectAsync(_set.MailServer);

                    // Note: only needed if the SMTP server requires authentication
                    await client.AuthenticateAsync(_set.Sender, _set.Password);
                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }

    }//class
}
