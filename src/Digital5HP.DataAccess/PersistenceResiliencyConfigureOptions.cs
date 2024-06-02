namespace Digital5HP.DataAccess;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal class PersistenceResiliencyConfigureOptions(IConfiguration configuration) : IConfigureOptions<PersistenceResiliencyOptions>
{
    private const string PERSISTENCE_RESILIENCY_CONFIGURATION_KEY = "Resiliency:Persistence";

    private readonly IConfiguration configuration = configuration;

    public void Configure(PersistenceResiliencyOptions options)
    {
        this.configuration.GetSection(PERSISTENCE_RESILIENCY_CONFIGURATION_KEY)
            .Bind(options);
    }
}
