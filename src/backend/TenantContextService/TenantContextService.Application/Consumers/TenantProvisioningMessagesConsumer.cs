using MassTransit;
using Microsoft.EntityFrameworkCore;
using TenantContextService.Infrastructure.Data;
using TenantProvisioning.Messages;

namespace TenantContextService.Application.Consumers;

public class TenantProvisioningMessagesConsumer(ApplicationDbContext dbContext) : IConsumer<TenantDatabaseCreated>
{
    public async Task Consume(ConsumeContext<TenantDatabaseCreated> context)
    {
        var tenantDb = await dbContext.TenantDbs
            .FirstOrDefaultAsync(x => x.TenantId == context.Message.TenantId && x.ServiceName == context.Message.ServiceName);

        if (tenantDb == null)
        {
            throw new InvalidOperationException($"Tenant DB record not found for TenantId: {context.Message.TenantId}, ServiceName: {context.Message.ServiceName}");
        }
        
        tenantDb.Active = true;
        
        await dbContext.SaveChangesAsync();
    }
}