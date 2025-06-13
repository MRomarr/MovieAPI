namespace MovieAPI.Model
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expiration;
        public bool IsActive => !IsExpired && RevokedOn is null;
    }
}
