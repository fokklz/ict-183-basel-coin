using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaselCoin2.Data;
using BaselCoin2.Models;
using BaselCoin2.DTOs.Requests;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BaselCoin2.Areas.Admin.Pages.Balances
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly BaselCoin2.Data.ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(BaselCoin2.Data.ApplicationDBContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [BindProperty]
        public BalanceEditRequest Balance { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var balance =  await _context.Balances.FirstOrDefaultAsync(m => m.Id == id);
            if (balance == null)
            {
                return NotFound();
            }
            Balance = _mapper.Map<BalanceEditRequest>(balance);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var balanceToUpdate = await _context.Balances.FirstOrDefaultAsync(b => b.Id == Balance.Id);
                if (balanceToUpdate == null)
                {
                    return NotFound();
                }
                _mapper.Map(Balance, balanceToUpdate);

                var tryGetUser = await _userManager.GetUserAsync(User);
                if (tryGetUser != null)
                {
                    await _context.SaveChangesAsync(tryGetUser.Id);
                }

                return RedirectToPage("./Index");
            }

            return Page();
        }

        private bool BalanceExists(int id)
        {
            return _context.Balances.Any(e => e.Id == id);
        }
    }
}
