using System;

namespace firenotes_api.Configuration
{
    public class Config
    {
        public static string DbPath => Environment.GetEnvironmentVariable("MONGO_URL");

        public static string Secret => Environment.GetEnvironmentVariable("SECRET");

        public static string FrontEndUrl => Environment.GetEnvironmentVariable("FRONTEND_URL");
    }
}