using Library.DTO.Books;
using Library.DTO.Suggestions;

namespace Library.Services.Suggestions
{
    public interface ISuggestionsService
    {
        Task<IReadOnlyList<SuggestionListItem>> SuggestionsAsync(int top = 10, CancellationToken ct = default);

    }    
}

