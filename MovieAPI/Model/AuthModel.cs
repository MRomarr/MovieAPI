using System.Text.Json.Serialization;

namespace MovieAPI.Model
{
    public class AuthModel
    {
        public string? Massage { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? Username { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }
        public List<string> Roles { get; set; }
        public string? Token { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }   
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
