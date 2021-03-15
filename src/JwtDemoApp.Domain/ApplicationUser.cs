using System;

namespace JwtDemoApp.Domain
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
