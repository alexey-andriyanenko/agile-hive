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
                                Id = Guid.Parse("0a1b2c3d-4e5f-4a6b-8c7d-1e2f3a4b5c6d"), TenantId = tenantId, ProjectId = projectId, Name = "Release v1.0",
                                Color = "#2E8B57", CreatedAt = DateTime.UtcNow
                            },
                            new TagEntity
                            {
                                Id = Guid.Parse("1b2c3d4e-5f6a-4b7c-8d9e-2f3a4b5c6d7e"), TenantId = tenantId, ProjectId = projectId, Name = "Release vNext",
                                Color = "#20B2AA", CreatedAt = DateTime.UtcNow
                            },
                            new TagEntity
                            {
                                Id = Guid.Parse("2c3d4e5f-6a7b-4c8d-8e9f-3a4b5c6d7e8f"), TenantId = tenantId, ProjectId = projectId, Name = "Frontend", Color = "#FF69B4",
                                CreatedAt = DateTime.UtcNow
                            },
                            new TagEntity
                            {
                                Id = Guid.Parse("3d4e5f6a-7b8c-4d9e-8f0a-4b5c6d7e8f90"), TenantId = tenantId, ProjectId = projectId, Name = "Backend", Color = "#6495ED",
                                CreatedAt = DateTime.UtcNow
                            },
                            new TagEntity
                            {
                                Id = Guid.Parse("4e5f6a7b-8c9d-4e0f-8a1b-5c6d7e8f9012"), TenantId = tenantId, ProjectId = projectId, Name = "API", Color = "#00BFFF",
                                CreatedAt = DateTime.UtcNow
                            },
                            new TagEntity
                            {
                                Id = Guid.Parse("5f6a7b8c-9d0e-4f1a-8b2c-6d7e8f901234"), TenantId = tenantId, ProjectId = projectId, Name = "Documentation",
                                Color = "#8B4513", CreatedAt = DateTime.UtcNow
                            },
                            new TagEntity
                            {
                                Id = Guid.Parse("6a7b8c9d-0e1f-4a2b-8c3d-7e8f90123456"), TenantId = tenantId, ProjectId = projectId, Name = "UI/UX", Color = "#FF1493",
                                CreatedAt = DateTime.UtcNow
                            },
                            new TagEntity
                            {
                                Id = Guid.Parse("7b8c9d0e-1f2a-4b3c-8d4e-8f9012345678"), TenantId = tenantId, ProjectId = projectId, Name = "Testing", Color = "#32CD32",
                                CreatedAt = DateTime.UtcNow
                            },
                            new TagEntity
                            {
                                Id = Guid.Parse("8c9d0e1f-2a3b-4c4d-8e5f-90123456789a"), TenantId = tenantId, ProjectId = projectId, Name = "Sprint 1", Color = "#FFDAB9",
                                CreatedAt = DateTime.UtcNow
                            },
                            new TagEntity
                            {
                                Id = Guid.Parse("9d0e1f2a-3b4c-4d5e-8f60-0123456789ab"), TenantId = tenantId, ProjectId = projectId, Name = "Sprint 2", Color = "#E6E6FA",
                                CreatedAt = DateTime.UtcNow
                            }
                        };
                    }
                }