using JwtDemoApp.Application.JwtAuth;
using JwtDemoApp.Infrastructure.Services;
using JwtDemoApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JwtDemoApp.IntegrationTests
{
    [TestClass]
    public class ValuesControllerTests
    {
        private readonly TestHostFixture _testHostFixture = new TestHostFixture();
        private HttpClient _httpClient;
        private IServiceProvider _serviceProvider;

        [TestInitialize]
        public void SetUp()
        {
            _httpClient = _testHostFixture.Client;
            _serviceProvider = _testHostFixture.ServiceProvider;
        }

        [TestMethod]
        public async Task ShouldExpect401WhenNotLoggedIn()
        {
            var response = await _httpClient.GetAsync("api/values");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public async Task ShouldGetAllKeyValuePairsUsingSuccessLogin()
        {
            var credentials = new LoginRequest
            {
                UserName = "admin",
                Password = "securePassword"
            };
            var loginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);
            var response = await _httpClient.GetAsync("api/values");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("[\"value1\",\"value2\"]", result);
        }

        [TestMethod]
        public async Task ShouldReturn401ForInvalidToken()
        {
            const string invalidTokenString =                @"eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTYxNTgxMzk5MiwiaXNzIjoiaHR0cHM6Ly92aW5heWFyb3JhdGVjaC5jb20iLCJhdWQiOiJodHRwczovL3ZpbmF5YXJvcmF0ZWNoLmNvbSJ9.RZtaxBQlVXUkzhp20XmDkh1-qKMKdlBBAWhyPThRJRY";

            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            Assert.ThrowsException<SecurityTokenExpiredException>(() => jwtAuthManager.DecodeJwtToken(invalidTokenString));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, invalidTokenString);
            var response = await _httpClient.GetAsync("api/values");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task ShouldReturn401ForExpiredToken()
        {
            const string userName = "admin";
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, UserRoles.Admin)
            };
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var jwtTokenConfig = _serviceProvider.GetRequiredService<JwtTokenConfig>();

            // expired token
            var jwtResult = jwtAuthManager.GenerateTokens(userName, claims, DateTime.Now.AddMinutes(-jwtTokenConfig.AccessTokenExpiration - 1));
            var invalidTokenString = jwtResult.AccessToken;
            Assert.ThrowsException<SecurityTokenExpiredException>(() => jwtAuthManager.DecodeJwtToken(invalidTokenString));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, invalidTokenString);
            var response = await _httpClient.GetAsync("api/values");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

            // not expired
            jwtResult = jwtAuthManager.GenerateTokens(userName, claims, DateTime.Now.AddMinutes(-jwtTokenConfig.AccessTokenExpiration));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtResult.AccessToken);
            response = await _httpClient.GetAsync("api/values");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // not expired token 2
            jwtResult = jwtAuthManager.GenerateTokens(userName, claims, DateTime.Now.AddMinutes(-jwtTokenConfig.AccessTokenExpiration - 1).AddSeconds(1));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtResult.AccessToken);
            response = await _httpClient.GetAsync("api/values");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}