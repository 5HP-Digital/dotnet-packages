namespace Digital5HP.Text.M3U;

public class Channel
{
#pragma warning disable CA1056 // URI-like properties should not be strings
    public string MediaUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    public string TvgId { get; set; }

    public string TvgName { get; set; }

    public int? TvgChannelNumber { get; set; }

#pragma warning disable CA1056 // URI-like properties should not be strings
    public string LogoUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

    public string GroupTitle { get; set; }

    public int Duration { get; set; } = -1;

    public string Title { get; set; }

    public string ChannelId { get; set; }

    public int? ChannelNumber { get; set; }
}
