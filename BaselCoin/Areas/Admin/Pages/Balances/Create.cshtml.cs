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

namespace BaselCoin2.Areas.Admin.Pages.Balances
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly BaselCoin2.Data.ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(BaselCoin2.Data.ApplicationDBContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            return Page();
        }

        [BindProperty]
        public BalanceCreateRequest Balance { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                _context.Balances.Add(_mapper.Map<Balance>(Balance));

                var tryGetUser = await _userManager.GetUserAsync(User);
                if (tryGetUser != null)
                {
                    await _context.SaveChangesAsync(tryGetUser.Id);
                }

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
