using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Common.Models;

public class TodoParameters
{
    public Guid? CategoryId { get; set; }
    public string? SearchPhrase { get; set; }
    public DateTime? DeadlineFrom { get; set; }
    public DateTime? DeadlineTo { get; set; }
    public bool? IsCompleted { get; set; }
    public Priority? Priority { get; set; }
    public string SortBy { get; set; } = "Deadline"; // CreatedAt lub Deadline
    public bool SortDescending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public override string ToString()
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(SearchPhrase))
            queryParams.Add($"searchPhrase={Uri.EscapeDataString(SearchPhrase)}");

        if (CategoryId.HasValue)
            queryParams.Add($"categoryId={CategoryId}");

        if (DeadlineFrom.HasValue)
            queryParams.Add($"deadlineFrom={DeadlineFrom.Value.ToString("o")}");

        if (DeadlineTo.HasValue)
            queryParams.Add($"deadlineTo={DeadlineTo.Value.ToString("o")}");

        if (IsCompleted.HasValue)
            queryParams.Add($"isCompleted={IsCompleted}");

        if (Priority.HasValue)
            queryParams.Add($"priority={Priority}");

        if (!string.IsNullOrEmpty(SortBy))
            queryParams.Add($"sortBy={SortBy}");

        queryParams.Add($"sortDescending={SortDescending}");
        queryParams.Add($"pageNumber={PageNumber}");
        queryParams.Add($"pageSize={PageSize}");

        return string.Join("&", queryParams);
    }
}
