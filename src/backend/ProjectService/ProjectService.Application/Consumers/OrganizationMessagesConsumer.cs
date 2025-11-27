using MassTransit;
using OrganizationMessages.Messages;
using ProjectMessages.Messages;
using ProjectService.Application.Extensions;
using ProjectService.Domain.Entities;
using ProjectService.Infrastructure.Data;

namespace ProjectService.Application.Consumers;

public class OrganizationMessagesConsumer(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint) : IConsumer<OrganizationCreationSucceededMessage>
{
    public async Task Consume(ConsumeContext<OrganizationCreationSucceededMessage> context)
    {
        var message = context.Message;

        var projects = new List<Project>();
        var projectMembers = new List<ProjectMember>();
        
        for (int i = 1; i <= 10; i++)
        {
            var name = $"Project {i} for {message.OrganizationName}";
            var project = new Project
            {
                Id = Guid.NewGuid(),
                OrganizationId = message.OrganizationId,
                Name = name,
                Slug = name.ToSlug(),
                Description = $"This is project {i} created for organization {message.OrganizationName}.",
                CreateByUserId = message.CreatedByUserId
            };
            var member = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = message.CreatedByUserId,
                Role = Domain.Enums.ProjectMemberRole.Admin
            };
            
            
            projects.Add(project);
            projectMembers.Add(member);
        }
        
        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.ProjectMembers.AddRangeAsync(projectMembers);
        await dbContext.SaveChangesAsync();

        foreach (var project in projects)
        {
            await publishEndpoint.Publish(new ProjectCreationSucceededMessage()
            {
                OrganizationId = project.OrganizationId,
                CreatedByUserId = project.CreateByUserId,
                ProjectId = project.Id,
            });
        }
    }
}