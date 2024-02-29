using BaselCoin2.Data;
using BaselCoin2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace BaselCoin2.Pages
{
    [Authorize]
    public class MemberModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public MemberModel(ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Debug.WriteLine("User: " + user);
            if (user != null)
            {
                var balance = _context.Balances.FirstOrDefault(b => b.UserId == user.Id);
                if (balance != null)
                {
                    Balance = balance;
                }
                else
                {
                    Balance = new Balance
                    {
                        UserId = user.Id,
                        Amount = 0
                    };

                    _context.Balances.Add(Balance);
                    await _context.SaveChangesAsync();
                }
            }
        }

        [BindProperty]
        public Balance Balance { get; set; } = default!;


    }
}
