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
        public IList<string>? AllCategories { get; set; }
        public IList<Review>? UserReviews { get; set; } // Kullanıcı yorumları ve puanları
        public Challenges? Challenges { get; set; } = new Challenges();
        [BindProperty]
        public Review TempReview { get; set; } = new Review(); // Geçici model
        [BindProperty]
        public string TempCategory { get; set; } // Geçici kategori

        public ListChallenges(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(string? category, string? period, string? difLevel, string? sortBy)
        {
            var query = _context.Challenges.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(c => c.Category.Contains(category) || c.OCat.Contains(category));
            }

            if (!string.IsNullOrEmpty(period))
            {
                query = query.Where(c => c.Period.Contains(period));
            }

            if (!string.IsNullOrEmpty(difLevel))
            {
                query = query.Where(c => c.DifLevel.Contains(difLevel));
            }

            if (sortBy == "Category")
            {
                query = query.OrderBy(c => c.Category).ThenBy(c => c.DifLevel);
            }
            else if (sortBy == "Difficulty")
            {
                query = query.OrderBy(c => c.DifLevel == "Easy" ? 1 :
                                           c.DifLevel == "Medium" ? 2 :
                                           c.DifLevel == "Hard" ? 3 : 4)
                             .ThenBy(c => c.Category);
            }

            ChallengesList = await query.ToListAsync();
            AllCategories = await _context.Challenges
                                          .Select(c => c.Category)
                                          .Distinct()
                                          .ToListAsync();

            // Bu kısım geçici kullanıcı verilerini yüklemek için örnek
            var tempUserId = "tempUser"; // Geçici kullanıcı kimliği
            UserReviews = await _context.Reviews
                                        .Where(r => r.UserId == tempUserId)
                                        .ToListAsync();

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
            return RedirectToPage(new { TempReview.Rating, TempReview.Comment, TempCategory });
        }

        public async Task<IActionResult> OnPostAddReview(int challengeId, int rating, string comment)
        {
            var tempUserId = "tempUser"; // Geçici kullanıcı kimliği

            var review = await _context.Reviews
                                       .FirstOrDefaultAsync(r => r.ChallengeId == challengeId && r.UserId == tempUserId);

            if (review == null)
            {
                review = new Review
                {
                    ChallengeId = challengeId,
                    UserId = tempUserId,
                    Rating = rating,
                    Comment = comment
                };
                _context.Reviews.Add(review);
            }
            else
            {
                review.Rating = rating;
                review.Comment = comment;
                _context.Reviews.Update(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { category = Request.Query["category"], period = Request.Query["period"], difLevel = Request.Query["difLevel"], sortBy = Request.Query["sortBy"], TempCategory });
        }

        public async Task<IActionResult> OnGetFilteredChallenges(string? category, string? period, string? difLevel)
        {
            var query = _context.Challenges.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(c => c.Category.Contains(category) || c.OCat.Contains(category));
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
            AllCategories = await _context.Challenges
                                          .Select(c => c.Category)
                                          .Distinct()
                                          .ToListAsync();

            // Bu kısım geçici kullanıcı verilerini yüklemek için örnek
            var tempUserId = "tempUser"; // Geçici kullanıcı kimliği
            UserReviews = await _context.Reviews
                                        .Where(r => r.UserId == tempUserId)
                                        .ToListAsync();

            return Page();
        }
    }
}