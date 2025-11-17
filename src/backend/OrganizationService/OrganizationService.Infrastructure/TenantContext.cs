namespace OrganizationService.Infrastructure;

public class TenantContext
{
    public required Guid TenantId { get; set; }
    
    public required string ServiceName { get; set; }
    
    public required string DbConnectionString { get; set; }
}