using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordWebhook
{
    public class DiscordWebhook
    {
        /// <summary>
        /// Webhook url
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Send webhook message
        /// </summary>
        public async Task SendAsync(DiscordMessage message, params FileInfo[] files)
        {
            using var httpClient = new HttpClient();

            string bound = "------------------------" + DateTime.Now.Ticks.ToString("x");

            var httpContent = new MultipartFormDataContent(bound);

            foreach (var file in files)
            {
                var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(file.FullName));
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                httpContent.Add(fileContent, file.Name, file.Name);
            }

            var jsonContent = new StringContent(JsonSerializer.Serialize(message));
            jsonContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            httpContent.Add(jsonContent, "payload_json");

            var response = await httpClient.PostAsync(Uri, httpContent);
            if (!response.IsSuccessStatusCode)
            {
                throw new DiscordException(await response.Content.ReadAsStringAsync());
            }
        }
    }
}