﻿namespace MovieAPI.Model
{
    public class ApplicationUser: IdentityUser
    {
        [Required,MaxLength(50)]    
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
    
}
