using System.Collections.Generic;
using System.Threading.Tasks;
using Mandrill;
using Mandrill.Model;

namespace firenotes_api.Configuration
{
    public static class Email
    {
        private static MandrillApi Api;
        
        static Email()
        {
            Api = new MandrillApi(Config.MandrillApiKey);
        }
        
        public static async Task<IList<MandrillSendMessageResponse>> Send(string recipient, string subject, string payload)
        {
            var message = new MandrillMessage(Config.ServiceEmail, recipient, subject, payload);
            return await Api.Messages.SendAsync(message);
        }
    }
}