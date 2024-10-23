namespace Digital5HP.Identity;

using Microsoft.Extensions.DependencyInjection;

using Digital5HP.Identity.Concrete;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers JWT services (<see cref="IJwtBuilder"/> and <see cref="IJwtValidator"/>).
    /// </summary>
    public static IServiceCollection AddJwt(this IServiceCollection services)
    {
        services.AddSingleton<IJwtBuilder, JwtBuilder>();
        services.AddSingleton<IJwtValidator, JwtValidator>();

        return services;
    }
}
