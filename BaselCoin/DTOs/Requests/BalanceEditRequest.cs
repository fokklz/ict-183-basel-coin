using Microsoft.EntityFrameworkCore;

namespace BaselCoin2.DTOs.Requests
{
    public class BalanceEditRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        [Precision(18, 5)]
        public decimal Amount { get; set; }
    }
}
