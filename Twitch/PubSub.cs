using AsyncAwaitBestPractices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Interfaces;

namespace Twitch
{
    public class PubSub : IDisposable
    {
        public static ITwitchPubSub MyPubSub;

        private static TwitchBot MyTwitchBot;

        private static DataTable itemSource;
        private static DataTable recipeSource;
        private static Dictionary<string, string> ItemDict;
        private static Dictionary<string, string> ReverseDict;
        private static Dictionary<string, string> ColorDict;
        private static Dictionary<string, string> RecipeDict;
        private static Dictionary<string, string> VillagerDict;

        private string channelId;
        private string accessToken;

        private string DropItemRewardId;
        private string DropRecipeRewardId;
        private string InjectVillagerRewardId;

        private static RichTextBox LogBox;

        public static List<ItemOrder> DropOrderList = new();
        public static List<VillagerOrder> VillagerOrderList = new();

        public PubSub(TwitchBot TwitchBot, string TwitchChannelid, string TwitchChannelAccessToken, ref RichTextBox log)
        {
            channelId = TwitchChannelid;
            accessToken = TwitchChannelAccessToken;
            DropItemRewardId = TwitchBot.GetJsonSetting(TwitchBot.TwitchSettingPath, "DropItemRewardId");
            DropRecipeRewardId = TwitchBot.GetJsonSetting(TwitchBot.TwitchSettingPath, "DropRecipeRewardId");
            InjectVillagerRewardId = TwitchBot.GetJsonSetting(TwitchBot.TwitchSettingPath, "InjectVillagerRewardId");

            MyTwitchBot = TwitchBot;

            LogBox = log;

            if (File.Exists(TwitchBot.itemPath))
            {
                itemSource = loadItemCSV(TwitchBot.itemPath);
                recipeSource = loadItemCSV(TwitchBot.recipePath);
                ItemDict = CreateItemDictionary(TwitchBot.itemPath); // Name -> ID
                ReverseDict = CreateReverseDictionary(TwitchBot.itemPath); // ID -> Name
                ColorDict = CreateColorDictionary(TwitchBot.itemPath); // ID -> Color
                RecipeDict = CreateRecipeDictionary(TwitchBot.recipePath); // ID <-> Name
                VillagerDict = CreateVillagerDictionary(); // Rname -> Iname
            }

            Connect().SafeFireAndForget();
        }

        private async Task Connect()
        {
            //Set up twitchlib pubsub
            MyPubSub = new TwitchPubSub();
            MyPubSub.OnListenResponse += OnListenResponse;
            MyPubSub.OnPubSubServiceConnected += OnPubSubServiceConnected;
            MyPubSub.OnPubSubServiceClosed += OnPubSubServiceClosed;
            MyPubSub.OnPubSubServiceError += OnPubSubServiceError;

            //Set up listeners
            //ListenToBits(channelId);
            //ListenToChatModeratorActions(channelId, channelId);
            //ListenToCommerce(channelId);
            //ListenToFollows(channelId);
            //ListenToLeaderboards(channelId);
            //ListenToPredictions(channelId);
            //ListenToRaid(channelId);
            ListenToRewards(channelId);
            //ListenToSubscriptions(channelId);
            //ListenToVideoPlayback(channelId);
            //ListenToWhispers(channelId);

            //Connect to pubsub
            MyPubSub.Connect();

            await Task.Delay(-1);
        }

        #region Whisper Events

        private void ListenToWhispers(string channelId)
        {
            MyPubSub.OnWhisper += PubSub_OnWhisper;
            MyPubSub.ListenToWhispers(channelId);
        }

        private void PubSub_OnWhisper(object sender, OnWhisperArgs e)
        {
            //_logger.Information($"{e.Whisper.DataObjectWhisperReceived.Recipient.DisplayName} send a whisper {e.Whisper.DataObjectWhisperReceived.Body}");
        }

        #endregion

        #region Video Playback Events

        private void ListenToVideoPlayback(string channelId)
        {
            MyPubSub.OnStreamUp += PubSub_OnStreamUp;
            MyPubSub.OnStreamDown += PubSub_OnStreamDown;
            MyPubSub.OnViewCount += PubSub_OnViewCount;
            //MyPubSub.OnCommercial += PubSub_OnCommercial;
            MyPubSub.ListenToVideoPlayback(channelId);
        }

        private void PubSub_OnViewCount(object sender, OnViewCountArgs e)
        {
            //_logger.Information($"Current viewers: {e.Viewers}");
        }

        private void PubSub_OnStreamDown(object sender, OnStreamDownArgs e)
        {
            //_logger.Information($"The stream is down");
        }

        private void PubSub_OnStreamUp(object sender, OnStreamUpArgs e)
        {
            //_logger.Information($"The stream is up");
        }

        #endregion

        #region Subscription Events

        private void ListenToSubscriptions(string channelId)
        {
            MyPubSub.OnChannelSubscription += PubSub_OnChannelSubscription;
            MyPubSub.ListenToSubscriptions(channelId);
        }

