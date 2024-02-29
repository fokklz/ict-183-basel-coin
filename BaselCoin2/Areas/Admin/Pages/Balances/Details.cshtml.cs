using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BaselCoin2.Data;
using BaselCoin2.Models;
using Microsoft.AspNetCore.Authorization;

namespace BaselCoin2.Areas.Admin.Pages.Balances
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly BaselCoin2.Data.ApplicationDBContext _context;

        public DetailsModel(BaselCoin2.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public Balance Balance { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var balance = await _context.Balances.FirstOrDefaultAsync(m => m.Id == id);
            if (balance == null)
            {
                return NotFound();
            }
            else
            {
                Balance = balance;
            }
            return Page();
        }
    }
}
