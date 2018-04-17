using System;
using System.Threading.Tasks;
using firenotes_api.Configuration;
using firenotes_api.Interfaces;
using RestSharp;
using RestSharp.Authenticators;

namespace firenotes_api.Services
{
    public class EmailService : IEmailService
    {
        public async Task<IRestResponse> SendAsync(string recipient, string subject, string payload)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(Config.MailgunBaseUri),
                Authenticator = new HttpBasicAuthenticator("api", Config.MailgunApiKey)
            };
            var request = new RestRequest();
            request.AddParameter("domain", Config.MailgunRequestUri, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"Firenotes Team <{Config.ServiceEmail}>");
            request.AddParameter("to", recipient);
            request.AddParameter("subject", subject);
            request.AddParameter("html", payload);
            request.Method = Method.POST;
            return await Task.Run(() => client.Execute(request));
        }
    }
}