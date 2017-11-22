using dotenv.net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace firenotes_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
            DotEnv.Config(false, "./../../../.env");
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}