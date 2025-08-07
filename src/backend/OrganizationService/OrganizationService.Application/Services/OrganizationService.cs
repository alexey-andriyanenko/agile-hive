using Grpc.Core;
using MassTransit;
using OrganizationMessages.Messages;
using OrganizationService.gRPC;
using OrganizationService.Infrastructure.Data;

namespace OrganizationService.Application.Services;

public class OrganizationService(ApplicationDbContext dbContext, ITopicProducer<OrganizationCreatedMessage> topicProducer) : Organization.OrganizationBase
{
    public override async Task<CreateOrganizationResponse> Create(CreateOrganizationRequest request, ServerCallContext context)
    {
        Guid.TryParse(request.UserId, out var parsedUserId);
        
        if (parsedUserId == Guid.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Provided UserId is not valid GUID."));
        }

        try
        {
            var name = new Domain.ValueObjects.OrganizationName(request.OrganizationName);
            var organization = Domain.Entities.Organization.Create(name, parsedUserId);

            dbContext.Organizations.Add(organization);

            await dbContext.SaveChangesAsync();
            
            await topicProducer.Produce(new OrganizationCreatedMessage()
            {
                OrganizationId = organization.Id,
                OrganizationName = organization.Name.Value,
                OwnerUserId = organization.OwnerUserId
            });

            return new CreateOrganizationResponse()
            {
                OrganizationId = organization.Id.ToString(),
                OrganizationName = organization.Name.Value,
                OwnerUserId = organization.OwnerUserId.ToString()
            };
        }
        catch (ArgumentException e)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
        }
        catch (Exception e)
        {
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while creating the organization.", e));
        }
    }
}
