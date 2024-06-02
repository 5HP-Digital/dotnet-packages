namespace Digital5HP.AspNetCore.Versioning;

using System.Collections;
using System.Collections.Generic;

using Asp.Versioning;

public class ApiVersionCollection : IReadOnlyCollection<ApiVersion>
{
    public static readonly ApiVersionCollection Instance = new();

    private readonly HashSet<ApiVersion> versions = new();

    public IEnumerator<ApiVersion> GetEnumerator() => this.versions.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public int Count => this.versions.Count;

    internal void Add(ApiVersion version) => this.versions.Add(version);
}
