using IdentityMessages.Messages;
using MassTransit;

namespace ProjectService.Application.Consumers;

public class IdentityMessagesConsumer : IConsumer<UserCreationSucceededMessage>
{
    public Task Consume(ConsumeContext<UserCreationSucceededMessage> context)
    {
        var message = context.Message;
        
        Console.WriteLine($"User created with ID {message.UserId}");
        
        return Task.CompletedTask;
    }
}