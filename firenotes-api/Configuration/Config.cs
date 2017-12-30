using System;

namespace firenotes_api.Configuration
{
    public class Config
    {
        public static string DbPath => Environment.GetEnvironmentVariable("MONGO_URL");

        public static string Secret => Environment.GetEnvironmentVariable("SECRET");

        public static string FrontEndUrl => Environment.GetEnvironmentVariable("FRONTEND_URL");
        
        public static string MailgunApiKey => Environment.GetEnvironmentVariable("MAILGUN_API_KEY");
        
        public static string MailgunBaseUri => Environment.GetEnvironmentVariable("MAILGUN_BASE_URI");
        
        public static string MailgunRequestUri => Environment.GetEnvironmentVariable("MAILGUN_REQUEST_URI");
        
        public static string MailgunSmtpLogin => Environment.GetEnvironmentVariable("MAILGUN_SMTP_LOGIN");
        
        public static string ServiceEmail => Environment.GetEnvironmentVariable("SERVICE_EMAIL");
    }
}