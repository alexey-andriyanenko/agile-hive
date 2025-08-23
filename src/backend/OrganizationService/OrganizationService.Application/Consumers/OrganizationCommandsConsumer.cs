using MassTransit;
using OrganizationMessages.Commands;
using OrganizationMessages.Messages;
using OrganizationService.Domain.Entities;
using OrganizationService.Infrastructure.Data;

namespace OrganizationService.Application.Consumers;

public class OrganizationCommandsConsumer(ApplicationDbContext dbContext,
    ITopicProducer<OrganizationCreationSucceededMessage> organizationCreationSucceededMessageProducer,
    ITopicProducer<OrganizationCreationFailedMessage> organizationCreationFailedMessageProducer
    ) : IConsumer<CreateOrganizationByOwnerUserCommand>
{
    public async Task Consume(ConsumeContext<CreateOrganizationByOwnerUserCommand> context)
    {
        try
        {
            var command = context.Message;

            var name = new Domain.ValueObjects.OrganizationName(command.OrganizationName);
            var organization = Organization.Create(name);
            var member = new OrganizationMember(Guid.NewGuid())
            {
                UserId = command.OwnerUserId,
                OrganizationId = organization.Id,
                Role = Domain.Enums.OrganizationMemberRole.Owner
            };

            dbContext.Organizations.Add(organization);
            dbContext.OrganizationMembers.Add(member);
        
            await dbContext.SaveChangesAsync();

            await organizationCreationSucceededMessageProducer.Produce(new OrganizationCreationSucceededMessage()
            {
                OrganizationId = organization.Id,
                OrganizationName = organization.Name.Value,
            });
        }
        catch (Exception e)
        {
            await organizationCreationFailedMessageProducer.Produce(new OrganizationCreationFailedMessage()
            {
                ErrorMessage = e.Message,
            });
        }
    }
}