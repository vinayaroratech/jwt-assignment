using System.Text.Json.Serialization;

namespace JwtDemoApp.Models
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}