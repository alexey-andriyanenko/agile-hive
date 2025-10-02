namespace Web.API.Parameters.Board;

public class UpdateBoardParameters
{
    public Guid TenantId { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public Guid BoardId { get; set; }
    
    public required string Name { get; set; }
    
    public IReadOnlyList<CreateOrUpdateBoardColumnItemRequest> Columns { get; set; } = [];
}

public class CreateOrUpdateBoardColumnItemRequest
{
    public Guid? Id { get; set; }
    
    public required string Name { get; set; }
    
    public int Order { get; set; }
}
