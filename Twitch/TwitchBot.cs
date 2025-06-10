using AsyncAwaitBestPractices;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Twitch
{
    public class TwitchBot : IDisposable
    {
        const string ip = "irc.chat.twitch.tv";
        const int port = 6667;
        TcpClient tcpClient;

        readonly string password;
        readonly string botUsername;
        readonly string channel;

        private StreamReader streamReader;
        private StreamWriter streamWriter;
        private readonly TaskCompletionSource<int> connected = new();

        public event TwitchChatEventHandler OnMessage = delegate { };
        public delegate void TwitchChatEventHandler(object sender, TwitchChatMessage e);

        private readonly string DropItemCommand;
        private readonly string DropRecipeCommand;

        private readonly bool commandMode;

        private bool stop;

        public static readonly string csvFolder = @"csv\";
        public static readonly string itemFile = @"items.csv";
        public static readonly string itemPath = csvFolder + itemFile;
        public static readonly string recipeFile = @"recipes.csv";
        public static readonly string recipePath = csvFolder + recipeFile;

        public static readonly string saveFolder = @"save\";
        public static readonly string TwitchSettingFile = @"twitch.json";
        public static readonly string TwitchSettingPath = saveFolder + TwitchSettingFile;

        public class TwitchChatMessage : EventArgs
        {
            public string Sender { get; set; }
            public string Message { get; set; }
            public string Channel { get; set; }
        }

        public TwitchBot(string TwitchBotUserName, string TwitchBotOauth, string TwitchChannelName)
        {
            var allowCommand = GetJsonSetting(TwitchSettingPath, "AllowCommand");
            DropItemCommand = GetJsonSetting(TwitchSettingPath, "DropItemCommand");
            DropRecipeCommand = GetJsonSetting(TwitchSettingPath, "DropRecipeCommand");

            if (allowCommand.ToLower().Equals("true"))
            {
                commandMode = true;
            }

            botUsername = TwitchBotUserName;
            password = TwitchBotOauth;
            channel = TwitchChannelName;

            Connect().SafeFireAndForget();
        }

        private async Task Connect()
        {
            Start().SafeFireAndForget();
            //We could .SafeFireAndForget() these two calls if we want to
            await JoinChannel(channel);
            if (commandMode)
                await SendMessage(channel, "Chat bot has started up! (Accept Commands !hey)");
            else
                await SendMessage(channel, "Chat bot has started up!");

            OnMessage += async (_, twitchChatMessage) =>
            {
                //Console.WriteLine($"{twitchChatMessage.Sender} said '{twitchChatMessage.Message}'");
                //Listen for !hey command
                if (twitchChatMessage.Message.StartsWith("!hey"))
                {
                    await SendMessage(twitchChatMessage.Channel, $"Hey there {twitchChatMessage.Sender}");
                }

                if (commandMode)
                {
                    string message;
                    if (twitchChatMessage.Message.StartsWith(DropItemCommand))
                    {
                        message = twitchChatMessage.Message.Replace(DropItemCommand, "").Replace('’', '\'').Trim();
                        //Console.WriteLine(message);
                        if (message.Equals(string.Empty))
                        {
                            await SendMessage(twitchChatMessage.Channel, $"Hey {twitchChatMessage.Sender}, you miss the item name!");
                        }
                        else
                        {
                            string name = "";
                            string num = "0";

                            if (message.Contains(','))
                            {
                                string[] temp = message.Split(',');
                                if (temp.Length >= 2)
                                {
                                    name = temp[0].Trim();
                                    num = temp[temp.Length - 1].Trim();
                                }
                            }
                            else
                            {
                                name = message;
                            }

                            await PubSub.CheckAndAddItem(name, num, twitchChatMessage.Sender);
                        }
                    }
                    else if (twitchChatMessage.Message.StartsWith(DropRecipeCommand))
                    {
                        message = twitchChatMessage.Message.Replace(DropRecipeCommand, "").Replace('’', '\'').Trim();
                        //Console.WriteLine(message);

                        if (message.Equals(string.Empty))
                        {
                            await SendMessage(twitchChatMessage.Channel, $"Hey {twitchChatMessage.Sender}, you miss the recipe name!");
                        }
                        else
                        {
                            await PubSub.CheckAndAddRecipe(message, twitchChatMessage.Sender);
                        }
                    }
                }
            };

            await Task.Delay(-1);
        }

        public async Task Start()
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);
            streamReader = new StreamReader(tcpClient.GetStream());
            streamWriter = new StreamWriter(tcpClient.GetStream()) { NewLine = "\r\n", AutoFlush = true };

            await streamWriter.WriteLineAsync($"PASS {password}");
            await streamWriter.WriteLineAsync($"NICK {botUsername}");
            connected.SetResult(0);

            while (true)
            {
                if (stop)
                    break;

                string line = await streamReader.ReadLineAsync();
                Console.WriteLine(line);

                if (line == null)
                    continue;

                string[] split = line.Split(' ');
                //PING :tmi.twitch.tv
                //Respond with PONG :tmi.twitch.tv
                if (line.StartsWith("PING"))
                {
                    Console.WriteLine("PONG");
                    await streamWriter.WriteLineAsync($"PONG {split[1]}");
                }

                if (split.Length > 2 && split[1] == "PRIVMSG")
                {
                    //:mytwitchchannel!mytwitchchannel@mytwitchchannel.tmi.twitch.tv 
                    // ^^^^^^^^
                    //Grab this name here
                    int exclamationPointPosition = split[0].IndexOf('!');
                    string username = split[0].Substring(1, exclamationPointPosition - 1);
                    //Skip the first character, the first colon, then find the next colon
                    int secondColonPosition = line.IndexOf(':', 1);//the 1 here is what skips the first character
                    string message = line.Substring(secondColonPosition + 1);//Everything past the second colon
                    string channel = split[2].TrimStart('#');

                    OnMessage(this, new TwitchChatMessage
                    {
                        Message = message,
                        Sender = username,
                        Channel = channel
                    });
                }
            }
        }

        public async Task SendMessage(string channel, string message)
        {
            await connected.Task;
            await streamWriter.WriteLineAsync($"PRIVMSG #{channel} :{message}");
        }

        public async Task SendMessage(string message)
        {
            await connected.Task;
            await streamWriter.WriteLineAsync($"PRIVMSG #{channel} :{message}");
        }

        public async Task JoinChannel(string channel)
        {
            await connected.Task;
            await streamWriter.WriteLineAsync($"JOIN #{channel}");
        }

        public void Dispose()
        {
            stop = true;
            GC.SuppressFinalize(this);
            streamReader.Close();
            streamWriter.Close();
            tcpClient.Close();
        }

        public static string GetJsonSetting(string path, string key)
        {
            JObject o = JObject.Parse(File.ReadAllText(path));
            var value = o.SelectToken(key);
            if (value == null)
                return string.Empty;
            return value.ToString();
        }
    }
}
