using Microsoft.AspNetCore.Mvc;

namespace firenotes_api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        // POST api/auth/login
        [Route("login"), HttpPost]
        public IActionResult Login()
    }
}