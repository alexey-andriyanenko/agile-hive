namespace TaskService.Application.Results;

public class PagedResult
{
    public int TotalCount { get; set; }
    
    public int PageSize { get; set; }
    
    public int CurrentPage { get; set; }
}