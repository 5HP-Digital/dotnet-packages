namespace Digital5HP.Text.M3U;

public class SerializationOptions
{
    internal static SerializationOptions Default => new();

    public SerializationOptions()
    {
        this.ChannelFormat = ChannelFormatType.Attributes;
    }

    public ChannelFormatType ChannelFormat { get; set; }
}
