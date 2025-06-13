
namespace MoviesAPI.DTOs
{
    public class RegisterDto
    {
        [MinLength(3, ErrorMessage = "UserName must be at least 3 characters long.")]
        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
