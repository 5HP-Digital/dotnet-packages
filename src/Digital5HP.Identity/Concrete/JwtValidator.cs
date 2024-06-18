namespace Digital5HP.Identity.Concrete;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

public class JwtValidator : IJwtValidator
{
    public ClaimsPrincipal ValidateToken(string token, JwtOptions options, int clockSkewSeconds = 0)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Configure the TokenValidationParameters. Assign the SigningKeys which were downloaded from Auth0.
        // Also set the Issuer and Audience(s) to validate
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecurityKey));
        var validationParameters =
            new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateIssuer = options.Issuer != null,
                ValidIssuer = options.Issuer,
                ValidateAudience = options.Audience != null,
                ValidAudience = options.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                RequireSignedTokens = true,
                ClockSkew = TimeSpan.FromSeconds(clockSkewSeconds),
            };

        // Validate the token. If the token is not valid for any reason, an exception will be thrown by the method
        var handler = new JwtSecurityTokenHandler();
        try
        {
            return handler.ValidateToken(token, validationParameters, out _);
        }
        catch (Exception ex)
        {
            throw new TokenException("Token invalid.", ex);
        }
    }
}
