namespace Digital5HP.Text.M3U;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public static partial class Serializer
{
    private const string M3U_HEADER_TAG = "EXTM3U";
    private const string M3U_VERSION_TAG = "EXT-X-VERSION";
    private const string M3U_CHANNEL_TAG = "EXTINF";
    private const string M3U_CHANNEL_GROUP_TAG = "EXTGRP";
    private const string M3U_CHANNEL_PLAYLIST_TAG = "PLAYLIST";
    private const string M3U_CHANNEL_LOGO_TAG = "EXTIMG";
    private const string M3U_TARGET_DURATION_TAG = "EXT-X-TARGETDURATION";
    private const string M3U_MEDIA_SEQUENCE_TAG = "EXT-X-MEDIA-SEQUENCE";
    private const string M3U_END_LIST_TAG = "EXT-X-ENDLIST";
    private const string M3U_PLAYLIST_TYPE_TAG = "EXT-X-PLAYLIST-TYPE";

    private const string M3U_CHANNEL_TVG_ID_ATTRIBUTE = "tvg-id";
    private const string M3U_CHANNEL_TVG_NAME_ATTRIBUTE = "tvg-name";
    private const string M3U_CHANNEL_TVG_LOGO_ATTRIBUTE = "tvg-logo";
    private const string M3U_CHANNEL_TVG_CHNO_ATTRIBUTE = "tvg-chno";
    private const string M3U_CHANNEL_GROUP_ATTRIBUTE = "group-title";
    private const string M3U_CHANNEL_ID_ATTRIBUTE = "channel-id";
    private const string M3U_CHANNEL_NUMBER_ATTRIBUTE = "channel-number";

    private static readonly Regex TagRegex = CreateTagRegex();
    private static readonly Regex ExtinfAttributesRegex = CreateExtinfAttributesRegex();
    private static readonly Regex ExtinfTitleRegex = CreateExtinfTitleRegex();

    private static readonly Dictionary<PlaylistType, string> PlaylistTypeDictionary = new() { { PlaylistType.Event, "EVENT" }, { PlaylistType.VOD, "VOD" } };

    public static Document Deserialize(Stream stream)
    {
        using var sr = new StreamReader(stream, leaveOpen: true);

        var text = sr.ReadToEnd();

        return Deserialize(text);
    }

    public static async Task<Document> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var sr = new StreamReader(stream, leaveOpen: true);

