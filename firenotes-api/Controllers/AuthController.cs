﻿using System;
using System.Linq;
using System.Net;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using firenotes_api.Models.View;
using firenotes_api.Models.Data;
using Microsoft.AspNetCore.Http;
using firenotes_api.Configuration;
using firenotes_api.Interfaces;
using firenotes_api.Models.Binding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace firenotes_api.Controllers
{
    [Route("api/[controller]"), AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AuthController(IMapper mapper, ILogger<AuthController> logger, IEmailService emailService,
            IUserService userService)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _emailService = emailService;
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

            var user = await _userService.GetUserByEmail(data.Email);

            if (user == null)
            {
                return NotFound("A user with that email address doesn't exist.");
            }

            if (user.IsArchived)
            {
                return BadRequest("This user's account has been archived. Please reach out to the administrator.");
            }

            if (!BCrypt.Net.BCrypt.Verify(data.Password, user.HashedPassword))
            {
                return Unauthorized();
            }

            var token = Helpers.GetToken(user, 48);
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


            var user = await _userService.GetUserByEmail(data.Email);

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
            await _emailService.SendAsync(data.Email, "Welcome", email);
            _logger.LogInformation("Welcome email sent uccessfully.");

            user = new User
            {
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                Password = data.Password
            };

            await _userService.Add(user);
            var result = _mapper.Map<AuthViewModel>(user);
            
            var token = Helpers.GetToken(user, 48);
            result.Token = token;
            
            return Ok(result);
        }

        // POST api/auth/forgot-password
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

            var user = await _userService.GetUserByEmail(bm.Email);

            if (user == null)
            {
                return NotFound("A user with that email address doesn't exist.");
            }

            var token = Helpers.GetToken(user, 12, TokenType.Reset);
            
            var email = EmailTemplates.GetForgotPasswordEmail(
                $"{Config.FrontEndUrl}/auth/reset-password?token={token}");
            var response = await _emailService.SendAsync(bm.Email, "Forgot Password", email);

            if (response.IsSuccessful)
            {
                _logger.LogInformation("Forgot password email sent successfully.");
                return Ok(new GenericViewModel
                {
                    Message = "Your password reset email has been sent."
                });
            }

            _logger.LogError("The email was not sent successfully.");
            _logger.LogError(response.ErrorException, response.ErrorMessage);
            return StatusCode((int) HttpStatusCode.InternalServerError, new GenericViewModel
            {
                Message = "An error occurred when sending the recovery email."
            });
        }

        // POST api/auth/reset-password
        [Route("reset-password"), HttpPost]
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

            try
            {
                var emailAddress = Helpers.GetResetTokenUserData(bm.Token);

                if (string.IsNullOrWhiteSpace(emailAddress))
                {
                    return BadRequest("The email is invalid.");
                }

                var user = await _userService.GetUserByEmail(emailAddress);

                if (user == null)
                {
                    return NotFound();
                }

                await _userService.SetPassword(emailAddress, bm.Password);
                
                // send notification email
                var email = EmailTemplates.GetResetPasswordEmail(user.FirstName);
                var response = await _emailService.SendAsync(user.Email, "Password Reset", email);

                if (response.IsSuccessful)
                {
                    _logger.LogInformation("Forgot password email sent successfully.");
                }
                else
                {
                    _logger.LogError("Forgot password email was not sent successfully.");
                    _logger.LogError(response.ErrorException, response.ErrorMessage);
                }

                return Ok(new GenericViewModel
                {
                    Message = "The password has been updated."
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred when decoding the reset token");

                return StatusCode((int) HttpStatusCode.InternalServerError, new GenericViewModel
                {
                    Message = e.Message
                });
            }
        }
    }
}