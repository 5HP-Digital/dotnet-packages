// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
namespace Digital5HP.ObjectMapping.Tests.Unit.Mapster
{
    using Digital5HP.ObjectMapping.Mapster;

    public class SourceClass
    {
        public string Id { get; set; }
    }

    public class TargetClass
    {
        public string Id { get; set; }
    }

    public class TestMappingProfile : MappingProfile<SourceClass>
    {
        protected override void Configure(MappingConfiguration<SourceClass> config)
        {
            config.MapTo<TargetClass>();
        }
    }
}
