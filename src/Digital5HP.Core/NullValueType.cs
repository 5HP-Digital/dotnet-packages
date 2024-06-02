namespace Digital5HP;

public readonly struct NullValueType<T>(T? item) : System.IEquatable<NullValueType<T>>
    where T : struct
{
    public T? Item { get; } = item;

#pragma warning disable CA2225
    public static implicit operator T?(NullValueType<T> vt) => vt.Item;
#pragma warning restore CA2225

#pragma warning disable CA2225
    public static implicit operator NullValueType<T>(T? v) => new(v);
#pragma warning restore CA2225

    public static implicit operator string(NullValueType<T> vt) => vt.ToString();

#pragma warning disable CA2225
    public static implicit operator NullValueType<T>(string v) => TryParse(v, out var nv) ? nv : new NullValueType<T>(null);
#pragma warning restore CA2225

    public static bool operator ==(NullValueType<T> left, NullValueType<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(NullValueType<T> left, NullValueType<T> right)
    {
        return !(left == right);
    }

#pragma warning disable CA1000, MA0018
    public static bool TryParse(string s, out NullValueType<T> value)
#pragma warning restore CA1000, MA0018
    {
        if (string.IsNullOrEmpty(s))
        {
            value = new(null);
            return true;
        }

        if (s.TryChangeType(out T result))
        {
            value = result;
            return true;
        }

        value = new(null);
        return false;
    }

    public override string ToString()
    {
        return this.Item != null ? this.Item.ToString() : "";
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return this.Item == null;

        if (obj is not NullValueType<T> obj2)
            return false;

        if (this.Item == null)
            return obj2.Item == null;

        return obj2.Item != null && this.Item.Equals(obj2.Item);
    }

    public override int GetHashCode()
    {
        if (this.Item == null)
            return 0;

        var result = this.Item.GetHashCode();

        if (result >= 0)
            result++;

        return result;
    }

    public bool Equals(NullValueType<T> other)
    {
        return this.Equals((object)other);
    }
}
