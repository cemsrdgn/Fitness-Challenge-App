using FCA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FCA.Pages
{
    public class ListChallenges : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IList<Challenges>? ChallengesList { get; set; }
        public Challenges? Challenges { get; set; } = new Challenges();

        public ListChallenges(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(string? category, string? period, string? difLevel,string sortBy)
        {
            ChallengesList = await _context.Challenges.ToListAsync() ?? new List<Challenges>();
            var query = _context.Challenges.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(c => c.Category.Contains(category));
            }

            if (!string.IsNullOrEmpty(period))
            {
                query = query.Where(c => c.Period.Contains(period));
            }

            if (!string.IsNullOrEmpty(difLevel))
            {
                query = query.Where(c => c.DifLevel.Contains(difLevel));
            }

            ChallengesList = await query.ToListAsync();
            query = _context.Challenges.AsQueryable();

            if (sortBy == "Category")
            {
                query = query.OrderBy(c => c.Category).ThenBy(c => c.DifLevel);
            }
            else if (sortBy == "Difficulty")
            {
                query = query.OrderBy(c => c.DifLevel).ThenBy(c => c.Category);
            }

            ChallengesList = await query.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostToggleFavorite(int id)
        {
            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge == null)
            {
                return NotFound();
            }

            challenge.IsFavorite = !challenge.IsFavorite;
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddReview(int challengeId, int rating, string comment)
        {
            var review = new Review
            {
                ChallengeId = challengeId,
                Rating = rating,
                Comment = comment,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) // Kullanıcı kimliği eklendi.
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetFilteredChallenges(string? category, string? period, string? difLevel)
        {
            var query = _context.Challenges.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(c => c.Category.Contains(category));
            }

            if (!string.IsNullOrEmpty(period))
            {
                query = query.Where(c => c.Period.Contains(period));
            }

            if (!string.IsNullOrEmpty(difLevel))
            {
                query = query.Where(c => c.DifLevel.Contains(difLevel));
            }

            ChallengesList = await query.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetSortChallengesAsync(string sortBy)
        {
            var query = _context.Challenges.AsQueryable();

            if (sortBy == "Category")
            {
                query = query.OrderBy(c => c.Category).ThenBy(c => c.DifLevel);
            }
            else if (sortBy == "Difficulty")
            {
                query = query.OrderBy(c => c.DifLevel).ThenBy(c => c.Category);
            }

            ChallengesList = await query.ToListAsync();
            return Page();
        }
    }

   
}
