namespace Digital5HP.DataAccess;

using Microsoft.Extensions.DependencyInjection;

public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Creates a <see cref="IUnitOfWork"/> using a new <see cref="IServiceScope"/>.
    /// </summary>
    /// <typeparam name="TUoW"></typeparam>
    /// <returns></returns>
    TUoW Create<TUoW>()
        where TUoW : IUnitOfWork;
}