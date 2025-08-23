using Shared.Messages;

namespace IdentityMessages.Messages;

public class UserDeletionFailedMessage : FailedMessageBase
{
    public Guid UserId { get; set; }
}