using Microsoft.AspNetCore.Mvc;

namespace firenotes_api.Controllers
{
    public class DefaultController : Controller
    {
        private string response = "Welcome to the Firenotes API.";

        // GET
        [HttpGet]
        [Route("/")]
        public string Root()
        {
            return $"{response} Start by making requests to the /api routes.";
        }

        // GET
        [HttpGet]
        [Route("/api")]
        public string ApiRoot()
        {
            return response;
        }
    }
}