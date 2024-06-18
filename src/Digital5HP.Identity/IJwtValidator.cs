namespace Digital5HP.Identity;

using System.Security.Claims;

public interface IJwtValidator
{
    /// <summary>
    /// Validates <paramref name="token"/> using the provided <see cref="JwtOptions"/>. If valid, returns <see cref="ClaimsPrincipal"/>. Otherwise, throws <see cref="TokenException"/>.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="settings"></param>
    /// <param name="clockSkewSeconds"></param>
    /// <returns></returns>
    /// <exception cref="TokenException">Throws when validation fails.</exception>
    ClaimsPrincipal ValidateToken(string token, JwtOptions options, int clockSkewSeconds = 0);
}
