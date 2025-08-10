using Library.Data;
using Library.DTO.Books;
using Library.DTO.Suggestions;
using Library.Services.Auth.Users; 
using Microsoft.EntityFrameworkCore;

namespace Library.Services.Suggestions
{
    public class SuggestionsService : ISuggestionsService
    {
        private readonly LibraryDbContext _db;
        private readonly ICurrentUserService _currentUser;

        
        private const double Alpha = 0.4;
        private const double Beta  = 0.3;
        private const double Gamma = 0.3;

        public SuggestionsService(LibraryDbContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<IReadOnlyList<SuggestionListItem>> SuggestionsAsync(int top = 10, CancellationToken ct = default)
        {
            var userId = _currentUser.GetCurrentUserId();

            var readBookIds = await _db.Loan
                .AsNoTracking()
                .Where(l => l.UserId == userId)
                .Select(l => l.BookId)
                .Distinct()
                .ToListAsync(ct);

            string? frequentAuthor = null;
            Guid? frequentCategoryId = null;
            int? avgYear = null;

            if (readBookIds.Count > 0)
            {
                frequentAuthor = await _db.Book
                    .AsNoTracking()
                    .Where(b => readBookIds.Contains(b.Id))
                    .GroupBy(b => b.Author)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefaultAsync(ct);

                frequentCategoryId = await _db.Book
                    .AsNoTracking()
                    .Where(b => readBookIds.Contains(b.Id))
                    .GroupBy(b => b.CategoryId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefaultAsync(ct);

                avgYear = await _db.Book
                    .AsNoTracking()
                    .Where(b => readBookIds.Contains(b.Id))
                    .AverageAsync(b => (double)b.PublicationYear, ct)
                    .ContinueWith(t => (int)Math.Round(t.Result), ct);
            }

            var popularityRaw = await _db.Loan
                .AsNoTracking()
                .GroupBy(l => l.BookId)
                .Select(g => new { BookId = g.Key, C = g.Count() })
                .ToListAsync(ct);

            var popDict = popularityRaw.ToDictionary(x => x.BookId, x => x.C);
            var maxLoans = popularityRaw.Count > 0 ? popularityRaw.Max(x => x.C) : 0;

            var candidates = await _db.Book
                .AsNoTracking()
                .Include(b => b.Category)
                .Where(b => !readBookIds.Contains(b.Id))
                .ToListAsync(ct);

            var ranked = candidates
                .Select(b => new
                {
                    Book = b,
                    Score = CalcScore(
                        book: b,
                        frequentAuthor,
                        frequentCategoryId,
                        avgYear,
                        maxLoans,
                        popDict.TryGetValue(b.Id, out var cnt) ? cnt : 0
                    )
                })
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Book.Title)
                .Take(top)
                .Select(x => new SuggestionListItem()
                {
                    Id = x.Book.Id,
                    Title = x.Book.Title,
                    Author = x.Book.Author,
                    Category = x.Book.Category.Name,
                    PublicationYear = x.Book.PublicationYear,
                    Score = Math.Round(x.Score, 4)
                })
                .ToList();

            return ranked;
        }

        private static double CalcScore(
            Book book,
            string? prefAuthor,
            Guid? prefCategoryId,
            int? avgYear,
            int maxLoansGlobal,
            int loansOfBook
        )
        {
            double simAuthor = (!string.IsNullOrWhiteSpace(prefAuthor) && book.Author == prefAuthor) ? 1.0 : 0.0;
            double simCategory = (prefCategoryId.HasValue && book.CategoryId == prefCategoryId.Value) ? 1.0 : 0.0;

            double simYear = 0.0;
            if (avgYear.HasValue)
            {
                var d = Math.Min(10, Math.Abs(book.PublicationYear - avgYear.Value));
                simYear = 1.0 - (d / 10.0); // [0,1]
            }

            double hasPrefs = (avgYear.HasValue || !string.IsNullOrWhiteSpace(prefAuthor) || prefCategoryId.HasValue) ? 1.0 : 0.0;
            double content = hasPrefs > 0 ? (simAuthor + simCategory + simYear) / 3.0 : 0.0;

            // --- Popularidad (P) ---
            double popularity = (maxLoansGlobal > 0) ? loansOfBook / (double)maxLoansGlobal : 0.0;

            // --- Historial (H) ---
            double history = content;

            double s = (Alpha * content) + (Beta * popularity) + (Gamma * history);
            if (s < 0) s = 0;
            if (s > 1) s = 1;
            return s;
        }
    }
}