        return await DeserializeInternalAsync(sr);
    }


    public static Document Deserialize(string text)
    {
        using var sr = new StringReader(text);

        return AsyncHelper.RunSync(() => DeserializeInternalAsync(sr));
    }

    public static string Serialize(Document document, Action<SerializationOptions> options = null)
    {
        using var ms = new MemoryStream();

        AsyncHelper.RunSync(() => SerializeInternalAsync(document, ms, options));

        ms.Position = 0;
        using var sr = new StreamReader(ms);

        return sr.ReadToEnd();
    }

    public static void Serialize(Document document, Stream stream, Action<SerializationOptions> options = null)
    {
        AsyncHelper.RunSync(() => SerializeInternalAsync(document, stream, options));
    }

    public static Task SerializeAsync(Document document, Stream stream, Action<SerializationOptions> options = null)
    {
        return SerializeInternalAsync(document, stream, options);
    }

    private static async Task SerializeInternalAsync(Document document, Stream stream, Action<SerializationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(stream);

        var serializationOptions = SerializationOptions.Default;
        options?.Invoke(serializationOptions);

        await using var writer = new StreamWriter(stream, leaveOpen: true);

        await writer.WriteLineAsync($"#{M3U_HEADER_TAG}");

        if (document.PlaylistType != null)
            await writer.WriteLineAsync($"#{M3U_PLAYLIST_TYPE_TAG}:{document.PlaylistType}");
        if (document.TargetDuration != null)
            await writer.WriteLineAsync($"#{M3U_TARGET_DURATION_TAG}:{document.TargetDuration}");
        if (document.Version != null)
            await writer.WriteLineAsync($"#{M3U_VERSION_TAG}:{document.Version}");
        if (document.MediaSequence != null)
            await writer.WriteLineAsync($"#{M3U_MEDIA_SEQUENCE_TAG}:{document.MediaSequence}");

        foreach (var channel in document.Channels)
        {
            await writer.WriteAsync($"#{M3U_CHANNEL_TAG}:{channel.Duration}");

            if (channel.TvgId != null)
                await writer.WriteAsync($" {M3U_CHANNEL_TVG_ID_ATTRIBUTE}=\"{channel.TvgId}\"");
            if (channel.TvgName != null)
                await writer.WriteAsync($" {M3U_CHANNEL_TVG_NAME_ATTRIBUTE}=\"{channel.TvgName}\"");
            if (channel.TvgChannelNumber != null)
                await writer.WriteAsync($" {M3U_CHANNEL_TVG_CHNO_ATTRIBUTE}=\"{channel.TvgChannelNumber}\"");

            if (serializationOptions.ChannelFormat == ChannelFormatType.Attributes)
            {
                if (channel.LogoUrl != null)
                    await writer.WriteAsync($" {M3U_CHANNEL_TVG_LOGO_ATTRIBUTE}=\"{channel.LogoUrl}\"");
                if (channel.GroupTitle != null)
                    await writer.WriteAsync($" {M3U_CHANNEL_GROUP_ATTRIBUTE}=\"{channel.GroupTitle}\"");
            }

            if (channel.ChannelId != null)
                await writer.WriteAsync($" {M3U_CHANNEL_ID_ATTRIBUTE}=\"{channel.ChannelId}\"");
            if (channel.ChannelNumber != null)
                await writer.WriteAsync($" {M3U_CHANNEL_NUMBER_ATTRIBUTE}=\"{channel.ChannelNumber}\"");

            await writer.WriteLineAsync($",{channel.Title ?? ""}");

            if (serializationOptions.ChannelFormat == ChannelFormatType.Tags)
            {
                if (channel.GroupTitle != null)
                    await writer.WriteLineAsync($"#{M3U_CHANNEL_GROUP_TAG}:{channel.GroupTitle}");
                if (channel.LogoUrl != null)
                    await writer.WriteLineAsync($"#{M3U_CHANNEL_LOGO_TAG}:{channel.LogoUrl}");
                if (channel.Title != null)
                    await writer.WriteLineAsync($"#{M3U_CHANNEL_PLAYLIST_TAG}:{channel.Title}");
            }

            await writer.WriteLineAsync(channel.MediaUrl);
        }

        if (document.EndList)
            await writer.WriteLineAsync($"#{M3U_END_LIST_TAG}");

        await writer.FlushAsync();
    }


    private static async Task<Document> DeserializeInternalAsync(TextReader reader)
    {
        var doc = new Document();

        var line = await reader.ReadLineAsync();

        if (!TryParseLine(line, out var tagPair) || tagPair.Tag != M3U_HEADER_TAG || tagPair.Value != null)
            throw new SerializationException("Invalid Extended M3U Playlist format: EXTM3U tag expected or invalid.");

        while ((line = await reader.ReadLineAsync()) != null)
        {
            // ignore blank lines
            if (line.IsNullOrEmpty())
                continue;

            // parse line
            if (!TryParseLine(line, out tagPair))
            {
                throw new SerializationException($"Invalid Extended M3U Playlist format: invalid text in '{line}'");
            }

            switch (tagPair.Tag)
            {
                case M3U_PLAYLIST_TYPE_TAG:
                    if (!PlaylistTypeDictionary.Values.Contains(tagPair.Value, StringComparer.Ordinal))
                        throw new SerializationException($"Invalid Extended M3U Playlist format: invalid value for tag '{tagPair.Tag}'");
                    doc.PlaylistType = tagPair.Value;
                    break;
                case M3U_VERSION_TAG:
                    {
                        if (!int.TryParse(tagPair.Value, CultureInfo.InvariantCulture, out var val))
                            throw new SerializationException($"Invalid Extended M3U Playlist format: invalid value for tag '{tagPair.Tag}'");
                        doc.Version = val;
                    }
                    break;
                case M3U_TARGET_DURATION_TAG:
                    {
                        if (!int.TryParse(tagPair.Value, CultureInfo.InvariantCulture, out var val))
                            throw new SerializationException($"Invalid Extended M3U Playlist format: invalid value for tag '{tagPair.Tag}'");
                        doc.TargetDuration = val;
                    }
                    break;
                case M3U_MEDIA_SEQUENCE_TAG:
                    {
                        if (!int.TryParse(tagPair.Value, CultureInfo.InvariantCulture, out var val))
                            throw new SerializationException($"Invalid Extended M3U Playlist format: invalid value for tag '{tagPair.Tag}'");
                        doc.MediaSequence = val;
                    }
                    break;
                case M3U_END_LIST_TAG:
                    if (tagPair.Value != null)
                        throw new SerializationException($"Invalid Extended M3U Playlist format: unexpected value for tag '{tagPair.Tag}'");
                    doc.EndList = true;
                    break;
                case M3U_CHANNEL_TAG:
                    {
                        // parse channel (multiple lines)
                        var channel = new Channel();

                        if (!TryParseExtinfAttributes(tagPair.Value, channel))
                            throw new SerializationException($"Invalid Extended M3U Playlist format: invalid value for tag '{tagPair.Tag}'");

                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            if (TryParseLine(line, out var channelTagPair))
                            {
                                switch (channelTagPair.Tag)
                                {
                                    case M3U_CHANNEL_GROUP_TAG:
                                        channel.GroupTitle = channelTagPair.Value;
                                        break;
                                    case M3U_CHANNEL_PLAYLIST_TAG:
                                        channel.Title = channelTagPair.Value;
                                        break;
                                    case M3U_CHANNEL_LOGO_TAG:
                                        channel.LogoUrl = channelTagPair.Value;
                                        break;
                                    default:
                                        throw new SerializationException($"Invalid Extended M3U Playlist format: unknown channel tag '{tagPair.Tag}' or missing media URL");
                                }
                            }
                            else if (Uri.IsWellFormedUriString(line, UriKind.RelativeOrAbsolute))
                            {
                                channel.MediaUrl = line;
                                break;
                            }
                            else if (!line.IsNullOrEmpty())
                            {
                                throw new SerializationException($"Invalid Extended M3U Playlist format: malformed channel; invalid text in '{line}'");
                            }
                        }

                        doc.Channels.Add(channel);
                    }
                    break;
                default:
                    throw new SerializationException($"Invalid Extended M3U Playlist format: unknown tag '{tagPair.Tag}'");
            }
        }

        return doc;
    }

    private static bool TryParseLine(string line, out (string Tag, string Value) tagPair)
    {
        var match = TagRegex.Match(line);
        tagPair = (match.Groups["key"].Success
            ? match.Groups["key"].Value
            : null,
            match.Groups["value"].Success
            ? match.Groups["value"].Value
            : null);
        return match.Success;
    }

    private static bool TryParseExtinfAttributes(string value, Channel channel)
    {
        ArgumentNullException.ThrowIfNull(channel);

        if (value == null)
            return false;

        var attributes = ExtinfAttributesRegex.Matches(value)
            .Cast<Match>()
            .ToDictionary(match => match.Groups["attr"].Value, match => match.Groups["value"].Value, StringComparer.OrdinalIgnoreCase);

        channel.TvgId = attributes.GetValueOrDefault(M3U_CHANNEL_TVG_ID_ATTRIBUTE, channel.TvgId);
        channel.TvgName = attributes.GetValueOrDefault(M3U_CHANNEL_TVG_NAME_ATTRIBUTE, channel.TvgName);
        channel.TvgChannelNumber = int.TryParse(
            attributes.GetValueOrDefault(M3U_CHANNEL_TVG_CHNO_ATTRIBUTE, channel.TvgChannelNumber?.ToString(CultureInfo.InvariantCulture)),
            CultureInfo.InvariantCulture,
            out var tvgChno)
            ? tvgChno
            : null;
        channel.LogoUrl = attributes.GetValueOrDefault(M3U_CHANNEL_TVG_LOGO_ATTRIBUTE, channel.LogoUrl);
        channel.GroupTitle = attributes.GetValueOrDefault(M3U_CHANNEL_GROUP_ATTRIBUTE, channel.GroupTitle);
        channel.ChannelId = attributes.GetValueOrDefault(M3U_CHANNEL_ID_ATTRIBUTE, channel.ChannelId);
        channel.ChannelNumber = int.TryParse(
            attributes.GetValueOrDefault(M3U_CHANNEL_NUMBER_ATTRIBUTE, defaultValue: channel.ChannelNumber?.ToString(CultureInfo.InvariantCulture)),
            CultureInfo.InvariantCulture,
            out var channelNum)
            ? channelNum
            : null;
        if (!int.TryParse(value.Split(' ', ',')[0], CultureInfo.InvariantCulture, out var duration))
            throw new SerializationException($"Invalid Extended M3U Playlist format: invalid value for tag '{M3U_CHANNEL_TAG}'");
        channel.Duration = duration;

        var match = ExtinfTitleRegex.Match(value);
        if (match.Success)
            channel.Title = match.Groups["title"].Value;

        return true;
    }

    [GeneratedRegex("^#(?<key>[a-zA-Z][a-zA-Z0-9-]+)(:(?<value>.+))?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex CreateTagRegex();

    [GeneratedRegex("(?<=\\s|^)(?<attr>[a-z-]+)=\"(?<value>[^\"]+?)\"", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex CreateExtinfAttributesRegex();

    [GeneratedRegex("^([a-zA-Z0-9\\-]+)(?> ([a-zA-Z0-9\\-]+=\"[^\"]*\"))*?,(?<title>.*)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex CreateExtinfTitleRegex();
}

