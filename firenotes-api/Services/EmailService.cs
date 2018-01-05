using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using firenotes_api.Configuration;
using firenotes_api.Interfaces;

namespace firenotes_api.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendAsync(string recipient, string subject, string payload)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(Config.MailgunBaseUri) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(Config.MailgunApiKey)));
 
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("from", Config.MailgunSmtpLogin),
                    new KeyValuePair<string, string>("to", recipient),
                    new KeyValuePair<string, string>("subject", subject),
                    new KeyValuePair<string, string>("text", payload)
                });
 
                await client.PostAsync(Config.MailgunRequestUri, content).ConfigureAwait(false);
            }
        }
    }
}