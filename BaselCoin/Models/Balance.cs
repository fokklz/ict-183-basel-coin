using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BaselCoin2.Models
{
    public class Balance
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Precision(18, 5)]
        public decimal Amount { get; set; }
    }
}
