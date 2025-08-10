namespace Library.DTO.Error;

public sealed class ErrorResponse
{
    public string TraceId { get; set; } = default!;
    public int Status { get; set; }
    public string Code { get; set; } = "error";
    public string Title { get; set; } = "Error";
    public string? Detail { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; } 
}