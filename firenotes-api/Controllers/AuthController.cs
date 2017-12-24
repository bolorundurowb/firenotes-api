using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using firenotes_api.Configuration;
using firenotes_api.Models.Binding;
using firenotes_api.Models.Data;
using firenotes_api.Models.View;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace firenotes_api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private IMongoDatabase _mongoDatabase;
        private IMapper _mapper;
        private ILogger _logger;

        public AuthController(IMapper mapper, ILogger<AuthController> logger)
        {
            _mapper = mapper;
            _logger = logger;
            
            var dbPath = Config.DbPath;
            var mongoClient = new MongoClient(dbPath);
            _mongoDatabase = mongoClient.GetDatabase(Startup.DatabaseName);
        }
        
        // POST api/auth/login
        [Route("login"), HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginBindingModel data)
        {
            if (data == null)
            {
                return BadRequest("The payload must not be null.");
            }
            
            if (string.IsNullOrWhiteSpace(data.Email))
            {
                return BadRequest("An email address is required.");
            }

            if (string.IsNullOrWhiteSpace(data.Password))
            {
                return BadRequest("A password is required.");
            }
            
            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            var user = await usersCollection.Find(x => x.Email == data.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("A user with that email address doesn't exist.");
            }

            if (!BCrypt.Net.BCrypt.Verify(data.Password, user.HashedPassword))
            {
                return Unauthorized();
            }

            var token = Helpers.GenerateToken("id", user.Id);
            var result = _mapper.Map<AuthViewModel>(user);
            result.Token = token;
            return Ok(result);
        }

        // POST api/auth/signup
        [Route("signup"), HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpBindingModel data)
        {
            if (data == null)
            {
                return BadRequest("The payload must not be null.");
            }
            
            if (string.IsNullOrWhiteSpace(data.Email))
            {
                return BadRequest("An email address is required.");
            }

            if (string.IsNullOrWhiteSpace(data.Password))
            {
                return BadRequest("A password is required.");
            }

            if (data.Password.Length < 8)
            {
                return BadRequest("The password cannot be less than 8 characters.");
            }


            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            var user = await usersCollection.Find(x => x.Email == data.Email).FirstOrDefaultAsync();

            if (user != null)
            {
                var response = new ContentResult
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Content = "Sorry, a user with that email already exists."
                };
                return response;
            }

            var email = EmailTemplates.GetWelcomeEmail();
            var emailStatus = await Email.Send(data.Email, "Forgot Password", email);
            if (emailStatus.Count == 0)
            {
                _logger.LogInformation("Forgot password email sent successfully.");
            }
            else
            {
                _logger.LogError("An error occurred when sending ");
            }
            
            user = new User
            {
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                Password = data.Password
            };
            await usersCollection.InsertOneAsync(user);
            
            var token = Helpers.GenerateToken("id", user.Id);
            var result = _mapper.Map<AuthViewModel>(user);
            result.Token = token;
            return Ok(result);
        }

        [Route("forgot-password"), HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordBindingModel bm)
        {
            if (bm == null)
            {
                return BadRequest("The payload must not be null.");
            }

            if (string.IsNullOrWhiteSpace(bm.Email))
            {
                return BadRequest("An email address is required.");
            }
            
            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            var user = await usersCollection.Find(x => x.Email == bm.Email).FirstOrDefaultAsync();
            
            if (user == null)
            {
                return NotFound("A user with that email address doesn't exist.");
            }

            var token = Helpers.GenerateToken("email", bm.Email, 12);
            var email = EmailTemplates.GetForgotPasswordEmail($"{Config.FrontEndUrl}/auth/reset-password?token={token}");
            var result = await Email.Send(bm.Email, "Forgot Password", email);
            if (result.Count == 0)
            {
                _logger.LogInformation("Forgot password email sent successfully.");
            }
            else
            {
                _logger.LogError("An error occurred when sending ");
            }

            return Ok("Your password reset email has been sent.");
        }

        [Route("reset-pssword"), HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordBindingModel bm)
        {
            if (bm == null)
            {
                return BadRequest("The payload must not be null.");
            }

            if (string.IsNullOrWhiteSpace(bm.Token))
            {
                return BadRequest("The token is required.");
            }

            if (string.IsNullOrWhiteSpace(bm.Password))
            {
                return BadRequest("A password is required.");
            }

            if (string.IsNullOrWhiteSpace(bm.ConfirmPassword))
            {
                return BadRequest("A password confirmation is required.");
            }

            if (bm.Password != bm.ConfirmPassword)
            {
                return BadRequest("The passwords must match.");
            }
            
            return Ok();
        }
    }
}