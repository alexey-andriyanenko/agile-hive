using MassTransit;
using OrganizationMessages.Commands;
using OrganizationMessages.Messages;
using OrganizationService.Domain.Entities;
using OrganizationService.Infrastructure.Data;

namespace OrganizationService.Application.Consumers;

public class OrganizationCommandsConsumer(ApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint
    ) : IConsumer<CreateOrganizationByOwnerUserCommand>
{
    public async Task Consume(ConsumeContext<CreateOrganizationByOwnerUserCommand> context)
    {
        try
        {
            var command = context.Message;

            var name = new Domain.ValueObjects.OrganizationName(command.OrganizationName);
            var organization = Organization.Create(name);
            var member = new OrganizationMember()
            {
                UserId = command.OwnerUserId,
                OrganizationId = organization.Id,
                Role = Domain.Enums.OrganizationMemberRole.Owner
            };

            dbContext.Organizations.Add(organization);
            dbContext.OrganizationMembers.Add(member);
        
            await dbContext.SaveChangesAsync();

            await publishEndpoint.Publish(new OrganizationCreationSucceededMessage()
            {
                OrganizationId = organization.Id,
                OrganizationName = organization.Name.Value,
            });
        }
        catch (Exception e)
        {
            await publishEndpoint.Publish(new OrganizationCreationFailedMessage()
            {
                ErrorMessage = e.Message,
            });
        }
    }
}