using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProjectMessages.Messages;
using ProjectService.Application.Extensions;
using ProjectService.Domain.Entities;
using ProjectService.Infrastructure;
using ProjectService.Infrastructure.Data;
using TenantProvisioning.Messages;

namespace ProjectService.Application.Consumers;

public class TenantProvisioningConsumer(ILogger<TenantProvisioningConsumer> logger, IPublishEndpoint publishEndpoint, IMemoryCache memoryCache)
    : IConsumer<TenantDatabaseCreationRequested>
{
    public async Task Consume(ConsumeContext<TenantDatabaseCreationRequested> context)
    {
        logger.LogInformation("Event received to create tenant database for TenantId: {TenantId}, ServiceName: {ServiceName}", context.Message.TenantId, context.Message.ServiceName);
        
        if (context.Message.ServiceName != "ProjectService")
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

        var cacheKey = $"tenantcontext:{context.Message.TenantId}:{context.Message.ServiceName}";
        
        memoryCache.Set(cacheKey, new TenantContext()
        {
            TenantId = context.Message.TenantId,
            ServiceName = context.Message.ServiceName,
            DbConnectionString = context.Message.DbConnectionString
        }, TimeSpan.FromDays(1));
            
        await using var db = new ApplicationDbContext(optionsBuilder.Options, tenantContext);

        logger.LogInformation("Applying migrations for service {ServiceName} and tenant {TenantId}", context.Message.ServiceName, context.Message.TenantId);
        
        await db.Database.MigrateAsync();

        logger.LogInformation("Migrations applied for service {ServiceName} and tenant {TenantId}", context.Message.ServiceName, context.Message.TenantId);
        
        await publishEndpoint.Publish(new TenantDatabaseCreated()
        {
            TenantId = context.Message.TenantId,
            ServiceName = context.Message.ServiceName
        });
        
        logger.LogInformation("Seeding initial projects for tenant {TenantId}", context.Message.TenantId);
        
        var projects = new List<Project>();
        var projectMembers = new List<ProjectMember>();
        
        for (int i = 1; i <= 10; i++)
        {
            var name = $"Project {i} for {context.Message.TenantId}";
            var project = new Project
            {
                Id = Guid.NewGuid(),
                OrganizationId = context.Message.TenantId,
                Name = name,
                Slug = name.ToSlug(),
                Description = $"This is project {i} created for organization {context.Message.TenantId}.",
                CreateByUserId = context.Message.CreatedByUserId
            };
            var member = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = context.Message.CreatedByUserId,
                Role = Domain.Enums.ProjectMemberRole.Admin
            };
            
            
            projects.Add(project);
            projectMembers.Add(member);
        }
        
        await db.Projects.AddRangeAsync(projects);
        await db.ProjectMembers.AddRangeAsync(projectMembers);
        await db.SaveChangesAsync();

        foreach (var project in projects)
        {
            await publishEndpoint.Publish(new ProjectCreationSucceededMessage()
            {
                OrganizationId = project.OrganizationId,
                CreatedByUserId = project.CreateByUserId,
                ProjectId = project.Id,
            });
        }
        
        logger.LogInformation("Seeded initial projects for tenant {TenantId}", context.Message.TenantId);
    }
}