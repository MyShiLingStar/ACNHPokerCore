using Newtonsoft.Json;
using System;
using System.Drawing;

namespace Discord
{

    /// <summary>
    /// Discord message data object
    /// </summary>
    public struct DiscordMessage
    {
        /// <summary>
        /// Message content
        /// </summary>
        public string Content;

        /// <summary>
        /// Read message to everyone on the channel
        /// </summary>
        public bool TTS;

        /// <summary>
        /// Webhook profile username to be shown
        /// </summary>
        public string Username;

        /// <summary>
        /// Webhook profile avater to be shown
        /// </summary>
        public string AvatarUrl;

        /// <summary>
        /// Array of embeds
        /// </summary>
        public DiscordEmbed[] Embeds;

        public override string ToString() => DiscordUtils.StructToJson(this).ToString(Formatting.None);
    }

    /// <summary>
    /// Discord embed data object
    /// </summary>
    public struct DiscordEmbed
    {
        /// <summary>
        /// Embed title
        /// </summary>
        public string Title;

        /// <summary>
        /// Embed description
        /// </summary>
        public string Description;

        /// <summary>
        /// Embed url
        /// </summary>
        public string Url;

        /// <summary>
        /// Embed timestamp
        /// </summary>
        public DateTime Timestamp;

        /// <summary>
        /// Embed color
        /// </summary>
        public Color Color;

        /// <summary>
        /// Embed footer
        /// </summary>
        public EmbedFooter Footer;

        /// <summary>
        /// Embed image
        /// </summary>
        public EmbedMedia Image;

        /// <summary>
        /// Embed thumbnail
        /// </summary>
        public EmbedMedia Thumbnail;

        /// <summary>
        /// Embed video
        /// </summary>
        public EmbedMedia Video;

        /// <summary>
        /// Embed provider
        /// </summary>
        public EmbedProvider Provider;

        /// <summary>
        /// Embed author
        /// </summary>
        public EmbedAuthor Author;

        /// <summary>
        /// Embed fields array
        /// </summary>
        public EmbedField[] Fields;

        public override string ToString() => DiscordUtils.StructToJson(this).ToString(Formatting.None);
    }

    /// <summary>
    /// Discord embed footer data object
    /// </summary>
    public struct EmbedFooter
    {
        /// <summary>
        /// Footer text
        /// </summary>
        public string Text;

        /// <summary>
        /// Footer icon
        /// </summary>
        public string IconUrl;

        /// <summary>
        /// Footer icon proxy
        /// </summary>
        public string ProxyIconUrl;

        public override string ToString() => DiscordUtils.StructToJson(this).ToString(Formatting.None);
    }

    /// <summary>
    /// Discord embed media data object (images/thumbs/videos)
    /// </summary>
    public struct EmbedMedia
    {
        /// <summary>
        /// Media url
        /// </summary>
        public string Url;

        /// <summary>
        /// Media proxy url
        /// </summary>
        public string ProxyUrl;

        /// <summary>
        /// Media height
        /// </summary>
        public int Height;

        /// <summary>
        /// Media width
        /// </summary>
        public int Width;

        public override string ToString() => DiscordUtils.StructToJson(this).ToString(Formatting.None);
    }

    /// <summary>
    /// Discord embed provider data object
    /// </summary>
    public struct EmbedProvider
    {
        /// <summary>
        /// Provider name
        /// </summary>
        public string Name;

        /// <summary>
        /// Provider url
        /// </summary>
        public string Url;

        public override string ToString() => DiscordUtils.StructToJson(this).ToString(Formatting.None);
    }

    /// <summary>
    /// Discord embed author data object
    /// </summary>
    public struct EmbedAuthor
    {
        /// <summary>
        /// Author name
        /// </summary>
        public string Name;

        /// <summary>
        /// Author url
        /// </summary>
        public string Url;

        /// <summary>
        /// Author icon
        /// </summary>
        public string IconUrl;

        /// <summary>
        /// Author icon proxy
        /// </summary>
        public string ProxyIconUrl;

        public override string ToString() => DiscordUtils.StructToJson(this).ToString(Formatting.None);
    }

    /// <summary>
    /// Discord embed field data object
    /// </summary>
    public struct EmbedField
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string Name;

        /// <summary>
        /// Field value
        /// </summary>
        public string Value;

        /// <summary>
        /// Field align
        /// </summary>
        public bool InLine;

        public override string ToString() => DiscordUtils.StructToJson(this).ToString(Formatting.None);
    }
}
