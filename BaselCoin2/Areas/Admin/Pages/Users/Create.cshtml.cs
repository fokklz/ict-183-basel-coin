using BaselCoin2.Data;
using BaselCoin2.DTOs.Requests;
using BaselCoin2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BaselCoin2.Areas.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDBContext _context;  

        public CreateModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDBContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult OnGet()
        {
            var roles = _roleManager.Roles;
            Roles = new SelectList(roles, "Name", "Name");

            return Page();
        }

        [BindProperty]
        public UserCreateRequest userToCreate { get; set; } = default!;

        public SelectList Roles { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userToCreate.UserName, Email = userToCreate.Email };
                await _userManager.CreateAsync(user, userToCreate.Password);
                if (userToCreate.Role != null && userToCreate.Role.ToUpper() != "USER")
                {
                    await _userManager.AddToRoleAsync(user, userToCreate.Role);
                }

                await _context.AddAsync(new Balance { UserId = user.Id, Amount = 1000 });
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
