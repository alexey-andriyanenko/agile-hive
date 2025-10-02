namespace Web.API.Parameters.Board;

public class CreateBoardParameters
{
    public Guid TenantId { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public required string Name { get; set; }
    
    public Guid BoardTypeId { get; set; }
    
    public IReadOnlyList<CreateBoardColumnItemRequest> Columns { get; set; } = [];
}

public class CreateBoardColumnItemRequest
{
    public required string Name { get; set; }
}
