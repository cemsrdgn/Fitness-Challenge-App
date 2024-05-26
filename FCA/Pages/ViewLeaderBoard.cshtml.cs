using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FCA.Data;
using Microsoft.AspNetCore.Authorization;

namespace FCA.Pages
{
     [Authorize]
    public class ViewLeaderBoardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ViewLeaderBoardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Challenges> Challenges { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Category { get; set; }

        public async Task<IActionResult> OnGetAsync(string category)
        {
            IQueryable<Challenges> query = _context.Challenges;

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(c => c.Category == category);
            }

            var challenges = await query.ToListAsync();

            Challenges = challenges
                .Where(c => !string.IsNullOrEmpty(c.Period) && int.TryParse(c.Period, out _))
                .OrderByDescending(c => int.Parse(c.Period))
                .ToList();

            return Page();
        }
    }
}