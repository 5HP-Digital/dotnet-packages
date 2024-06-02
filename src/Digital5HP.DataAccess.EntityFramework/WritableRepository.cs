namespace Digital5HP.DataAccess.EntityFramework;

using System;

using Digital5HP.DataAccess.EntityFramework.Specification;
using Digital5HP.DataAccess.Operations;
using Digital5HP.DataAccess.Specifications;
using Digital5HP.Logging;

public abstract class WritableRepository<TEntity, TSpecification>(ILogger logger, IDbContext context, ISpecificationFactory specificationFactory)
    : ReadableRepository<TEntity, TSpecification>(logger, context, specificationFactory),
      IWritableRepository<TEntity, TSpecification>
    where TEntity : class, IEntity
    where TSpecification : ISpecification<TEntity>
{
    protected override bool IsReadOnly => false;

    public void Add<T>(T entity)
        where T : TEntity
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        this.Context.Set<TEntity>()
            .Add(entity);
    }

    public void Update<T>(T entity)
        where T : TEntity
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        this.Context.Set<TEntity>()
            .Update(entity);
    }

    public void Delete<T>(T entity)
        where T : TEntity
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        // If the IDeletableEntity interface is used, flag the item as deleted rather than removing.
        if (entity is ISoftDeletable deletableEntity)
        {
            deletableEntity.IsDeleted = true;

            this.Context.Set<TEntity>()
                .Update(entity);
        }
        else
        {
            this.Context.Set<TEntity>()
                .Remove(entity);
        }
    }

    public void Upsert<T>(T entity)
        where T : TEntity
    {
        throw new NotSupportedException("Upsert operation is not supported in EF.");
    }

    // TODO: implement bulk operations in EF
    public IOperationDefinitionBuilder<TEntity> DefineOperation() =>
        throw new NotSupportedException("Defining operation for queue/bulk is not supported in EF.");
}
