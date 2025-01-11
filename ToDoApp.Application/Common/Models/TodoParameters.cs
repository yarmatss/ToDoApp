namespace ToDoApp.Application.Common.Models;

public class TodoParameters
{
    public Guid? CategoryId { get; set; }
    public string? SearchPhrase { get; set; }
    public DateTime? DeadlineFrom { get; set; }
    public DateTime? DeadlineTo { get; set; }
    public bool? IsCompleted { get; set; }
    public string SortBy { get; set; } = "Deadline"; // CreatedAt lub Deadline
    public bool SortDescending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}