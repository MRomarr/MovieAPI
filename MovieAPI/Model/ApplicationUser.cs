namespace MovieAPI.Model
{
    public class ApplicationUser: IdentityUser
    {
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
    
}
