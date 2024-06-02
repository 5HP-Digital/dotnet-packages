namespace Digital5HP.DataAccess;

using System;

using Microsoft.Extensions.DependencyInjection;

public class UnitOfWorkFactory(IServiceProvider serviceProvider) : IUnitOfWorkFactory
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public TUoW Create<TUoW>()
        where TUoW : IUnitOfWork
    {
        return this.serviceProvider.GetRequiredService<TUoW>();
    }
}