using JwtDemoApp.Application.Services;
using JwtDemoApp.Domain;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JwtDemoApp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;

        private readonly IDictionary<string, ApplicationUser> _users = new Dictionary<string, ApplicationUser>
        {
            { "test1", new ApplicationUser(){ Id = 1, UserName="test1", Password= SecurePasswordHasher.Hash("password1") } },
            { "test2", new ApplicationUser(){ Id = 2, UserName="test2", Password= SecurePasswordHasher.Hash("password2") } },
            { "admin", new ApplicationUser(){ Id = 3, UserName="test2", Password= SecurePasswordHasher.Hash("securePassword") }  }
        };


        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        public bool IsValidUserCredentials(string userName, string password)
        {
            _logger.LogInformation($"Validating user [{userName}]");
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            return _users.TryGetValue(userName, out var p) && SecurePasswordHasher.Verify(password, p.Password);
        }

        public bool IsAnExistingUser(string userName)
        {
            return _users.ContainsKey(userName);
        }

        public string GetUserRole(string userName)
        {
            if (!IsAnExistingUser(userName))
            {
                return string.Empty;
            }

            if (userName == "admin")
            {
                return UserRoles.Admin;
            }

            return UserRoles.BasicUser;
        }
    }

    public static class UserRoles
    {
        public const string Admin = nameof(Admin);
        public const string BasicUser = nameof(BasicUser);
    }
}
