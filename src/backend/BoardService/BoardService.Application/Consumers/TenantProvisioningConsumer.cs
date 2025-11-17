using BoardService.Infrastructure;
using BoardService.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TenantProvisioning.Messages;

namespace BoardService.Application.Consumers;

public class TenantProvisioningConsumer(ILogger<TenantProvisioningConsumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<TenantDatabaseCreationRequested>
{
    public async Task Consume(ConsumeContext<TenantDatabaseCreationRequested> context)
    {
        logger.LogInformation("Event received to create tenant database for TenantId: {TenantId}, ServiceName: {ServiceName}", context.Message.TenantId, context.Message.ServiceName);
        
        if (context.Message.ServiceName != "BoardService")
        {
            return;
        }
        
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(context.Message.DbConnectionString);

        var tenantContext = new TenantContext()
        {
            TenantId = context.Message.TenantId,
            ServiceName = context.Message.ServiceName,
            DbConnectionString = context.Message.DbConnectionString,
        };

        await using var db = new ApplicationDbContext(optionsBuilder.Options, tenantContext);

        logger.LogInformation("Applying migrations for service {ServiceName} and tenant {TenantId}", context.Message.ServiceName, context.Message.TenantId);
        
        await db.Database.MigrateAsync();

        logger.LogInformation("Migrations applied for service {ServiceName} and tenant {TenantId}", context.Message.ServiceName, context.Message.TenantId);
        
        await publishEndpoint.Publish(new TenantDatabaseCreated()
        {
            TenantId = context.Message.TenantId,
            ServiceName = context.Message.ServiceName
        });
    }
}