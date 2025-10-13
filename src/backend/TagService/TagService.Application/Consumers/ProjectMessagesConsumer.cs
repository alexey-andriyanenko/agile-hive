using MassTransit;
using ProjectMessages.Messages;
using TagService.Domain.Entities;
using TagService.Infrastructure.Data;

namespace TagService.Application.Consumers;

public class ProjectMessagesConsumer(
    ApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint
    ) : IConsumer<ProjectCreationSucceededMessage>
{
    public async Task Consume(ConsumeContext<ProjectCreationSucceededMessage> context)
    {
        var message = context.Message;

        var predefinedTags = GetPredefinedProjectTags(message.OrganizationId, message.ProjectId);

        dbContext.Tags.AddRange(predefinedTags);
        await dbContext.SaveChangesAsync();
    }
    
    public static List<TagEntity> GetPredefinedProjectTags(Guid tenantId, Guid projectId)
    {
        return new List<TagEntity>
        {
            new TagEntity
            {
                Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = projectId, Name = "Release v1.0",
                Color = "#2E8B57", CreatedAt = DateTime.UtcNow
            },
            new TagEntity
            {
                Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = projectId, Name = "Release vNext",
                Color = "#20B2AA", CreatedAt = DateTime.UtcNow
            },
            new TagEntity
            {
                Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = projectId, Name = "Frontend", Color = "#FF69B4",
                CreatedAt = DateTime.UtcNow
            },
            new TagEntity
            {
                Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = projectId, Name = "Backend", Color = "#6495ED",
                CreatedAt = DateTime.UtcNow
            },
            new TagEntity
            {
                Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = projectId, Name = "API", Color = "#00BFFF",
                CreatedAt = DateTime.UtcNow
            },
            new TagEntity
            {
                Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = projectId, Name = "Documentation",
                Color = "#8B4513", CreatedAt = DateTime.UtcNow
            },
            new TagEntity
            {
                Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = projectId, Name = "UI/UX", Color = "#FF1493",
                CreatedAt = DateTime.UtcNow
            },
            new TagEntity
            {
                Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = projectId, Name = "Testing", Color = "#32CD32",
                CreatedAt = DateTime.UtcNow
            },
            new TagEntity
            {
                Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = projectId, Name = "Sprint 1", Color = "#FFDAB9",
                CreatedAt = DateTime.UtcNow
            },
            new TagEntity
            {
                Id = Guid.NewGuid(), TenantId = tenantId, ProjectId = projectId, Name = "Sprint 2", Color = "#E6E6FA",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}