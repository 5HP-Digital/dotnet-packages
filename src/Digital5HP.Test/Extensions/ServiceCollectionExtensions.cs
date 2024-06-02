namespace Digital5HP.Test
{
    using System.Linq;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Determines whether the <typeparamref name="TService"></typeparamref> is registered.
        /// </summary>
        /// <typeparam name="TService">Service or interface to check.</typeparam>
        public static bool IsRegistered<TService>(this IServiceCollection services, bool isImplementationType = false)
            where TService : class
        {
            return isImplementationType
                ? services.Any(sd => sd.ImplementationType == typeof(TService))
                : services.Any(sd => sd.ServiceType == typeof(TService));
        }
    }
}
