using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;

namespace DiscordWebhook
{
    public class DiscordMessage
    {
        public DiscordMessage()
        {
            Embeds = new List<DiscordEmbed>();
        }

        [JsonPropertyName("content")]
        /// <summary>
        /// Message content
        /// </summary>
        public string Content { get; set; }

        [JsonPropertyName("tts")]
        /// <summary>
        /// Read message to everyone on the channel
        /// </summary>
        public bool TTS { get; set; }

        [JsonPropertyName("username")]
        /// <summary>
        /// Webhook profile username to be shown
        /// </summary>
        public string Username { get; set; }

        [JsonPropertyName("avatar_url")]
        /// <summary>
        /// Webhook profile avater to be shown
        /// </summary>
        public string AvatarUrl { get; set; }

        [JsonPropertyName("embeds")]
        /// <summary>
        /// List of embeds
        /// </summary>
        public List<DiscordEmbed> Embeds { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("allowed_mentions")]
        /// <summary>
        /// Allowed mentions for this message
        /// </summary>
        public AllowedMentions AllowedMentions { get; set; }
    }

    public class DiscordEmbed
    {
        public DiscordEmbed()
        {
            Fields = new List<EmbedField>();
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("title")]
        /// <summary>
        /// Embed title
        /// </summary>
        public string Title { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("description")]
        /// <summary>
        /// Embed description
        /// </summary>
        public string Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("url")]
        /// <summary>
        /// Embed url
        /// </summary>
        public string Url { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        /// <summary>
        /// Embed timestamp
        /// </summary>
        public DateTime? Timestamp
        {
            get => DateTime.Parse(StringTimestamp);
            set => StringTimestamp = value?.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("timestamp")]
        public string StringTimestamp { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        /// <summary>
        /// Embed color
        /// </summary>
        public Color? Color
        {
            get => HexColor.ToColor();
            set => HexColor = value.ToHex();
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("color")]
        public int? HexColor { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("footer")]
        /// <summary>
        /// Embed footer
        /// </summary>
        public EmbedFooter Footer { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("image")]
        /// <summary>
        /// Embed image
        /// </summary>
        public EmbedMedia Image { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("thumbnail")]
        /// <summary>
        /// Embed thumbnail
        /// </summary>
        public EmbedMedia Thumbnail { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("video")]
        /// <summary>
        /// Embed video
        /// </summary>
        public EmbedMedia Video { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("provider")]
        /// <summary>
        /// Embed provider
        /// </summary>
        public EmbedProvider Provider { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("author")]
        /// <summary>
        /// Embed author
        /// </summary>
        public EmbedAuthor Author { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("fields")]
        /// <summary>
        /// Embed fields list
        /// </summary>
        public List<EmbedField> Fields { get; set; }
    }

    public class EmbedFooter
    {
        [JsonPropertyName("text")]
        /// <summary>
        /// Footer text
        /// </summary>
        public string Text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("icon_url")]
        /// <summary>
        /// Footer icon
        /// </summary>
        public string IconUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("proxy_icon_url")]
        /// <summary>
        /// Footer icon proxy
        /// </summary>
        public string ProxyIconUrl { get; set; }
    }

    public class EmbedMedia
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("url")]
        /// <summary>
        /// Media url
        /// </summary>
        public string Url { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("proxy_url")]
        /// <summary>
        /// Media proxy url
        /// </summary>
        public string ProxyUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("height")]
        /// <summary>
        /// Media height
        /// </summary>
        public int? Height { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("width")]
        /// <summary>
        /// Media width
        /// </summary>
        public int? Width { get; set; }
    }

    public class EmbedProvider
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("name")]
        /// <summary>
        /// Provider name
        /// </summary>
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("url")]
        /// <summary>
        /// Provider url
        /// </summary>
        public string Url { get; set; }
    }

    public class EmbedAuthor
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("name")]
        /// <summary>
        /// Author name
        /// </summary>
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("url")]
        /// <summary>
        /// Author url
        /// </summary>
        public string Url { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("icon_url")]
        /// <summary>
        /// Author icon
        /// </summary>
        public string IconUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("proxy_icon_url")]
        /// <summary>
        /// Author icon proxy
        /// </summary>
        public string ProxyIconUrl { get; set; }
    }

    public class EmbedField
    {
        [JsonPropertyName("name")]
        /// <summary>
        /// Field name
        /// </summary>
        public string Name { get; set; }

        [JsonPropertyName("value")]
        /// <summary>
        /// Field value
        /// </summary>
        public string Value { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("inline")]
        /// <summary>
        /// Field align
        /// </summary>
        public bool? InLine { get; set; }
    }

    public class AllowedMentions
    {
        [JsonPropertyName("parse")]
        /// <summary>
        /// List of allowd mention types to parse from the content
        /// </summary>
        public List<string> Parse { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("roles")]
        /// <summary>
        /// List of role_ids to mention
        /// </summary>
        public List<ulong> Roles { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("users")]
        /// <summary>
        /// List of user_ids to mention
        /// </summary>
        public List<ulong> Users { get; set; }
    }
}