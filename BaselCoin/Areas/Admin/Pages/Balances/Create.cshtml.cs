using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BaselCoin2.Data;
using BaselCoin2.Models;
using System.Diagnostics;
using BaselCoin2.DTOs.Requests;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaselCoin2.Areas.Admin.Pages.Balances
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly BaselCoin2.Data.ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        private void UpdateViewDataUserId()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
        }

        public CreateModel(BaselCoin2.Data.ApplicationDBContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            UpdateViewDataUserId();
            return Page();
        }

        [BindProperty]
        public BalanceCreateRequest Balance { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var newBalance = _mapper.Map<Balance>(Balance);
                if (! await DoesBalanceForUserExist(newBalance.UserId))
                {

                    _context.Balances.Add(newBalance);

                    var tryGetUser = await _userManager.GetUserAsync(User);
                    if (tryGetUser != null)
                    {
                        try
                        {
                            await _context.SaveChangesAsync(tryGetUser.Id);
                        }
                        catch (DbUpdateException ex)
                        {
                            ModelState.AddModelError("", "Unable to save changes. " +
                                                           "Try again, and if the problem persists, " +
                                                                                      "see your system administrator.");

                            UpdateViewDataUserId();

                            return Page();
                        }
                    }

                    return RedirectToPage("./Index");
                }
                else { 
                        ModelState.AddModelError("", "A balance for this user already exists.");
                               }
            }


            UpdateViewDataUserId();

            return Page();
        }

        public async Task<bool> DoesBalanceForUserExist(string userId)
        {
            return await _context.Balances.AnyAsync(b => b.UserId == userId);
        }
    }
}