        private void PubSub_OnChannelSubscription(object sender, OnChannelSubscriptionArgs e)
        {
            var gifted = e.Subscription.IsGift ?? false;
            if (gifted)
            {
                //_logger.Information($"{e.Subscription.DisplayName} gifted a subscription to {e.Subscription.RecipientName}");
            }
            else
            {
                var cumulativeMonths = e.Subscription.CumulativeMonths ?? 0;
                if (cumulativeMonths != 0)
                {
                    //_logger.Information($"{e.Subscription.DisplayName} just subscribed (total of {cumulativeMonths} months)");
                }
                //_logger.Information($"{e.Subscription.DisplayName} just subscribed");
            }

        }

        #endregion

        #region Reward Events

        private void ListenToRewards(string channelId)
        {
            MyPubSub.OnChannelPointsRewardRedeemed += MyPubSub_OnChannelPointsRewardRedeemed;
            MyPubSub.ListenToChannelPoints(channelId);
            /*
            MyPubSub.OnRewardRedeemed += PubSub_OnRewardRedeemed;
            MyPubSub.OnCustomRewardCreated += PubSub_OnCustomRewardCreated;
            MyPubSub.OnCustomRewardDeleted += PubSub_OnCustomRewardDeleted;
            MyPubSub.OnCustomRewardUpdated += PubSub_OnCustomRewardUpdated;
            MyPubSub.ListenToRewards(channelId);
            */
        }

        private async void MyPubSub_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
        {
            string TimeStamp = e.RewardRedeemed.Redemption.RedeemedAt.ToString(CultureInfo.InvariantCulture);
            string ChannelId = e.ChannelId;
            string DisplayName = e.RewardRedeemed.Redemption.User.DisplayName;
            string RewardTitle = e.RewardRedeemed.Redemption.Reward.Title;
            string RewardId = e.RewardRedeemed.Redemption.Reward.Id;
            string Message = e.RewardRedeemed.Redemption.UserInput;

            Debug.Print($"{TimeStamp} {ChannelId} {DisplayName} {RewardTitle} {RewardId} : {Message}");

            if (RewardId.Equals(DropItemRewardId)) // Drop item
            {
                if (Message == null)
                {
                    WriteLog($">>> Reward : \"{RewardTitle}\" is missing the \"Require Viewer to Enter Text\" setting! <<< ");
                    return;
                }

                string name = "";
                string num = "0";
                string message = Message.Replace('’', '\'').Replace('`', '\'').Trim();
                if (message.Contains(","))
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
                await CheckAndAddItem(name, num, DisplayName);
            }
            else if (RewardId.Equals(DropRecipeRewardId)) // Drop recipe
            {
                if (Message == null)
                {
                    WriteLog($">>> Reward : \"{RewardTitle}\" is missing the \"Require Viewer to Enter Text\" setting! <<< ");
                }
                else
                {
                    string name = Message.Replace('’', '\'').Replace('`', '\'').Trim();
                    await CheckAndAddRecipe(name, DisplayName);
                }
            }
            else if (RewardId.Equals(InjectVillagerRewardId)) // Inject villager
            {
                if (Message == null)
                {
                    WriteLog($">>> Reward : \"{RewardTitle}\" is missing the \"Require Viewer to Enter Text\" setting! <<< ");
                }
                else
                {
                    string name = Message.Replace('’', '\'').Replace('é', 'e').Replace('É', 'E').Replace('[', ' ').Replace(']', ' ').Trim();
                    await CheckAndAddVillager(name, DisplayName, Message);
                }
            }
            else
            {
                WriteLog($"An unknown Reward : [{RewardTitle}] - \"{RewardId}\" has been redeemed.", true);
            }
        }

        private async void PubSub_OnRewardRedeemed(object sender, OnRewardRedeemedArgs e)
        {
            //Statuses can be:
            // "UNFULFILLED": when a user redeemed the reward
            // "FULFILLED": when a broadcaster or moderator marked the reward as complete
            /*
            if (e.Status == "UNFULFILLED")
            {
                Debug.Print($"{e.DisplayName} redeemed: {e.RewardTitle}");
            }
            */
            //Debug.Print($"Reward from {e.DisplayName} ({e.RewardTitle}) ({e.RewardId}) ({e.Message}) has been marked as complete");

            Debug.Print($"{e.TimeStamp} {e.ChannelId} {e.DisplayName} {e.RewardTitle} {e.RewardId} : {e.Message}");

            if (e.RewardId.ToString().Equals(DropItemRewardId)) // Drop item
            {
                if (e.Message == null)
                {
                    WriteLog($">>> Reward : \"{e.RewardTitle}\" is missing the \"Require Viewer to Enter Text\" setting! <<< ");
                }
                else
                {
                    string name = "";
                    string num = "0";
                    string message = e.Message.Replace('’', '\'').Replace('`', '\'').Trim();
                    if (message.Contains(","))
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
                    await CheckAndAddItem(name, num, e.DisplayName);
                }
            }
            else if (e.RewardId.ToString().Equals(DropRecipeRewardId)) // Drop recipe
            {
                if (e.Message == null)
                {
                    WriteLog($">>> Reward : \"{e.RewardTitle}\" is missing the \"Require Viewer to Enter Text\" setting! <<< ");
                    return;
                }

                string name = e.Message.Replace('’', '\'').Replace('`', '\'').Trim();
                await CheckAndAddRecipe(name, e.DisplayName);
            }
            else if (e.RewardId.ToString().Equals(InjectVillagerRewardId)) // Inject villager
            {
                if (e.Message == null)
                {
                    WriteLog($">>> Reward : \"{e.RewardTitle}\" is missing the \"Require Viewer to Enter Text\" setting! <<< ");
                    return;
                }

                string name = e.Message.Replace('’', '\'').Replace('é', 'e').Replace('É', 'E').Replace('[', ' ').Replace(']', ' ').Trim();
                await CheckAndAddVillager(name, e.DisplayName, e.Message);
            }
        }

