using FCA.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FCA.Pages;

public class CreateChallengesModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateChallengesModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Challenges? ChallengeResult { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            _context.Challenges.Add(ChallengeResult);
            await _context.SaveChangesAsync();
            Console.WriteLine($"{ChallengeResult.Category} {ChallengeResult.Period} {ChallengeResult.DifLevel} has been added successfully!");
            return RedirectToPage("./ListChallenges", new { success = true });
        }
        else
        {
            Console.WriteLine("Challenge cannot be added to DB!");
            return RedirectToPage("./Index", new { success = false });
        }
    }

    // Filtreleme işlemleri için yeni metot
    public IActionResult OnGetFilteredChallenges(string? keywords, string? category, int? period, string? difLevel)
    {
        var query = _context.Challenges.AsQueryable();

        if (!string.IsNullOrEmpty(keywords))
        {
            // Anahtar kelimelerle ilgili arama
            query = query.Where(c =>
                c.Category.Contains(keywords) ||
                c.Period.Contains(keywords) ||
                c.DifLevel.Contains(keywords));
        }

        if (!string.IsNullOrEmpty(category))
        {
            // Kategoriye göre filtreleme
            query = query.Where(c => c.Category.Contains(category));
        }

        if (period.HasValue)
        {
            // Döneme göre filtreleme
            query = query.Where(c => c.Period == period.Value.ToString());
        }

        if (!string.IsNullOrEmpty(difLevel))
        {
            // Zorluk seviyesine göre filtreleme
            query = query.Where(c => c.DifLevel.Contains(difLevel));
        }

        var filteredChallenges = query.ToList();
        return new JsonResult(filteredChallenges);
    }
}
