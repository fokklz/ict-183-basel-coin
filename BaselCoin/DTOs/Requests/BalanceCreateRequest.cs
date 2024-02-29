using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BaselCoin2.DTOs.Requests
{
    public class BalanceCreateRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }

        [Precision(18, 5)]
        [Required(ErrorMessage = "Amount is required")]
        [Range(0, 9999999999999999.99999, ErrorMessage = "Amount must be greater than 0 when creating a new balance, we like our customers!")]
        public decimal Amount { get; set; }
    }
}
