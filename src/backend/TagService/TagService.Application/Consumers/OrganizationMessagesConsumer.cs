using MassTransit;
using OrganizationMessages.Messages;
using ProjectMessages.Messages;
using TagService.Domain.Entities;
using TagService.Infrastructure.Data;

namespace TagService.Application.Consumers;

public class OrganizationMessagesConsumer(
    ApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint
) : IConsumer<OrganizationCreationSucceededMessage>
{
    public async Task Consume(ConsumeContext<OrganizationCreationSucceededMessage> context)
    {
        var message = context.Message;

        var predefinedTags = GetPredefinedTenantTags(message.OrganizationId);

        dbContext.Tags.AddRange(predefinedTags);
        await dbContext.SaveChangesAsync();
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