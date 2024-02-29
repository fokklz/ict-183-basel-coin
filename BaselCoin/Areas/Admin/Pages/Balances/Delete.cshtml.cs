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
using Microsoft.AspNetCore.Identity;

namespace BaselCoin2.Areas.Admin.Pages.Balances
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly BaselCoin2.Data.ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteModel(BaselCoin2.Data.ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var balance = await _context.Balances.FindAsync(id);
            if (balance != null)
            {
                Balance = balance;
                _context.Balances.Remove(Balance);

                var tryGetUser = await _userManager.GetUserAsync(User);
                if (tryGetUser != null)
                {
                    await _context.SaveChangesAsync(tryGetUser.Id);
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
