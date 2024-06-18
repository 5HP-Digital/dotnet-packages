namespace Digital5HP.Identity.Concrete;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

public class JwtBuilder(ITimeProvider timeProvider) : IJwtBuilder
{
    private const string BEARER_TOKEN_TYPE = "Bearer";

    private readonly ITimeProvider timeProvider = timeProvider;

    public Token Build(JwtOptions options, params Claim[] claims)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrEmpty(options.SecurityKey))
        {
            throw new ConfigurationException($"JWT option {nameof(options.SecurityKey)} is required.");
        }

        if (claims != null && claims.Any(c => c.Type == "iat"))
        {
            throw new ArgumentException($"Claim 'iat' is set by JWT builder.", nameof(claims));
        }

        var securityAlgorithm = options.SecurityAlgorithm switch
        {
            SecurityAlgorithm.SHA256 => SecurityAlgorithms.HmacSha256,
            SecurityAlgorithm.SHA384 => SecurityAlgorithms.HmacSha384,
            SecurityAlgorithm.SHA512 => SecurityAlgorithms.HmacSha512,
            _ => throw new NotSupportedException($"Value {options.SecurityAlgorithm} is not a supported security algorithm."),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecurityKey));
        var signingCredentials = new SigningCredentials(key, securityAlgorithm);
        
        var now = this.timeProvider.Now;

        var issuedAt = ((DateTimeOffset)now).ToUnixTimeSeconds();

        var listClaims = new List<Claim>(claims)
                         {
                             new("iat", issuedAt.ToString(CultureInfo.InvariantCulture)),
                         };

        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            listClaims,
            expires: now.AddMinutes(options.ExpiresMinutes),
            signingCredentials: signingCredentials,
            notBefore: now);

        var expiresIn = Convert.ToInt64(
            token.ValidTo.Subtract(token.ValidFrom)
                 .TotalSeconds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new Token
               {
                   ExpiresIn = expiresIn,
                   TokenType = BEARER_TOKEN_TYPE,
                   AccessToken = accessToken,
                   IssuedAt = issuedAt,
               };
    }
}
