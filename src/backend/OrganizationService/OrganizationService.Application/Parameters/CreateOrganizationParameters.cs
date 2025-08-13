namespace OrganizationService.Application.Parameters;

internal class CreateOrganizationParameters
{
   public Guid OwnerUserId { get; set; }
   
   public string Name { get; set; } = string.Empty;
}