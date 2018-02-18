using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace firenotes_api.Controllers
{
    [AllowAnonymous]
    public class DefaultController : Controller
    {
        private string response = "Welcome to the Firenotes API.";

        // GET /
        [Route("/"), HttpGet]
        public string Root()
        {
            return $"{response} Start by making requests to the /api routes.";
        }

        // GET /api
        [Route("/api"), HttpGet]
        public string ApiRoot()
        {
            return response;
        }
    }
}