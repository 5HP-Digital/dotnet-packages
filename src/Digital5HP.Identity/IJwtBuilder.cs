namespace Digital5HP.Identity;

public interface IJwtBuilder
{
    Token Build(JwtOptions options, params System.Security.Claims.Claim[] claims);
}
