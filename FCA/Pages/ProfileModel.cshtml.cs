using FCA.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileModel> _logger;

        public ProfileModel(ApplicationDbContext context, ILogger<ProfileModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Challenges> FavoriteChallenges { get; set; }

        // Geçici kısa biyografi
        private static string TempBio = "This is a short bio about the user. It gives a brief introduction about their fitness journey and goals.";

        [BindProperty]
        public string ShortBio { get; set; }

        public void OnGet()
        {
            ShortBio = TempBio;

            // Favori zorlukları database'den çek
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            FavoriteChallenges = _context.Challenges
                .Where(c => c.IsFavorite)
                .ToList();
        }

        public IActionResult OnPostUpdateBio()
        {
            TempBio = ShortBio;
            return RedirectToPage();
        }
    }
}
