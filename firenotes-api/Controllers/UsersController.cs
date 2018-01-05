using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using firenotes_api.Configuration;
using firenotes_api.Interfaces;
using firenotes_api.Models.Binding;
using Microsoft.Extensions.Logging;

namespace firenotes_api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public UsersController(ILogger<AuthController> logger, IUserService userService, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
            _userService = userService;
        }
        
        // POST api/users/:id/archive
        [Route("{id}/archive"), HttpPost]
        public async Task<IActionResult> ArchiveUser(string id)
        {
            var callerId = HttpContext.Items["id"].ToString();

            if (callerId != id)
            {
                return BadRequest("You can only archive your own account.");
            }

            await _userService.Archive(id);
            var user = await _userService.GetUser(id);
            var email = EmailTemplates.GetArchivedAccountEmail();
            await _emailService.SendAsync(user.Email, "Archived Account", email);
            _logger.LogInformation("Forgot password email sent successfully.");
            
            return Ok();
        }
        
        // PUT api/users/:id
        [Route("{id}"), HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] UserBindingModel bm)
        {
            var callerId = HttpContext.Items["id"].ToString();

            if (callerId != id)
            {
                return BadRequest("You can only update your own profile.");
            }

            await _userService.Update(id, bm);
            
            return Ok("Profile successfully updated.");
        }
    }
}