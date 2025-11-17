namespace TenantProvisioning.Messages;

public class TenantDatabaseCreationRequested
{
    public required Guid TenantId { get; set; }
    
    public required Guid CreatedByUserId { get; set; }
    
    public required string ServiceName { get; set; }
    
    public required string DbConnectionString { get; set; }
}
