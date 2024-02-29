using System.ComponentModel.DataAnnotations;

namespace BaselCoin2.DTOs.Requests
{
    public class UserCreateRequest
    {
        [Required(ErrorMessage = "UserName is required")]
        [StringLength(50, ErrorMessage = "Username must be between 3 and 50 characters", MinimumLength = 3)]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = default!;
    }
}
