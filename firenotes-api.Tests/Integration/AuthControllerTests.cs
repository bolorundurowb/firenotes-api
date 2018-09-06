using System.Net;
using System.Threading.Tasks;
using AspNetCore.Http.Extensions;
using firenotes_api.Configuration;
using firenotes_api.Models.Binding;
using firenotes_api.Models.Data;
using firenotes_api.Models.View;
using FluentAssertions;
using NUnit.Framework;

namespace firenotes_api.Tests.Integration
{
    [TestFixture]
    public class AuthControllerTests : BaseApiControllerTests
    {
        #region SignUp

        [Test]
        public async Task BadReqestIfTheSignUpEmailIsNull()
        {
            var payload = new LoginBindingModel();
            var response = await Client.PostAsJsonAsync("/api/auth/signup", payload);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("An email address is required.");
        }

        [Test]
        public async Task BadReqestIfTheSignUpPasswordIsNull()
        {
            var payload = new LoginBindingModel {Email = "name@email.com", Password = ""};
            var response = await Client.PostAsJsonAsync("/api/auth/signup", payload);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A password is required.");
        }

        [Test]
        public async Task BadReqestIfTheSignUpPasswordIsLessThanEightCharacters()
        {
            var payload = new LoginBindingModel {Email = "name@email.com", Password = "12345"};
            var response = await Client.PostAsJsonAsync("/api/auth/signup", payload);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The password cannot be less than 8 characters.");
        }

        [Test, Order(100)]
        public async Task SuccessfulSignUp()
        {
            var payload = new LoginBindingModel {Email = "name@email.com", Password = "12345678"};
            var response = await Client.PostAsJsonAsync("/api/auth/signup", payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadAsJsonAsync<AuthViewModel>();

            user.Id.Should().NotBeNullOrWhiteSpace();
            user.Token.Should().NotBeNullOrWhiteSpace();
            user.Email.Should().Be("name@email.com");
            user.FirstName.Should().BeNullOrEmpty();
            user.LastName.Should().BeNullOrEmpty();
        }

        [Test, Order(101)]
        public async Task ConflictIfAUserWithTheEmailExists()
        {
            var payload = new LoginBindingModel {Email = "name@email.com", Password = "123456789"};
            var response = await Client.PostAsJsonAsync("/api/auth/signup", payload);
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Sorry, a user with that email already exists.");
        }

        #endregion

        #region Login

        [Test]
        public async Task BadReqestIfTheEmailIsNull()
        {
            var payload = new LoginBindingModel {Email = " "};
            var response = await Client.PostAsJsonAsync("/api/auth/login", payload);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("An email address is required.");
        }

        [Test]
        public async Task BadReqestIfThePasswordIsNull()
        {
            var payload = new LoginBindingModel {Email = "name@email.com", Password = " "};
            var response = await Client.PostAsJsonAsync("/api/auth/login", payload);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A password is required.");
        }

        [Test]
        public async Task NotFoundIfTheUserDoesNotExist()
        {
            var payload = new LoginBindingModel {Email = "names@email.com", Password = "password"};
            var response = await Client.PostAsJsonAsync("/api/auth/login", payload);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A user with that email address doesn't exist.");
        }

        [Test, Order(102)]
        public async Task SuccessfulLoginWithExistingUser()
        {
            var payload = new LoginBindingModel {Email = "name@email.com", Password = "12345678"};
            var response = await Client.PostAsJsonAsync("/api/auth/login", payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var user = await response.Content.ReadAsJsonAsync<AuthViewModel>();

            user.Id.Should().NotBeNullOrWhiteSpace();
            user.Token.Should().NotBeNullOrWhiteSpace();
            user.Email.Should().Be("name@email.com");
            user.FirstName.Should().BeNullOrEmpty();
            user.LastName.Should().BeNullOrEmpty();
        }

        #endregion

        #region ForgotPassword

        [Test]
        public async Task ForgotPassword_Should_ReturnnNotFound_When_TheEmailIsNonExistent()
        {
            var payload = new LoginBindingModel {Email = "non-existent@email.com"};
            var response = await Client.PostAsJsonAsync("/api/auth/forgot-password", payload);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A user with that email address doesn't exist.");
        }

        [Test, Ignore("The email sending is causing this to fail")]
        public async Task ForgotPassword_Should_ReturnnOk_When_TheEmailExists()
        {
            var payload = new LoginBindingModel {Email = "name@email.com"};
            var response = await Client.PostAsJsonAsync("/api/auth/forgot-password", payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Your password reset email has been sent.");
        }

        #endregion

        #region ResetPassword

        [Test]
        public async Task ResetPassword_Should_ReturnBadRequest_When_TheDecodedEmailDoesNotExist()
        {
            var token = Helpers.GetToken(new User() {Email = "unknown@email.com"}, 12, TokenType.Reset);
            var payload = new ResetPasswordBindingModel {Token = token, Password = "xxxx", ConfirmPassword = "xxxx"};
            var response = await Client.PostAsJsonAsync("/api/auth/reset-password", payload);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task ResetPassword_Should_ReturnOk_When_TheDataIsComplete()
        {
            var token = Helpers.GetToken(new User() {Email = "name@email.com"}, 12, TokenType.Reset);
            var payload =
                new ResetPasswordBindingModel {Token = token, Password = "12345678", ConfirmPassword = "12345678"};
            var response = await Client.PostAsJsonAsync("/api/auth/reset-password", payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("{\"message\":\"The password has been updated.\"}");
        }

        #endregion
    }
}