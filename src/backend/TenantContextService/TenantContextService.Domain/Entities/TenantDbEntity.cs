namespace TenantContextService.Domain.Entities;

public class TenantDbEntity
{
    public Guid Id { get; set; }
    
    public Guid TenantId { get; set; }
    
    public required string ServiceName { get; set; }
    
    public required string ConnectionString { get; set; }
    
    public bool Active { get; set; }
}