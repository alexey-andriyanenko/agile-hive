using FluentValidation;
using MassTransit;
using OrganizationMessages.Commands;
using OrganizationMessages.Messages;
using OrganizationService.Application.Extensions;
using OrganizationService.Contracts;
using OrganizationService.Domain.Entities;
using OrganizationService.Infrastructure.Data;

namespace OrganizationService.Application.Consumers;

public class OrganizationCommandsConsumer(ApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    IValidator<CreateOrganizationRequest> validator
    ) : IConsumer<CreateOrganizationByOwnerUserCommand>
{
    public async Task Consume(ConsumeContext<CreateOrganizationByOwnerUserCommand> context)
    {
        try
        {
            var command = context.Message;
            var validationResult = await validator.ValidateAsync(new CreateOrganizationRequest()
            {
                OrganizationName = command.OrganizationName,
            });
         
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
         
            var organization = new Organization(Guid.NewGuid())
            {
                Name = command.OrganizationName,
                Slug = command.OrganizationName.ToSlug(),
            };
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
                OrganizationName = organization.Name
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