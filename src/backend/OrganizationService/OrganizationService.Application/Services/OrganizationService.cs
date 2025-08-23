using Grpc.Core;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrganizationMessages.Messages;
using OrganizationService.Application.Mapping;
using OrganizationService.Domain.Entities;
using OrganizationService.gRPC;
using OrganizationService.Infrastructure.Data;
using Organization = OrganizationService.gRPC.Organization;

namespace OrganizationService.Application.Services;

public class OrganizationService(
    ApplicationDbContext dbContext,
    ITopicProducer<OrganizationCreationSucceededMessage> organizationCreationSucceededMessageProducer,
    ITopicProducer<OrganizationCreationFailedMessage> organizationCreationFailedMessageProducer
) : Organization.OrganizationBase
{
    public override async Task<OrganizationDto> Create(CreateOrganizationRequest request, ServerCallContext context)
    {
        Guid.TryParse(request.UserId, out var parsedUserId);

        if (parsedUserId == Guid.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Provided UserId is not valid GUID."));
        }

        try
        {
            var name = new Domain.ValueObjects.OrganizationName(request.OrganizationName);
            var organization = Domain.Entities.Organization.Create(name);
            var organizationUser = new OrganizationMember
            {
                UserId = parsedUserId,
                OrganizationId = organization.Id,
                Role = Domain.Enums.OrganizationMemberRole.Owner
            };

            dbContext.Organizations.Add(organization);
            dbContext.OrganizationMembers.Add(organizationUser);

            await dbContext.SaveChangesAsync();

            await organizationCreationSucceededMessageProducer.Produce(new OrganizationCreationSucceededMessage()
            {
                OrganizationId = organization.Id,
                OrganizationName = organization.Name.Value,
            });

            return new OrganizationDto()
            {
                Id = organization.Id.ToString(),
                Name = organization.Name.Value,
            };
        }
        catch (ArgumentException e)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
        }
        catch (DbUpdateException e)
        {
            await organizationCreationFailedMessageProducer.Produce(new OrganizationCreationFailedMessage()
            {
                ErrorMessage = e.Message
            });
            
            throw new RpcException(new Status(StatusCode.Internal, "Database update failed.", e));
        }
        catch (Exception e)
        {
            await organizationCreationFailedMessageProducer.Produce(new OrganizationCreationFailedMessage()
            {
                ErrorMessage = e.Message,
            });
            
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while creating the organization.", e));
        }
    }

    public override async Task<OrganizationDto> GetById(GetOrganizationByIdRequest request, ServerCallContext context)
    {
        var organization = await dbContext.Organizations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == Guid.Parse(request.OrganizationId));

        if (organization is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Organization with ID '{request.OrganizationId}' not found."));
        }
        
        return organization.ToDto();
    }

    public override async Task<GetManyOrganizationsResponse> GetMany(GetManyOrganizationsRequest request,
        ServerCallContext context)
    {
        var organizationIds = request.OrganizationIds.Select(Guid.Parse).ToList();
        List<Domain.Entities.Organization> organizations;

        if (request.OrganizationIds.Count > 0)
        {
            organizations = await dbContext.Organizations.Where(x => organizationIds.Contains(x.Id))
                .ToListAsync();
        }
        else
        {
            organizations = await dbContext.Organizations.ToListAsync();
        }

        return new GetManyOrganizationsResponse()
        {
            Organizations =
            {
                organizations.Select(x => x.ToDto())
            }
        };
    }

    public override async Task<OrganizationDto> Update(UpdateOrganizationRequest request, ServerCallContext context)
    {
        var organizationId = Guid.Parse(request.OrganizationId);
        var organization = await dbContext.Organizations.SingleOrDefaultAsync(x => x.Id == organizationId);
        
        if (organization is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Organization with ID '{request.OrganizationId}' not found."));
        }
        
        try
        {
            var newName = new Domain.ValueObjects.OrganizationName(request.OrganizationName);
            organization.Rename(newName);
            
            dbContext.Organizations.Update(organization);
            await dbContext.SaveChangesAsync();

            return organization.ToDto();
        }
        catch (ArgumentException e)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
        }
    }
}