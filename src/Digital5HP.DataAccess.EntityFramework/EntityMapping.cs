namespace Digital5HP.DataAccess.EntityFramework;

using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class EntityMapping<T> : IEntityTypeConfiguration<T>
    where T : class
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        this.ConfigureInternal(builder);

        var entityType = typeof(T);

        // filter out deleted entities (soft delete)
        if (typeof(ISoftDeletable).IsAssignableFrom(entityType)
            && (entityType.BaseType == null || entityType.BaseType == typeof(object)))
        {
            builder.AddQueryFilter(e => !((ISoftDeletable) e).IsDeleted);
        }
    }

    protected abstract void ConfigureInternal(EntityTypeBuilder<T> builder);
}
