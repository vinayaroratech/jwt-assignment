using System.Text.Json.Serialization;

namespace JwtDemoApp.Models
{
    public class ImpersonationRequest
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
}