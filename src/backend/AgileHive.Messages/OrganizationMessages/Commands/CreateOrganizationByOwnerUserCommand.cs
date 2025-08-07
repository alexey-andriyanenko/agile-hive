namespace OrganizationMessages.Commands;

public class CreateOrganizationByOwnerUserCommand
{
    public Guid OwnerUserId { get; set; }
    
    public string OrganizationName { get; set; } = string.Empty;
}