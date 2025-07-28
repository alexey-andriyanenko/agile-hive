namespace IdentityService.Contracts.Dtos;

public class GenerateTokensResult
{
    public string AccessToken { get; set; }
    
    public string RefreshToken { get; set; }
}