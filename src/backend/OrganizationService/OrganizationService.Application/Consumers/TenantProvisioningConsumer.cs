using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrganizationService.Domain.Entities;
using OrganizationService.Domain.Enums;
using OrganizationService.Infrastructure;
using OrganizationService.Infrastructure.Data;
using TenantProvisioning.Messages;

namespace OrganizationService.Application.Consumers;

public class TenantProvisioningConsumer(ILogger<TenantProvisioningConsumer> logger, IPublishEndpoint publishEndpoint)
    : IConsumer<TenantDatabaseCreationRequested>
{
    public async Task Consume(ConsumeContext<TenantDatabaseCreationRequested> context)
    {
        logger.LogInformation("Event received to create tenant database for TenantId: {TenantId}, ServiceName: {ServiceName}", context.Message.TenantId, context.Message.ServiceName);
        
        if (context.Message.ServiceName != "OrganizationService")
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
        
        var organizationMember = new OrganizationMember()
        {
            UserId = context.Message.CreatedByUserId,
            OrganizationId = context.Message.TenantId,
            Role = OrganizationMemberRole.Admin
        };
        
        db.OrganizationMembers.Add(organizationMember);
        await db.SaveChangesAsync();
        
        logger.LogInformation("Created initial admin member for organization {OrganizationId}", context.Message.TenantId);
        
        await publishEndpoint.Publish(new TenantDatabaseCreated()
        {
            TenantId = context.Message.TenantId,
            ServiceName = context.Message.ServiceName
        });
    }
}