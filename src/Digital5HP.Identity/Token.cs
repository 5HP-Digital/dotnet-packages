namespace Digital5HP.Identity;

/// <summary>
/// Represents a JWT Token response
/// </summary>
public class Token
{
    /// <summary>
    /// Identifies the time at which the JWT was issued. Its value is a
    /// number containing a NumericDate value. The value is represented
    /// as seconds since the Unix epoch (1970-Jan-01).
    /// </summary>
    public long IssuedAt { get; set; }

    /// <summary>
    /// String representation of token with all claims.
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// The duration of time the access token is granted for (in seconds).
    /// </summary>
    public long ExpiresIn { get; set; }

    /// <summary>
    /// The type of token this is. Typically just the string “bearer”.
    /// </summary>
    public string TokenType { get; set; }
}
