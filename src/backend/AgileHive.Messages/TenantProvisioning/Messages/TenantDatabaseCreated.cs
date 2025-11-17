namespace TenantProvisioning.Messages;

public class TenantDatabaseCreated
{
    public required Guid TenantId { get; set; }

    public required string ServiceName { get; set; }
}