namespace IdentityMessages.Messages;

public class UserCreatedMessage
{
    public Guid UserId { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
}