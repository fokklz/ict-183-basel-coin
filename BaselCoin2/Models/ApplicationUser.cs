﻿using Microsoft.AspNetCore.Identity;

namespace BaselCoin2.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
