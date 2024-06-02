namespace Digital5HP.DataAccess.EntityFramework.Specification;

using Digital5HP.DataAccess.Specifications;
using Digital5HP.ObjectMapping;

public abstract class KeyedAbstractResultableQueryableSpecification<TSpec, TEntity, TPrimaryKey>(IMapperProvider mapperProvider, ISpecificationFactory specificationFactory)
    : KeyedAbstractQueryableSpecification<TSpec, TEntity, TPrimaryKey>(mapperProvider, specificationFactory), IAbstractResultableSpecification<TEntity>
    where TSpec : class, IKeyedSpecification<TSpec, TEntity, TPrimaryKey>, IAbstractResultableSpecification<TEntity>
    where TEntity : class, IKeyedEntity<TPrimaryKey>
{

    /// <summary>
    /// Specifies a derived entity type to include in query result.
    /// </summary>
    /// <remarks>
    /// Note: When the derived types have different navigation properties, this would look something like this:
    /// public IDerivedSpecificationResult&gt;BattleActivity&lt; AsAny()
    /// {
    ///    var queryable = this.Queryable
    ///                        .Include(a => (a as PlayerRegisteredToTeamBattleActivity).Player)
    ///                        .ThenInclude(i => i.Profile)
    ///                        .Include(a => (a as EndedBattleActivity).WinningTeam)
    ///                        .Include(a => (a as PlayerDefeatedBattleActivity).Player)
    ///                        .ThenInclude(p => p.Profile)
    ///                        .Include(a => (a as TeamDefeatedBattleActivity).Team);
    ///    return new DerivedSpecificationResult&gt;BattleActivity&lt;(queryable);
    /// }
    /// </remarks>
    public abstract IAbstractSpecificationResult<TEntity> AsAny();
}
