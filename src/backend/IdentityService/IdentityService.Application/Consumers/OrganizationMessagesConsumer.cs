using MassTransit;
using OrganizationMessages.Messages;

namespace IdentityService.Application.Consumers;

public class OrganizationMessagesConsumer : IConsumer<OrganizationCreatedMessage>
{
    public Task Consume(ConsumeContext<OrganizationCreatedMessage> context)
    {
        var message = context.Message;
        
        Console.WriteLine($"Organization created with ID {message.OrganizationId}");
        
        return Task.CompletedTask;
    }
}