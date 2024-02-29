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
    public class IndexModel : PageModel
    {
        private readonly BaselCoin2.Data.ApplicationDBContext _context;

        public IndexModel(BaselCoin2.Data.ApplicationDBContext context)
        {
            _context = context;
        }

        public IList<Balance> Balance { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Balance = await _context.Balances
                .Include(b => b.User).ToListAsync();
        }
    }
}
