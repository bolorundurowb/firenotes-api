﻿using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace firenotes_api.Tests.Integration
{
    public class AuthControllerTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public AuthControllerTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        #region SignUp

        [Fact]
        public async void BadReqestIfTheSignUpPayloadIsNull()
        {
            StringContent stringContent = new StringContent(
                "",
                UnicodeEncoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The payload must not be null.");
        }

        [Fact]
        public async void BadReqestIfTheSignUpEmailIsNull()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"  \" }",
                UnicodeEncoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("An email address is required.");
        }

        [Fact]
        public async void BadReqestIfTheSignUpPasswordIsNull()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"  \" }",
                UnicodeEncoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A password is required.");
        }

        [Fact]
        public async void BadReqestIfTheSignUpPasswordIsLessThanEightCharacters()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"12345\" }",
                UnicodeEncoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The password cannot be less than 8 characters.");
        }

        #endregion

        #region Login

        [Fact]
        public async void BadReqestIfThePayloadIsNull()
        {
            StringContent stringContent = new StringContent(
                "",
                UnicodeEncoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The payload must not be null.");
        }

        [Fact]
        public async void BadReqestIfTheEmailIsNull()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"  \" }",
                UnicodeEncoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("An email address is required.");
        }

        [Fact]
        public async void BadReqestIfThePasswordIsNull()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"  \" }",
                UnicodeEncoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A password is required.");
        }

        [Fact]
        public async void NotFoundIfTheUserDoesNotExist()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"password\" }",
                UnicodeEncoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A user with that email address doesn't exist.");
        }

         #endregion
    }
}