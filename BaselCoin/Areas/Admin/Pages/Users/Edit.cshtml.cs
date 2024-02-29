using AutoMapper;
using BaselCoin2.DTOs.Requests;
using BaselCoin2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaselCoin2.Areas.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContext, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContext;
            _mapper = mapper;
        }

        [BindProperty]
        public UserEditRequest User { get; set; } = default!;

        public SelectList Roles { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                User = _mapper.Map<UserEditRequest>(user);
            }

            var roles = _roleManager.Roles.ToList();
            Roles = new SelectList(roles, "Name", "Name");

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(User.Id);
            if (user != null) {
                _mapper.Map(User, user);
                await _userManager.UpdateAsync(user);

                if (!string.IsNullOrEmpty(User.Role))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, roles);
                    await _userManager.AddToRoleAsync(user, User.Role);
                }

                if (!string.IsNullOrEmpty(User.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, User.Password);
                }
            }

            return RedirectToPage("./Index");
        }
    }

}