        public static async Task CheckAndAddItem(string name, string numStr, string displayName)
        {
            int num = 0;
            int otherColor = 0;
            string colorStr = "";


            if (numStr.Contains("[") && numStr.Contains("]"))                       // ,[2]     ,[red]
            {
                string tempNum = numStr.Replace("[", "").Replace("]", "");          // 2 red
                try
                {
                    otherColor = int.Parse(tempNum);                                // 2
                }
                catch
                {
                    otherColor = 0;
                    colorStr = tempNum.ToLower();                                   // red
                    Debug.Print("OtherColor Invalid");
                }
            }
            else
            {
                try
                {
                    num = int.Parse(numStr);                                        // ,2
                }
                catch
                {
                    num = 0;
                    Debug.Print("Num Invalid");
                }
            }

            string hexValue = "0";
            if (num > 0)
            {
                hexValue = (num - 1).ToString("X");
            }

            //-----------------------------------------------------------------
            string matchName;
            string id = "";
            string color = "";
            if (ItemDict.ContainsKey(name.ToLower()) && otherColor == 0)            // exect name       exect name,[red]
            {
                DataView dv = new(itemSource);
                dv.RowFilter = string.Format("eng LIKE '%{0}%' AND color = '{1}'", EscapeLikeValue(name), EscapeLikeValue(colorStr));
                if (dv.Count > 0)                                                   // exect name,[red]
                {
                    DataRowView drv = dv[0];
                    matchName = drv["eng"].ToString();
                    if (matchName != null && matchName.Contains("[!]"))
                    {
                        await MyTwitchBot.SendMessage($"Sorry, the item \"{name}\" is invalid.");
                        return;
                    }
                    id = drv["id"].ToString();
                    color = drv["color"].ToString();
                }
                else
                {
                    id = ItemDict[name.ToLower()];
                    if (ColorDict.ContainsKey(id))
                        color = ColorDict[id];
                }
            }
            else
            {
                if (!colorStr.Equals(string.Empty))                                 // LIKE name,[red]
                {
                    DataView dv = new(itemSource);
                    dv.RowFilter = string.Format("eng LIKE '%{0}%' AND color = '{1}'", EscapeLikeValue(name), EscapeLikeValue(colorStr));
                    if (dv.Count > 0)
                    {
                        DataRowView drv = dv[0];
                        matchName = drv["eng"].ToString();
                        if (matchName != null && matchName.Contains("[!]"))
                        {
                            await MyTwitchBot.SendMessage($"Sorry, the item \"{name}\" is invalid.");
                            return;
                        }
                        id = drv["id"].ToString();
                        color = drv["color"].ToString();
                    }
                }


                if (id != null && id.Equals(string.Empty))
                {
                    DataView dv = new(itemSource);
                    dv.RowFilter = string.Format("eng LIKE '%{0}%'", EscapeLikeValue(name));
                    if (dv.Count > 0)
                    {
                        DataRowView drv;

                        if (ItemDict.ContainsKey(name.ToLower()))                   // exect name,[2]
                        {
                            int startRow = 9999;
                            int endRow = 0;
                            for (int i = 0; i < dv.Count; i++)
                            {
                                if (dv[i]["eng"].ToString()?.ToLower() == name.ToLower())
                                {
                                    if (i < startRow)
                                        startRow = i;
                                    if (i > endRow)
                                        endRow = i;
                                }
                            }

                            if (startRow + otherColor <= endRow)
                            {
                                drv = dv[startRow + otherColor];
                            }
                            else
                            {
                                drv = dv[endRow];
                            }

                        }
                        else
                        {
                            if (otherColor > dv.Count)
                            {
                                drv = dv[dv.Count - 1];
                            }
                            else
                            {
                                drv = dv[otherColor];
                            }
                        }
                        matchName = drv["eng"].ToString();
                        if (matchName != null && matchName.Contains("[!]"))
                        {
                            await MyTwitchBot.SendMessage($"Sorry, the item \"{name}\" is invalid.");
                            return;
                        }
                        id = drv["id"].ToString();
                        color = drv["color"].ToString();
                    }
                    else
                    {
                        if (name.Contains("[") && name.Contains("]"))
                            await MyTwitchBot.SendMessage($"Sorry, I am unable to find an item with the name \"{name}\". Did you forget the comma \",\" before the Brackets \"[ ]\"?");
                        else if (name.Contains("poster") || name.Contains("photo"))
                            await MyTwitchBot.SendMessage($"Sorry, I am unable to find an item with the name \"{name}\". Did you forget the \"\'s \" after the name?");
                        else if (name.Contains("0") || name.Contains("1") || name.Contains("2") || name.Contains("3") || name.Contains("4") || name.Contains("5") || name.Contains("6") || name.Contains("7") || name.Contains("8") || name.Contains("9"))
                            await MyTwitchBot.SendMessage($"Sorry, I am unable to find an item with the name \"{name}\". Did you forget the comma \",\" before the number?");
                        else
                            await MyTwitchBot.SendMessage($"Sorry, I am unable to find an item with the name \"{name}\". Are you sure you are using the correct \"English\" name?");
                        return;
                    }
                }
            }




            Debug.Print($"id {id} | hexValue {hexValue} | color {color}");


            if (id != null && !id.Equals(String.Empty))
            {
                bool success = UInt16.TryParse(id, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out _);

                if (color != null && !color.Equals(string.Empty))
                {
                    if (ReverseDict.ContainsKey(id))
                        await MyTwitchBot.SendMessage($"{displayName}, your order of \"{ReverseDict[id]}\" ({color}) have been received!");
                    else
                        await MyTwitchBot.SendMessage($"{displayName}, your order have been received!");
                }
                else if (success && hexValue != "0")
                {
                    /*
                    if (ItemAttr.hasQuantity(itemId))
                        await MyTwitchBot.SendMessage($"{displayName}, your order of \"{ReverseDict[id]}\" Quantity: [ {num} ]  have been received!");
                    else
                        await MyTwitchBot.SendMessage($"{displayName}, your order of \"{ReverseDict[id]}\" Variation: [ {hexValue} ]  have been received!");
                    */
                    await MyTwitchBot.SendMessage($"{displayName}, your order of \"{ReverseDict[id]}\" have been received!");
                }
                else
                {
                    if (ReverseDict.ContainsKey(id))
                        await MyTwitchBot.SendMessage($"{displayName}, your order of \"{ReverseDict[id]}\" have been received!");
                    else
                        await MyTwitchBot.SendMessage($"{displayName}, your order have been received!");
                }

                if (ReverseDict.ContainsKey(id))
                    WriteLog($"{displayName} | {ReverseDict[id]} | {id} | {hexValue} | {color}", true);
                else
                    WriteLog($"{displayName} | {id} | {hexValue} | {color}", true);

                /*
                string path = Form1.GetImagePathFromID(id, itemSource, Convert.ToUInt32("0x" + hexValue, 16));

                Image image = null;
                if (File.Exists(path))
                    image = Image.FromFile(path);
                */

                AddItem(displayName, id, hexValue, ReverseDict[id], color);
            }
        }

