namespace AuthenticationService.Models;

public class GoogleApiConfig
{
    public string AuthorizationMethod { get; set; }
    
    public string OAuth2Mode { get; set; }
    
    public string OAuth2ClientId { get; set; }
    
    public string OAuth2ClientSecret { get; set; }
    
    public string OAuth2Scope { get; set; }
}