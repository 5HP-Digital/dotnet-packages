namespace Digital5HP;

using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// A Unique ID with a user-friendly string representation.
/// </summary>
public readonly struct UserFriendlyUniqueId : IEquatable<UserFriendlyUniqueId>
{
    internal const string LETTERS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private readonly string uniqueId;

    private UserFriendlyUniqueId(string uniqueId)
    {
        this.uniqueId = uniqueId;
    }

    /// <summary>
    /// Creates a unique ID based on <see cref="Guid"/> provided.
    /// </summary>
    public UserFriendlyUniqueId(Guid uniqueId)
        : this(uniqueId.ToByteArray())
    {
    }

    /// <summary>
    /// Creates a unique ID based on <paramref name="bytes"/> provided.
    /// </summary>
    /// <remarks>
    /// <paramref name="bytes"/> must have at least one byte.
    /// </remarks>
    public UserFriendlyUniqueId(byte[] bytes)
        : this(new ReadOnlySpan<byte>(bytes ?? throw new ArgumentNullException(nameof(bytes))))
    {
    }

    /// <summary>
    /// Creates a unique ID based on <paramref name="span"/> provided.
    /// </summary>
    /// <remarks>
    /// <paramref name="span"/> must have at least one byte.
    /// </remarks>
    public UserFriendlyUniqueId(ReadOnlySpan<byte> span)
    {
        if (span.Length == 0)
        {
            throw new ArgumentException($"{nameof(span)} must have at least one byte.", nameof(span));
        }

        var guid128Bit = new BigInteger(span, isUnsigned: true);
        this.uniqueId = ConvertBigIntegerToBase36(guid128Bit);
    }

    /// <summary>
    /// Creates a unique ID based on <see cref="Guid"/>.
    /// </summary>
    public static UserFriendlyUniqueId New()
    {
        return new(Guid.NewGuid());
    }

    /// <summary>
    /// Creates a unique ID based on the provided <paramref name="num"/> of characters in the user-friendly representation.
    /// </summary>
    public static UserFriendlyUniqueId New(int num)
    {
        if (num <= 0)
        {
            throw new ArgumentException($"{nameof(num)} must be greater than zero.", nameof(num));
        }

        var chars = new char[num];
        for (var i = 0; i < num; i++)
        {
            chars[i] = LETTERS[RandomNumberGenerator.GetInt32(0, LETTERS.Length)];
        }

        return new UserFriendlyUniqueId(new string(chars));
    }

    /// <summary>
    /// Returns user-friendly representation of this object.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.uniqueId;
    }

    public override bool Equals(object obj)
    {
        return obj switch
               {
                   null                         => false,
                   UserFriendlyUniqueId unique2 => this.Equals(unique2),
                   Guid guid                    => this.Equals(new UserFriendlyUniqueId(guid)),
                   byte[] bytes                 => this.Equals(new UserFriendlyUniqueId(bytes)),
                   string str                   => this.Equals(new UserFriendlyUniqueId(str)),
                   _                            => false
               };
    }

    /// <summary>
    /// Indicated whether this instance and the specified <see cref="UserFriendlyUniqueId"/> object are equal.
    /// </summary>
    /// <returns><see langword="true"/> if <paramref name="other"/> and this instance represent the same value; otherwise, <see langword="false"/></returns>
    public bool Equals(UserFriendlyUniqueId other)
    {
        return this.uniqueId == other.uniqueId;
    }

    public override int GetHashCode()
    {
        return this.uniqueId != null ? this.uniqueId.GetHashCode(StringComparison.Ordinal) : 0;
    }

    private static string ConvertBigIntegerToBase36(BigInteger integer)
    {
        var sb = new StringBuilder();
        do
        {
            sb.Append(LETTERS[(int) (integer % LETTERS.Length)]);
            integer /= LETTERS.Length;
        } while (integer != BigInteger.Zero);

        return sb.ToString();
    }

    public static bool operator ==(UserFriendlyUniqueId left, UserFriendlyUniqueId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(UserFriendlyUniqueId left, UserFriendlyUniqueId right)
    {
        return !(left == right);
    }
}