        public static async Task CheckAndAddRecipe(string name, string displayName)
        {
            const string ItemId = "16A2";

            string recipeId;
            if (RecipeDict.ContainsKey(name.ToLower()))
            {
                recipeId = RecipeDict[name.ToLower()];
            }
            else
            {
                DataView dv = new(recipeSource);
                dv.RowFilter = string.Format("eng LIKE '%{0}%'", EscapeLikeValue(name));
                if (dv.Count > 0)
                {
                    DataRowView drv;
                    drv = dv[0];
                    recipeId = drv["id"].ToString();
                }
                else
                {
                    await MyTwitchBot.SendMessage($"Sorry, I am unable to find an recipe with the name \"{name}\". Are you sure you are using the correct \"English\" name?");
                    return;
                }
            }

            Debug.Print($"ItemId {ItemId} | hexValue {recipeId}");

            if (recipeId != null && !recipeId.Equals(String.Empty))
            {
                if (RecipeDict.ContainsKey(recipeId))
                    await MyTwitchBot.SendMessage($"{displayName}, your order of \"{RecipeDict[recipeId]} recipe\" have been received!");
                else
                    await MyTwitchBot.SendMessage($"{displayName}, your order of recipe have been received!");

                if (RecipeDict.ContainsKey(recipeId))
                    WriteLog($"{displayName} | {RecipeDict[recipeId]} recipe | {recipeId} ", true);
                else
                    WriteLog($"{displayName} | {recipeId}", true);

                AddItem(displayName, ItemId, recipeId, RecipeDict[recipeId] + " recipe", "");
            }
        }

        public static async Task CheckAndAddVillager(string name, string displayName, string userInput)
        {
            if (VillagerDict.ContainsKey(name.ToLower()))
            {
                string Iname = VillagerDict[name.ToLower()];

                if (Iname.Equals("cbr18") || Iname.Equals("der10") || Iname.Equals("elp11") || Iname.Equals("gor11") || Iname.Equals("rbt20") || Iname.Equals("shp14"))
                {
                    await MyTwitchBot.SendMessage("Sorry, but Sanrio Villagers require special care. Talk to the streamer if you really want them.");
                    return;
                }


                string Rname = RealName[Iname];
                /*
                Image img;
                string path = Utilities.GetVillagerImage(Iname);

                if (!path.Equals(string.Empty))
                    img = Image.FromFile(path);
                else
                    img = new Bitmap(Properties.Resources.Leaf, new Size(110, 110));
                */

                await MyTwitchBot.SendMessage($"{displayName}, villager \"{name}\" is now packing up. Please wait for the confirmation before flying in.");
                WriteLog($"{displayName} | {Iname} | {Rname} ", true);

                AddVillager(displayName, Iname, Rname);
            }
            else
            {
                await MyTwitchBot.SendMessage($"Sorry, I am unable to find a villager with the name \"{userInput}\". Are you sure you are using the correct \"English\" name?");
            }
        }

