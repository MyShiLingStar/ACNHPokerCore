using System;
using System.IO;
using System.Net;
using System.Text;

namespace Discord.Webhook
{
    public class DiscordWebhook
    {
        /// <summary>
        /// Webhook url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Send webhook message
        /// </summary>
        public void Send(DiscordMessage message, FileInfo file = null)
        {
            string bound = "------------------------" + DateTime.Now.Ticks.ToString("x");
            WebClient webhookRequest = new WebClient();
            webhookRequest.Headers.Add("Content-Type", "multipart/form-data; boundary=" + bound);
            MemoryStream stream = new MemoryStream();
            byte[] beginBodyBuffer = Encoding.UTF8.GetBytes("--" + bound + "\r\n");
            stream.Write(beginBodyBuffer, 0, beginBodyBuffer.Length);
            bool flag = file != null && file.Exists;
            if (flag)
            {
                string fileBody = "Content-Disposition: form-data; name=\"file\"; filename=\"" + file.Name + "\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                byte[] fileBodyBuffer = Encoding.UTF8.GetBytes(fileBody);
                stream.Write(fileBodyBuffer, 0, fileBodyBuffer.Length);
                byte[] fileBuffer = File.ReadAllBytes(file.FullName);
                stream.Write(fileBuffer, 0, fileBuffer.Length);
                string fileBodyEnd = "\r\n--" + bound + "\r\n";
                byte[] fileBodyEndBuffer = Encoding.UTF8.GetBytes(fileBodyEnd);
                stream.Write(fileBodyEndBuffer, 0, fileBodyEndBuffer.Length);
            }
            string jsonBody = string.Concat(new string[]
            {
                "Content-Disposition: form-data; name=\"payload_json\"\r\nContent-Type: application/json\r\n\r\n",
                string.Format("{0}\r\n", message),
                "--",
                bound,
                "--"
            });
            byte[] jsonBodyBuffer = Encoding.UTF8.GetBytes(jsonBody);
            stream.Write(jsonBodyBuffer, 0, jsonBodyBuffer.Length);
            webhookRequest.UploadData(this.Url, stream.ToArray());
        }
    }
}
