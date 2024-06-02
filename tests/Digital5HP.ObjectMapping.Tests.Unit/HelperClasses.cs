namespace Digital5HP.ObjectMapping.Tests.Unit
{
    using System.Collections.Generic;
    using System.Linq;

    public class Foo
    {
        public int OtherId { get; set; }

        public string Name { get; set; }
    }

    public record Bar(int Id);

    public abstract record BazBase(int Id);

    public record Baz(int Id, string Name) : BazBase(Id);

    public record Quuz(IEnumerable<Baz> Bazes);

    public class Quux
    {
        public IEnumerable<Foo> Foos { get; set; }
    }

    public interface INameFormatter
    {
        string Format(string name);
    }

    public class NameFormatter : INameFormatter
    {
        public string Format(string name)
        {
            return new(
                name.Reverse()
                    .ToArray());
        }
    }
}
