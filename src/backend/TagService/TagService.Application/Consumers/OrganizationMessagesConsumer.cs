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
                    Id = Guid.Parse("a3f1c6d2-9c4b-4e1b-8f92-1a2b3c4d5e6f"), TenantId = tenantId, ProjectId = null,
                    Name = "Urgent", Color = "#FF0000",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.Parse("b4e2d7c3-8d5f-4a2b-9c03-2b3c4d5e6f7a"), TenantId = tenantId, ProjectId = null,
                    Name = "High Priority",
                    Color = "#FFA500", CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.Parse("c5d3e8f4-7e6a-4b3c-8d14-3c4d5e6f7a8b"), TenantId = tenantId, ProjectId = null,
                    Name = "Medium Priority",
                    Color = "#FFD700", CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.Parse("d6e4f9a5-6f7b-4c4d-9e25-4d5e6f7a8b9c"), TenantId = tenantId, ProjectId = null,
                    Name = "Low Priority",
                    Color = "#008000", CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.Parse("e7f5a0b6-5a8c-4d5e-0f36-5e6f7a8b9c0d"), TenantId = tenantId, ProjectId = null,
                    Name = "Bug", Color = "#DC143C",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.Parse("f8a6b1c7-4b9d-4e6f-1a47-6f7a8b9c0d1e"), TenantId = tenantId, ProjectId = null,
                    Name = "Feature", Color = "#800080",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.Parse("01234567-89ab-4cde-8f01-23456789abcd"), TenantId = tenantId, ProjectId = null,
                    Name = "Improvement", Color = "#4682B4",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"), TenantId = tenantId, ProjectId = null,
                    Name = "Support", Color = "#00CED1",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.Parse("9abcdef0-1234-4abc-9def-0123456789ab"), TenantId = tenantId, ProjectId = null,
                    Name = "Blocked", Color = "#000000",
                    CreatedAt = DateTime.UtcNow
                },
                new TagEntity
                {
                    Id = Guid.Parse("0fedcba9-4321-4fed-8cba-9876543210fe"), TenantId = tenantId, ProjectId = null,
                    Name = "Needs Review",
                    Color = "#1E90FF", CreatedAt = DateTime.UtcNow
                }
            };
        }
    }
}