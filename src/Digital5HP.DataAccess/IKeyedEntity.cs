namespace Digital5HP.DataAccess;

public interface IKeyedEntity<out TPrimaryKey> : IEntity
{
    TPrimaryKey Id { get; }
}
