using FCA.Data;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileModel> _logger;

        public ProfileModel(ApplicationDbContext context, ILogger<ProfileModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Challenges> FavoriteChallenges { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Kullanıcı kimliğini al
            FavoriteChallenges = await _context.Challenges
                .Where(c => c.IsFavorite && _context.Reviews.Any(r => r.ChallengeId == c.Id && r.UserId == userId))
                .ToListAsync();
        }
    }
}
