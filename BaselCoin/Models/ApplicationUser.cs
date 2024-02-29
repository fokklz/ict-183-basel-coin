using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BaselCoin2.Models
{
    public class ApplicationUser : IdentityUser
    {
        public override string UserName { get; set; }

        public string? FullName { get; set; }
    }
}
