using MassTransit;
using OrganizationMessages.Messages;

namespace IdentityService.Application.Consumers;

public class OrganizationMessagesConsumer :
    IConsumer<OrganizationCreationSucceededMessage>,
    IConsumer<OrganizationCreationFailedMessage>
{
    public Task Consume(ConsumeContext<OrganizationCreationSucceededMessage> context)
    {
        var message = context.Message;
        
        Console.WriteLine($"Organization created with ID {message.OrganizationId}");
        
        return Task.CompletedTask;
    }
    
    public Task Consume(ConsumeContext<OrganizationCreationFailedMessage> context)
    {
        var message = context.Message;
        
        Console.WriteLine($"Organization creation failed: {message.ErrorMessage}");
        
        return Task.CompletedTask;
    }
}