using MassTransit;
using OrganizationMessages.Commands;
using OrganizationMessages.Messages;
using OrganizationService.Infrastructure.Data;

namespace OrganizationService.Application.Consumers;

public class IdentityCommandsConsumer(ApplicationDbContext dbContext, ITopicProducer<OrganizationCreatedMessage> topicProducer) : IConsumer<CreateOrganizationByOwnerUserCommand>
{
    public async Task Consume(ConsumeContext<CreateOrganizationByOwnerUserCommand> context)
    {
        var command = context.Message;

        var name = new Domain.ValueObjects.OrganizationName(command.OrganizationName);
        var organization = Domain.Entities.Organization.Create(name, command.OwnerUserId);

        dbContext.Organizations.Add(organization);
        
        await dbContext.SaveChangesAsync();

        await topicProducer.Produce(new OrganizationCreatedMessage()
        {
            OrganizationId = organization.Id,
            OrganizationName = organization.Name.Value,
            OwnerUserId = organization.OwnerUserId
        });
    }
}