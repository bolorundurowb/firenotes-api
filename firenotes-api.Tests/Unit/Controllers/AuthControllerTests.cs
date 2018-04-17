using System;
using System.Threading.Tasks;
using AutoMapper;
using firenotes_api.Configuration;
using firenotes_api.Controllers;
using firenotes_api.Interfaces;
using firenotes_api.Models.Binding;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace firenotes_api.Tests.Unit.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        #region ForgotPassword

        [Test]
        public async Task ForgotPassword_BadRequest_WhenPayload_IsNull()
        {
            var userService = new Mock<IUserService>();
            var emailService = new Mock<IEmailService>();
            var mapperService = new Mock<IMapper>();
            var loggerService = new Mock<ILogger<AuthController>>();

            var controller = new AuthController(
                mapperService.Object,
                loggerService.Object,
                emailService.Object,
                userService.Object
            );

            var result = await controller.ForgotPassword(null);
            var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var message = badResult.Value.Should().BeAssignableTo<string>().Subject;
            message.Should().Be("The payload must not be null.");
        }

        [Test]
        public async Task ForgotPassword_BadRequest_WhenEmail_IsNull()
        {
            var userService = new Mock<IUserService>();
            var emailService = new Mock<IEmailService>();
            var mapperService = new Mock<IMapper>();
            var loggerService = new Mock<ILogger<AuthController>>();

            var controller = new AuthController(
                mapperService.Object,
                loggerService.Object,
                emailService.Object,
                userService.Object
            );

            var result = await controller.ForgotPassword(new ForgotPasswordBindingModel
            {
                Email = ""
            });
            var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var message = badResult.Value.Should().BeAssignableTo<string>().Subject;
            message.Should().Be("An email address is required.");
        }

        #endregion

        #region ResetPassword

        [Test]
        public async Task ResetPassword_BadRequest_WhenPayload_IsNull()
        {
            var userService = new Mock<IUserService>();
            var emailService = new Mock<IEmailService>();
            var mapperService = new Mock<IMapper>();
            var loggerService = new Mock<ILogger<AuthController>>();

            var controller = new AuthController(
                mapperService.Object,
                loggerService.Object,
                emailService.Object,
                userService.Object
            );

            var result = await controller.ResetPassword(null);
            var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var message = badResult.Value.Should().BeAssignableTo<string>().Subject;
            message.Should().Be("The payload must not be null.");
        }

        [Test]
        public async Task ResetPassword_BadRequest_WhenToken_IsEmpty()
        {
            var userService = new Mock<IUserService>();
            var emailService = new Mock<IEmailService>();
            var mapperService = new Mock<IMapper>();
            var loggerService = new Mock<ILogger<AuthController>>();

            var controller = new AuthController(
                mapperService.Object,
                loggerService.Object,
                emailService.Object,
                userService.Object
            );

            var result = await controller.ResetPassword(new ResetPasswordBindingModel
            {
                Token = ""
            });
            var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var message = badResult.Value.Should().BeAssignableTo<string>().Subject;
            message.Should().Be("The token is required.");
        }

        [Test]
        public async Task ResetPassword_BadRequest_WhenPassword_IsEmpty()
        {
            var userService = new Mock<IUserService>();
            var emailService = new Mock<IEmailService>();
            var mapperService = new Mock<IMapper>();
            var loggerService = new Mock<ILogger<AuthController>>();

            var controller = new AuthController(
                mapperService.Object,
                loggerService.Object,
                emailService.Object,
                userService.Object
            );

            var result = await controller.ResetPassword(new ResetPasswordBindingModel
            {
                Token = "xxxx",
                Password = ""
            });
            var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var message = badResult.Value.Should().BeAssignableTo<string>().Subject;
            message.Should().Be("A password is required.");
        }

        [Test]
        public async Task ResetPassword_BadRequest_WhenConfirmPassword_IsEmpty()
        {
            var userService = new Mock<IUserService>();
            var emailService = new Mock<IEmailService>();
            var mapperService = new Mock<IMapper>();
            var loggerService = new Mock<ILogger<AuthController>>();

            var controller = new AuthController(
                mapperService.Object,
                loggerService.Object,
                emailService.Object,
                userService.Object
            );

            var result = await controller.ResetPassword(new ResetPasswordBindingModel
            {
                Token = "xxxx",
                Password = "xxxx",
                ConfirmPassword = ""
            });
            var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var message = badResult.Value.Should().BeAssignableTo<string>().Subject;
            message.Should().Be("A password confirmation is required.");
        }

        [Test]
        public async Task ResetPassword_BadRequest_WhenConfirmPassword_And_Password_DontMatch()
        {
            var userService = new Mock<IUserService>();
            var emailService = new Mock<IEmailService>();
            var mapperService = new Mock<IMapper>();
            var loggerService = new Mock<ILogger<AuthController>>();

            var controller = new AuthController(
                mapperService.Object,
                loggerService.Object,
                emailService.Object,
                userService.Object
            );

            var result = await controller.ResetPassword(new ResetPasswordBindingModel
            {
                Token = "xxxx",
                Password = "xxxx",
                ConfirmPassword = "xxxxx"
            });
            var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var message = badResult.Value.Should().BeAssignableTo<string>().Subject;
            message.Should().Be("The passwords must match.");
        }

        [Test, Ignore("Hid the implementation")]
        public async Task ResetPassword_BadRequest_When()
        {
            Environment.SetEnvironmentVariable("SECRET", "xxxx");
            var token = string.Empty;

            var userService = new Mock<IUserService>();
            var emailService = new Mock<IEmailService>();
            var mapperService = new Mock<IMapper>();
            var loggerService = new Mock<ILogger<AuthController>>();

            var controller = new AuthController(
                mapperService.Object,
                loggerService.Object,
                emailService.Object,
                userService.Object
            );

            var result = await controller.ResetPassword(new ResetPasswordBindingModel
            {
                Token = token,
                Password = "xxxx",
                ConfirmPassword = "xxxx"
            });
            var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var message = badResult.Value.Should().BeAssignableTo<string>().Subject;
            message.Should().Be("The email is invalid.");
        }

        #endregion
    }
}