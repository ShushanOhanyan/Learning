using System.IdentityModel.Tokens.Jwt;
using AuthenticationService.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Configuration;

namespace AuthenticationService;

public interface IAuthenticationService
{
    string GetAuthUrl(string state, string redirectUrl);

    Task<(string RefreshToken, string Email)> GetAuthDetailsAsync(string redirectUrl, string authorizationCode);

    UserCredential GetUserCredential(string refreshToken);
}


public class GoogleAuthService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    public const string ConfigSectionName = "GoogleApi";
    
    public string GetAuthUrl(string state, string redirectUrl)
    {
        var authFlow = CreateAuthCodeFlow(state);
        
        Uri authUri = authFlow.CreateAuthorizationCodeRequest(redirectUrl).Build();

        return authUri.AbsoluteUri;
    }

    public async Task<(string RefreshToken, string Email)> GetAuthDetailsAsync(string redirectUrl, string authorizationCode)
    {
        var authFlow = CreateAuthCodeFlow();
        
        TokenResponse tokenResponse =
            await authFlow.ExchangeCodeForTokenAsync(null, authorizationCode, redirectUrl, CancellationToken.None);

        var email = ExtractEmailFromIdToken(tokenResponse.IdToken);

        return (tokenResponse.RefreshToken, email);
    }
    
    public UserCredential GetUserCredential(string refreshToken)
    {
        var token = new TokenResponse { RefreshToken = refreshToken };

        var initializer = new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = GetGoogleConfig().OAuth2ClientId,
                ClientSecret = GetGoogleConfig().OAuth2ClientSecret,
            },
            Scopes = new[] { GetGoogleConfig().OAuth2Scope },
        };
            
        UserCredential credential = new UserCredential(new AuthorizationCodeFlow(initializer), "", token);

        return credential;
    }
    
    private string ExtractEmailFromIdToken(string idToken)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(idToken);

        string? email = token.Claims
            .FirstOrDefault(claim => claim.Type.Equals("email", StringComparison.OrdinalIgnoreCase))?.Value;

        if (string.IsNullOrEmpty(email)) {
            throw new Exception("Failed to extract email from authenticated credentials.");
        }

        return email;
    }

    private GoogleApiConfig GetGoogleConfig()
    {
        IConfigurationSection? configSection = _configuration.GetSection("GoogleApi");
        if (configSection == null || !configSection.Exists())
            throw new ArgumentNullException("Google config can't be null.");

        GoogleApiConfig config = new GoogleApiConfig();
        configSection.Bind(config);

        return config;
    }

    private IAuthorizationCodeFlow CreateAuthCodeFlow(string? userDefinedStateParam = null)
    {
        return new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                Prompt = "consent",
                ClientSecrets = new ClientSecrets()
                {
                    ClientId = GetGoogleConfig().OAuth2ClientId,
                    ClientSecret = GetGoogleConfig().OAuth2ClientSecret,
                },
                Scopes = new[] {GetGoogleConfig().OAuth2Scope},
                UserDefinedQueryParams = new Dictionary<string, string?> {{"state", userDefinedStateParam}},
            });
    }

    public GoogleAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
}
