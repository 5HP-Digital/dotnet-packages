namespace Digital5HP.Identity;

public class JwtOptions
{
    /// <summary>
    /// Value to use in issuer claim. If not specified, not utilized.
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// Value to use in audience claim. If not specified, not utilized.
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// Security key used to encrypt tokens. (Required)
    /// </summary>
    public string SecurityKey { get; set; }

    /// <summary>
    /// Expiration of token in minutes. Default value is 20 minutes.
    /// </summary>
    public int ExpiresMinutes { get; set; } = 20;

    /// <summary>
    /// Security algorithm used to encrypt tokens. Default value is SHA-256.
    /// </summary>
    public SecurityAlgorithm SecurityAlgorithm { get; set; } = SecurityAlgorithm.SHA256;

    /// <summary>
    /// Clock skew to apply to validation. Default value is 0.
    /// </summary>
    public int ClockSkew { get; set; }
}