        private static string EscapeLikeValue(string valueWithoutWildcards)
        {
            StringBuilder sb = new();
            for (int i = 0; i < valueWithoutWildcards.Length; i++)
            {
                char c = valueWithoutWildcards[i];
                if (c == '*' || c == '%' || c == '[' || c == ']')
                    sb.Append("[").Append(c).Append("]");
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private DataTable loadItemCSV(string filePath)
        {
            var dt = new DataTable();

            File.ReadLines(filePath).Take(1)
                .SelectMany(x => x.Split(new[] { " ; " }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(x => dt.Columns.Add(x.Trim()));

            File.ReadLines(filePath).Skip(1)
                .Select(x => x.Split(new[] { " ; " }, StringSplitOptions.RemoveEmptyEntries))
                .ToList()
                .ForEach(line => dt.Rows.Add(line));

            if (dt.Columns.Contains("id"))
                dt.PrimaryKey = new[] { dt.Columns["id"] };

            return dt;
        }

        private Dictionary<string, string> CreateItemDictionary(string path)
        {
            Dictionary<string, string> dict = new();

            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(new[] { " ; " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        if (!dict.ContainsKey(parts[2].ToLower()))
                            dict.Add(parts[2].ToLower(), parts[0]);
                    }
                    //Debug.Print(parts[0]);
                }
            }

            return dict;
        }

        private Dictionary<string, string> CreateReverseDictionary(string path)
        {
            Dictionary<string, string> dict = new();

            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(new[] { " ; " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        if (!dict.ContainsKey(parts[0]))
                            dict.Add(parts[0], parts[2].ToLower());
                    }
                }
            }

            return dict;
        }

        private Dictionary<string, string> CreateColorDictionary(string path)
        {
            Dictionary<string, string> dict = new();

            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(new[] { " ; " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        if (parts.Length > 13 && !dict.ContainsKey(parts[0]))
                            dict.Add(parts[0], parts[13]);
                    }
                    //Debug.Print(parts[0]);
                }
            }

            return dict;
        }

        private Dictionary<string, string> CreateRecipeDictionary(string path)
        {
            Dictionary<string, string> dict = new();

            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(new[] { " ; " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        dict.Add(parts[0], parts[2]);
                        dict.Add(parts[2], parts[0]);
                    }
                }
            }

            return dict;
        }

        private Dictionary<string, string> CreateVillagerDictionary()
        {
            Dictionary<string, string> dict = new();

            foreach (KeyValuePair<string, string> entry in RealName)
            {
                dict.Add(entry.Value.Replace('é', 'e').Replace('É', 'E').ToLower(), entry.Key);
            }

            if (dict.ContainsKey("empty"))
                dict.Remove("empty");

            return dict;
        }

        #endregion

        #region Pubsub events

        private void OnPubSubServiceError(object sender, OnPubSubServiceErrorArgs e)
        {
            Debug.Print($"{e.Exception.Message}");
        }

        private void OnPubSubServiceClosed(object sender, EventArgs e)
        {
            Debug.Print("Connection closed to pubsub server");
        }

        private void OnPubSubServiceConnected(object sender, EventArgs e)
        {
            WriteLog("Connected to Twitch pubsub server", true);
            MyPubSub.SendTopics(accessToken);
        }

        private void OnListenResponse(object sender, OnListenResponseArgs e)
        {
            if (!e.Successful)
            {
                Debug.Print($"Failed to listen! Response{e.Response}");
            }
        }

        public void Dispose()
        {
            MyPubSub.Disconnect();
        }

        #endregion

        public static void WriteLog(string line, Boolean time = false)
        {
            LogBox.Invoke((MethodInvoker)delegate
            {
                if (time)
                {
                    DateTime localDate = DateTime.Now;
                    LogBox.AppendText(localDate + " : " + line + "\n");
                }
                else
                    LogBox.AppendText(line + "\n");
            });
        }

        public static void AddItem(string Owner, string Id, string Count, string Name, string Color, Image Image = null)
        {
            DropOrderList.Add(new ItemOrder { Owner = Owner, Id = Id, Count = Count, Name = Name, Color = Color, Image = Image });

            /*
            if (itemDisplay != null && Image != null)
                itemDisplay.setItemdisplay(Image);
            */

            Debug.Print($"{Owner} - Item Added : {Name} ({Color})  [{Id}] [{Count}]");
        }

        public static void AddVillager(string Owner, string Iname, string Rname, Image Image = null)
        {
            VillagerOrderList.Add(new VillagerOrder { Owner = Owner, InternalName = Iname, RealName = Rname, Image = Image });

            Debug.Print($"{Owner} - Villager Added : {Rname} [{Iname}] ");
        }

