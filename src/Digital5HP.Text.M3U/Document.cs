namespace Digital5HP.Text.M3U;

using System.Collections.Generic;

public class Document
{
    public string PlaylistType { get; set; }

    public int? TargetDuration { get; set; }

    public int? Version { get; set; }

    public int? MediaSequence { get; set; }

    public IList<Channel> Channels { get; } = [];

    public bool EndList { get; set; }
}
