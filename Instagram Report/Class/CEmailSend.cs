using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instagram_Email_Scrape.Class
{
    class CEmailSend
    {
        public async Task SendGridEmail(string subject, string content, string from, string[] to, string sName, string sKey)
        {
            var apiKey = Environment.GetEnvironmentVariable(sName);
            var client = new SendGridClient(sKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(from, from),
                Subject = subject,
                PlainTextContent = content,
                HtmlContent = content
            };
            foreach (var item in to)
            {
                msg.AddTo(new EmailAddress(item, item));
            }
            var response = await client.SendEmailAsync(msg);
        }
    }
}
