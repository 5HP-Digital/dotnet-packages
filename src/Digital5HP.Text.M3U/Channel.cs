namespace Digital5HP.Text.M3U;

public class Channel
{
    public string MediaUrl { get; set; }

    public string TvgId { get; set; }

    public string TvgName { get; set; }

    public string LogoUrl { get; set; }

    public string GroupTitle { get; set; }

    public int Duration { get; set; } = -1;

    public string Title { get; set; }
}
