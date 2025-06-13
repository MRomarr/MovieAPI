namespace MovieAPI.Model
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Expairation { get; set; }
        public bool IsExpired =>  DateTime.UtcNow > Expairation;
        public DateTime CreatedOn { get; set; }
        public DateTime RevokedOn { get; set; }
        public bool IsActive => !IsExpired && RevokedOn == null;


    }
}
