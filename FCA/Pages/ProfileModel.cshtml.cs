using FCA.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FCA.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileModel> _logger;

        public ProfileModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context, ILogger<ProfileModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        public List<Challenges> FavoriteChallenges { get; set; }
        public IdentityUser CurrentUser { get; set; }

        // Geçici kısa biyografi
        private static string TempBio = "This is a short bio about the user. It gives a brief introduction about their fitness journey and goals.";

        [BindProperty]
        public string ShortBio { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CurrentUser = await _userManager.FindByIdAsync(userId);
            ShortBio = TempBio;
            Email = CurrentUser.Email;

            // Favori zorlukları database'den çek
            FavoriteChallenges = await _context.Challenges
                .Where(c => c.IsFavorite)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateBioAsync()
        {
            TempBio = ShortBio;
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateEmailAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, Email);
            var result = await _userManager.ChangeEmailAsync(user, Email, token);

            if (result.Succeeded)
            {
                user.UserName = Email; // UserName alanını güncelle
                await _userManager.UpdateAsync(user);

                // Oturumu kapat ve kullanıcıyı ana sayfaya yönlendir
                await _signInManager.SignOutAsync();
                return Redirect("~/");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdatePasswordAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, Password);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToPage();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
