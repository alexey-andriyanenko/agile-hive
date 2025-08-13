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
            var organization = Domain.Entities.Organization.Create(name, command.OwnerUserId);
            var organizationUser = new OrganizationUser(Guid.NewGuid())
            {
                UserId = command.OwnerUserId,
                OrganizationId = organization.Id,
            };

            dbContext.Organizations.Add(organization);
            dbContext.OrganizationUsers.Add(organizationUser);
        
            await dbContext.SaveChangesAsync();

            await organizationCreationSucceededMessageProducer.Produce(new OrganizationCreationSucceededMessage()
            {
                OrganizationId = organization.Id,
                OrganizationName = organization.Name.Value,
                OwnerUserId = organization.OwnerUserId
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