        #region Villager
        public static Dictionary<string, string> RealName = new()
        {
                {"ant00", "Cyrano"},
                {"ant01", "Antonio"},
                {"ant02", "Pango"},
                {"ant03", "Anabelle"},
                {"ant05", "Zoe"},
                {"ant06", "Snooty"},
                {"ant08", "Annalisa"},
                {"ant09", "Olaf"},
                {"bea00", "Teddy"},
                {"bea01", "Pinky"},
                {"bea02", "Curt"},
                {"bea03", "Chow"},
                {"bea05", "Nate"},
                {"bea06", "Groucho"},
                {"bea07", "Tutu"},
                {"bea08", "Ursala"},
                {"bea09", "Grizzly"},
                {"bea10", "Paula"},
                {"bea11", "Ike"},
                {"bea12", "Charlise"},
                {"bea13", "Beardo"},
                {"bea14", "Klaus"},
                {"bea15", "Megan"},
                {"brd00", "Jay"},
                {"brd01", "Robin"},
                {"brd02", "Anchovy"},
                {"brd03", "Twiggy"},
                {"brd04", "Jitters"},
                {"brd05", "Piper"},
                {"brd06", "Admiral"},
                {"brd08", "Midge"},
                {"brd09", "Ace"},
                {"brd11", "Jacob"},
                {"brd15", "Lucha"},
                {"brd16", "Jacques"},
                {"brd17", "Peck"},
                {"brd18", "Sparro"},
                {"bul00", "Angus"},
                {"bul01", "Rodeo"},
                {"bul03", "Stu"},
                {"bul05", "T-Bone"},
                {"bul07", "Coach"},
                {"bul08", "Vic"},
                {"cat00", "Bob"},
                {"cat01", "Mitzi"},
                {"cat02", "Rosie"},
                {"cat03", "Olivia"},
                {"cat04", "Kiki"},
                {"cat05", "Tangy"},
                {"cat06", "Punchy"},
                {"cat07", "Purrl"},
                {"cat08", "Moe"},
                {"cat09", "Kabuki"},
                {"cat10", "Kid Cat"},
                {"cat11", "Monique"},
                {"cat12", "Tabby"},
                {"cat13", "Stinky"},
                {"cat14", "Kitty"},
                {"cat15", "Tom"},
                {"cat16", "Merry"},
                {"cat17", "Felicity"},
                {"cat18", "Lolly"},
                {"cat19", "Ankha"},
                {"cat20", "Rudy"},
                {"cat21", "Katt"},
                {"cat23", "Raymond"},
                {"cbr00", "Bluebear"},
                {"cbr01", "Maple"},
                {"cbr02", "Poncho"},
                {"cbr03", "Pudge"},
                {"cbr04", "Kody"},
                {"cbr05", "Stitches"},
                {"cbr06", "Vladimir"},
                {"cbr07", "Murphy"},
                {"cbr09", "Olive"},
                {"cbr10", "Cheri"},
                {"cbr13", "June"},
                {"cbr14", "Pekoe"},
                {"cbr15", "Chester"},
                {"cbr16", "Barold"},
                {"cbr17", "Tammy"},
                {"cbr18", "Marty"},
                {"cbr19", "Judy"},
                {"chn00", "Goose"},
                {"chn01", "Benedict"},
                {"chn02", "Egbert"},
                {"chn05", "Ava"},
                {"chn09", "Becky"},
                {"chn10", "Plucky"},
                {"chn11", "Knox"},
                {"chn12", "Broffina"},
                {"chn13", "Ken"},
                {"cow00", "Patty"},
                {"cow01", "Tipper"},
                {"cow06", "Norma"},
                {"cow07", "Naomi"},
                {"crd00", "Alfonso"},
                {"crd01", "Alli"},
                {"crd02", "Boots"},
                {"crd04", "Del"},
                {"crd05", "Roswell"},
                {"crd06", "Sly"},
                {"crd07", "Gayle"},
                {"crd08", "Drago"},
                {"der00", "Fauna"},
                {"der01", "Bam"},
                {"der02", "Zell"},
                {"der03", "Bruce"},
                {"der04", "Deirdre"},
                {"der05", "Lopez"},
                {"der06", "Fuchsia"},
                {"der07", "Beau"},
                {"der08", "Diana"},
                {"der09", "Erik"},
                {"der10", "Chelsea"},
                {"der11", "Shino"},
                {"dog00", "Goldie"},
                {"dog01", "Butch"},
                {"dog02", "Lucky"},
                {"dog03", "Biskit"},
                {"dog04", "Bones"},
                {"dog05", "Portia"},
                {"dog06", "Walker"},
                {"dog07", "Daisy"},
                {"dog08", "Cookie"},
                {"dog09", "Maddie"},
                {"dog10", "Bea"},
                {"dog11", "Frett"},
                {"dog14", "Mac"},
                {"dog15", "Marcel"},
                {"dog16", "Benjamin"},
                {"dog17", "Cherry"},
                {"dog18", "Shep"},
                {"duk00", "Bill"},
                {"duk01", "Joey"},
                {"duk02", "Pate"},
                {"duk03", "Maelle"},
                {"duk04", "Deena"},
                {"duk05", "Pompom"},
                {"duk06", "Mallary"},
                {"duk07", "Freckles"},
                {"duk08", "Derwin"},
                {"duk09", "Drake"},
                {"duk10", "Scoot"},
                {"duk11", "Weber"},
                {"duk12", "Miranda"},
                {"duk13", "Ketchup"},
                {"duk15", "Gloria"},
                {"duk16", "Molly"},
                {"duk17", "Quillson"},
                {"elp00", "Opal"},
                {"elp01", "Dizzy"},
                {"elp02", "Big Top"},
                {"elp03", "Eloise"},
                {"elp04", "Margie"},
                {"elp05", "Paolo"},
                {"elp06", "Axel"},
                {"elp07", "Ellie"},
                {"elp09", "Tucker"},
                {"elp10", "Tia"},
                {"elp11", "Chai"},
                {"elp12", "Cyd"},
                {"flg00", "Lily"},
                {"flg01", "Ribbot"},
                {"flg02", "Frobert"},
                {"flg03", "Camofrog"},
                {"flg04", "Drift"},
                {"flg05", "Wart Jr."},
                {"flg06", "Puddles"},
                {"flg07", "Jeremiah"},
                {"flg09", "Tad"},
                {"flg10", "Cousteau"},
                {"flg11", "Huck"},
                {"flg12", "Prince"},
                {"flg13", "Jambette"},
                {"flg15", "Raddle"},
                {"flg16", "Gigi"},
                {"flg17", "Croque"},
                {"flg18", "Diva"},
                {"flg19", "Henry"},
                {"goa00", "Chevre"},
                {"goa01", "Nan"},
                {"goa02", "Billy"},
                {"goa04", "Gruff"},
                {"goa06", "Velma"},
                {"goa07", "Kidd"},
                {"goa08", "Pashmina"},
                {"goa09", "Sherb"},
                {"gor00", "Cesar"},
                {"gor01", "Peewee"},
                {"gor02", "Boone"},
                {"gor04", "Louie"},
                {"gor05", "Boyd"},
                {"gor07", "Violet"},
                {"gor08", "Al"},
                {"gor09", "Rocket"},
                {"gor10", "Hans"},
                {"gor11", "Rilla"},
                {"ham00", "Hamlet"},
                {"ham01", "Apple"},
                {"ham02", "Graham"},
                {"ham03", "Rodney"},
                {"ham04", "Soleil"},
                {"ham05", "Clay"},
                {"ham06", "Flurry"},
                {"ham07", "Hamphrey"},
                {"ham09", "Marlo"},
                {"hip00", "Rocco"},
                {"hip02", "Bubbles"},
                {"hip03", "Bertha"},
                {"hip04", "Biff"},
                {"hip05", "Bitty"},
                {"hip08", "Harry"},
                {"hip09", "Hippeux"},
                {"hrs00", "Buck"},
                {"hrs01", "Victoria"},
                {"hrs02", "Savannah"},
                {"hrs03", "Elmer"},
                {"hrs04", "Roscoe"},
                {"hrs05", "Winnie"},
                {"hrs06", "Ed"},
                {"hrs07", "Cleo"},
                {"hrs08", "Peaches"},
                {"hrs09", "Annalise"},
                {"hrs10", "Clyde"},
                {"hrs11", "Colton"},
                {"hrs12", "Papi"},
                {"hrs13", "Julian"},
                {"hrs16", "Reneigh"},
                {"kal00", "Yuka"},
                {"kal01", "Alice"},
                {"kal02", "Melba"},
                {"kal03", "Sydney"},
                {"kal04", "Gonzo"},
                {"kal05", "Ozzie"},
                {"kal08", "Canberra"},
                {"kal07", "Faith"},
                {"kal09", "Lyman"},
                {"kal10", "Eugene"},
                {"kgr00", "Kitt"},
                {"kgr01", "Mathilda"},
                {"kgr02", "Carrie"},
                {"kgr05", "Astrid"},
                {"kgr06", "Sylvia"},
                {"kgr08", "Walt"},
                {"kgr09", "Rooney"},
                {"kgr10", "Marcie"},
                {"lon00", "Bud"},
                {"lon01", "Elvis"},
                {"lon02", "Rex"},
                {"lon04", "Leopold"},
                {"lon06", "Mott"},
                {"lon07", "Rory"},
                {"lon08", "Lionel"},
                {"mnk01", "Nana"},
                {"mnk02", "Simon"},
                {"mnk03", "Tammi"},
                {"mnk04", "Monty"},
                {"mnk05", "Elise"},
                {"mnk06", "Flip"},
                {"mnk07", "Shari"},
                {"mnk08", "Deli"},
                {"mnk09", "Tiansheng"},
                {"mus00", "Dora"},
                {"mus01", "Limberg"},
                {"mus02", "Bella"},
                {"mus03", "Bree"},
                {"mus04", "Samson"},
                {"mus05", "Rod"},
                {"mus08", "Candi"},
                {"mus09", "Rizzo"},
                {"mus10", "Anicotti"},
                {"mus12", "Broccolo"},
                {"mus14", "Moose"},
                {"mus15", "Bettina"},
                {"mus16", "Greta"},
                {"mus17", "Penelope"},
                {"mus18", "Chadder"},
                {"mus19", "Petri"},
                {"ocp00", "Octavian"},
                {"ocp01", "Marina"},
                {"ocp02", "Zucker"},
                {"ocp04", "Cephalobot"},
                {"ost00", "Queenie"},
                {"ost01", "Gladys"},
                {"ost02", "Sandy"},
                {"ost03", "Sprocket"},
                {"ost04", "Rio"},
                {"ost05", "Julia"},
                {"ost06", "Cranston"},
                {"ost07", "Phil"},
                {"ost08", "Blanche"},
                {"ost09", "Flora"},
                {"ost10", "Phoebe"},
                {"pbr00", "Apollo"},
                {"pbr01", "Amelia"},
                {"pbr02", "Pierce"},
                {"pbr03", "Buzz"},
                {"pbr05", "Avery"},
                {"pbr06", "Frank"},
                {"pbr07", "Sterling"},
                {"pbr08", "Keaton"},
                {"pbr09", "Celia"},
                {"pbr10", "Quinn"},
                {"pgn00", "Aurora"},
                {"pgn01", "Roald"},
                {"pgn02", "Cube"},
                {"pgn03", "Hopper"},
                {"pgn04", "Friga"},
                {"pgn05", "Gwen"},
                {"pgn06", "Puck"},
                {"pgn07", "Chabwick"},
                {"pgn09", "Wade"},
                {"pgn10", "Boomer"},
                {"pgn11", "Iggly"},
                {"pgn12", "Tex"},
                {"pgn13", "Flo"},
                {"pgn14", "Sprinkle"},
                {"pig00", "Curly"},
                {"pig01", "Truffles"},
                {"pig02", "Rasher"},
                {"pig03", "Hugh"},
                {"pig04", "Lucy"},
                {"pig05", "Spork"},
                {"pig08", "Cobb"},
                {"pig09", "Boris"},
                {"pig10", "Maggie"},
                {"pig11", "Peggy"},
                {"pig13", "Gala"},
                {"pig14", "Chops"},
                {"pig15", "Kevin"},
                {"pig16", "Pancetti"},
                {"pig17", "Agnes"},
                {"rbt00", "Bunnie"},
                {"rbt01", "Dotty"},
                {"rbt02", "Coco"},
                {"rbt03", "Snake"},
                {"rbt04", "Gaston"},
                {"rbt05", "Gabi"},
                {"rbt06", "Pippy"},
                {"rbt07", "Tiffany"},
                {"rbt08", "Genji"},
                {"rbt09", "Ruby"},
                {"rbt10", "Doc"},
                {"rbt11", "Claude"},
                {"rbt12", "Francine"},
                {"rbt13", "Chrissy"},
                {"rbt14", "Hopkins"},
                {"rbt15", "O'Hare"},
                {"rbt16", "Carmen"},
                {"rbt17", "Bonbon"},
                {"rbt18", "Cole"},
                {"rbt19", "Mira"},
                {"rbt20", "Toby"},
                {"rbt21", "Sasha"},
                {"rhn00", "Tank"},
                {"rhn01", "Rhonda"},
                {"rhn02", "Spike"},
                {"rhn04", "Hornsby"},
                {"rhn05", "Azalea"},
                {"rhn07", "Merengue"},
                {"rhn08", "Renée"},
                {"shp00", "Vesta"},
                {"shp01", "Baabara"},
                {"shp02", "Eunice"},
                {"shp03", "Stella"},
                {"shp04", "Cashmere"},
                {"shp07", "Willow"},
                {"shp08", "Curlos"},
                {"shp09", "Wendy"},
                {"shp10", "Timbra"},
                {"shp11", "Frita"},
                {"shp12", "Muffy"},
                {"shp13", "Pietro"},
                {"shp14", "Étoile"},
                {"shp15", "Dom"},
                {"squ00", "Peanut"},
                {"squ01", "Blaire"},
                {"squ02", "Filbert"},
                {"squ03", "Pecan"},
                {"squ04", "Nibbles"},
                {"squ05", "Agent S"},
                {"squ06", "Caroline"},
                {"squ07", "Sally"},
                {"squ08", "Static"},
                {"squ09", "Mint"},
                {"squ10", "Ricky"},
                {"squ11", "Cally"},
                {"squ13", "Tasha"},
                {"squ14", "Sylvana"},
                {"squ15", "Poppy"},
                {"squ16", "Sheldon"},
                {"squ17", "Marshal"},
                {"squ18", "Hazel"},
                {"squ21", "Ione"},
                {"tig00", "Rolf"},
                {"tig01", "Rowan"},
                {"tig02", "Tybalt"},
                {"tig03", "Bangle"},
                {"tig04", "Leonardo"},
                {"tig05", "Claudia"},
                {"tig06", "Bianca"},
                {"wol00", "Chief"},
                {"wol01", "Lobo"},
                {"wol02", "Wolfgang"},
                {"wol03", "Whitney"},
                {"wol04", "Dobie"},
                {"wol05", "Freya"},
                {"wol06", "Fang"},
                {"wol08", "Vivian"},
                {"wol09", "Skye"},
                {"wol10", "Kyle"},
                {"wol12", "Audie"},
                {"non00", "Empty" }
            };
        #endregion
    }
}
