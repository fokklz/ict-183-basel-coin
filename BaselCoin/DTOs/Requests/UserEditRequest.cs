using System.ComponentModel.DataAnnotations;

namespace BaselCoin2.DTOs.Requests
{
    public class UserEditRequest
    {
        public string Id { get; set; } = default!;

        [StringLength(50, ErrorMessage = "Username must be between 3 and 50 characters", MinimumLength = 3)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "The username can only contain letters and numbers")]
        public string UserName { get; set; } = default!;

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = default!;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
