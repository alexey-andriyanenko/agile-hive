namespace BoardService.Domain.Constants;

public static class PredefinedBoardTypes
{
    public static readonly Guid KanbanId = Guid.Parse("fcc9b678-fb47-4e2d-9a91-a9c878be01ce");
    public const string KanbanName = "Kanban";
    
    public static readonly Guid ScrumId = Guid.Parse("789395d8-1619-4a48-942c-60859f825d3f");
    public const string ScrumName = "Scrum";
    
    public static readonly Guid BacklogId = Guid.Parse("76797260-eb29-412d-b47c-374989ed77b5");
    public const string BacklogName = "Backlog";
    
    public static readonly Guid CustomId = Guid.Parse("7654469d-935c-4597-a4fb-888e54eab3a2");
    public const string CustomName = "Custom";
}