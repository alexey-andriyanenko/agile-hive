namespace Shared.Messages;

public abstract class FailedMessageBase
{
    public required string ErrorMessage { get; set; }
}