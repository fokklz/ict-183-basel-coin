using System.ComponentModel.DataAnnotations;

namespace BaselCoin2.DTOs.Requests
{
    public class UserEditRequest
    {
        public string Id { get; set; } = default!;

        [StringLength(50, ErrorMessage = "Username must be between 3 and 50 characters", MinimumLength = 3)]
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
