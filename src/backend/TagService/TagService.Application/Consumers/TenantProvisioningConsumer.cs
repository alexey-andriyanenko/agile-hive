using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TagService.Domain.Entities;
using TagService.Infrastructure;
using TagService.Infrastructure.Data;
using TenantProvisioning.Messages;

namespace TagService.Application.Consumers;

public class TenantProvisioningConsumer(ILogger<TenantProvisioningConsumer> logger, IPublishEndpoint publishEndpoint): IConsumer<TenantDatabaseCreationRequested>
{
    public async Task Consume(ConsumeContext<TenantDatabaseCreationRequested> context)
    {
        logger.LogInformation("Event received to create tenant database for TenantId: {TenantId}, ServiceName: {ServiceName}", context.Message.TenantId, context.Message.ServiceName);
        
        if (context.Message.ServiceName != "TagService")
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
        
        var predefinedTags = GetPredefinedTenantTags(context.Message.TenantId);
        db.Tags.AddRange(predefinedTags);
        await db.SaveChangesAsync();
        
        logger.LogInformation("Inserted predefined tags for tenant {TenantId}", context.Message.TenantId);
    }
    
     private static IReadOnlyList<TagEntity> GetPredefinedTenantTags(Guid tenantId)
    {
        {
            return new List<TagEntity>
            {
                new TagEntity
                {
                    Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = null, Name = "Urgent", Color = "#FF0000",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = null, Name = "High Priority",
                    Color = "#FFA500", CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = null, Name = "Medium Priority",
                    Color = "#FFD700", CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = null, Name = "Low Priority",
                    Color = "#008000", CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = null, Name = "Bug", Color = "#DC143C",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = null, Name = "Feature", Color = "#800080",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = null, Name = "Improvement", Color = "#4682B4",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = null, Name = "Support", Color = "#00CED1",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = null, Name = "Blocked", Color = "#000000",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = null, Name = "Needs Review",
                    Color = "#1E90FF", CreatedAt = DateTime.UtcNow
                }
            };
        }
    }
}