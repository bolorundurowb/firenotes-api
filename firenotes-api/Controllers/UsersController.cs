using AutoMapper;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using firenotes_api.Configuration;
using firenotes_api.Interfaces;
using firenotes_api.Models.Binding;
using firenotes_api.Models.Data;
using Microsoft.Extensions.Logging;

namespace firenotes_api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private IUserService _userService;
        private ILogger _logger;

        public UsersController(ILogger<AuthController> logger, IUserService userService)
        {
            _logger = logger;
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
            
            var user = await _userService.GetUser(id);
            var email = EmailTemplates.GetArchivedAccountEmail();
            var result = await Email.SendAsync(user.Email, "Archived Account", email);
            
            if (result.Count == 0)
            {
                _logger.LogInformation("Forgot password email sent successfully.");
            }
            else
            {
                _logger.LogError("An error occurred when sending ");
            }
            
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