namespace Digital5HP.Identity.Tests.Unit;

using System.ComponentModel.DataAnnotations;
using System.IO;

using Digital5HP.Test;
using Digital5HP.Text.M3U;

using FluentAssertions;

using Xunit;

[Trait("Category", "Unit")]
public class SerializerTests : FixtureBase
{
    [Fact]
    public void Deserialize_Succeed()
    {
        // Arrange
        var testFile = File.ReadAllText("./test.m3u");

        // Act
        var result = Serializer.Deserialize(testFile);

        // Assert
        result.Should()
              .NotBeNull();

        result.EndList.Should()
              .BeTrue();
        result.Version.Should()
              .Be(1);
        result.TargetDuration.Should()
              .Be(-1);
        result.MediaSequence.Should()
              .Be(0);
        result.PlaylistType.Should()
              .Be("VOD");

        result.Channels.Should()
            .HaveCount(6);
    }

    [Fact]
    public void SerializeAttributes_Succeed()
    {
        // Arrange
        const string EXPECTED_RESULT = @"#EXTM3U
#EXTINF:-1 tvg-id=""channel1"" tvg-name=""Some Channel Name"" tvg-logo=""http://logo.com/wow.png"" group-title=""Cool Group"",Some Channel Name
http://my.channel.com/1234
#EXTINF:-1 tvg-id=""channel2"" tvg-name=""Yet Another Channel"" tvg-logo=""http://logo.com/wow.png"" group-title=""Cool Group"",Yet Another Channel
http://my.channel.com/4321
#EXTINF:-1 tvg-id="""" tvg-name=""Yet Another Channel, Again"" tvg-logo="""" group-title="""",Yet Another Channel, Again
http://my.channel.com/4444
#EXTINF:-1 tvg-name=""Slim Channel"",Slim Channel
http://my.channel.com/6789
#EXTINF:-1,
http://my.channel.com/0000
";
        var document = new Document();
        document.Channels.Add(new Channel()
        {
            TvgId = "channel1",
            TvgName = "Some Channel Name",
            LogoUrl = "http://logo.com/wow.png",
            GroupTitle = "Cool Group",
            Title = "Some Channel Name",
            MediaUrl = "http://my.channel.com/1234"
        });
        document.Channels.Add(new Channel()
        {
            TvgId = "channel2",
            TvgName = "Yet Another Channel",
            LogoUrl = "http://logo.com/wow.png",
            GroupTitle = "Cool Group",
            Title = "Yet Another Channel",
            MediaUrl = "http://my.channel.com/4321"
        });
        document.Channels.Add(new Channel()
        {
            TvgId = "",
            TvgName = "Yet Another Channel, Again",
            LogoUrl = "",
            GroupTitle = "",
            Title = "Yet Another Channel, Again",
            MediaUrl = "http://my.channel.com/4444"
        });
        document.Channels.Add(new Channel()
        {
            TvgName = "Slim Channel",
            Title = "Slim Channel",
            MediaUrl = "http://my.channel.com/6789"
        });
        document.Channels.Add(new Channel()
        {
            MediaUrl = "http://my.channel.com/0000"
        });

        // Act
        var result = Serializer.Serialize(document);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
            .Be(EXPECTED_RESULT);
    }

    [Fact]
    public void SerializeTags_Succeed()
    {
        // Arrange
        const string EXPECTED_RESULT = @"#EXTM3U
#EXT-X-PLAYLIST-TYPE:VOD
#EXT-X-TARGETDURATION:-1
#EXT-X-VERSION:1
#EXT-X-MEDIA-SEQUENCE:0
#EXTINF:-1 tvg-id=""channel0"" tvg-name=""Tagged Channel"",Tagged Channel
#EXTGRP:Cool Group
#EXTIMG:http://logo.com/wow.png
#PLAYLIST:Tagged Channel
http://my.channel.com/9898
#EXT-X-ENDLIST
";
        var document = new Document()
        {
            Version = 1,
            TargetDuration = -1,
            MediaSequence = 0,
            PlaylistType = "VOD",
            EndList = true,
        };

        document.Channels.Add(new Channel()
        {
            TvgId = "channel0",
            TvgName = "Tagged Channel",
            LogoUrl = "http://logo.com/wow.png",
            GroupTitle = "Cool Group",
            Title = "Tagged Channel",
            MediaUrl = "http://my.channel.com/9898"
        });

        // Act
        var result = Serializer.Serialize(document, options => options.ChannelFormat = ChannelFormatType.Tags);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
            .Be(EXPECTED_RESULT);
    }
}