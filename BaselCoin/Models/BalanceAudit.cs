﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BaselCoin2.Models
{
    public class BalanceAudit
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public int? BalanceId { get; set; }

        [Precision(18, 5)]
        public decimal Amount { get; set; }

        [Precision(18, 5)]
        public decimal BalanceBefore { get; set; }

        [Precision(18, 5)]
        public decimal BalanceAfter { get; set; }
        public string Owner { get; set; }
        public string Action { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
