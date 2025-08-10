namespace Library.DTO.Suggestions;

public class SuggestionListItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string Category { get; set; } = default!;
    public int PublicationYear { get; set; }
    public double Score { get; set; } 
}