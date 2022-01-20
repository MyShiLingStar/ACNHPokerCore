using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public static class Utilities
    {
        public static UInt32 masterAddress = 0xAF71E6E0; //0xAF70D6E0;

        public static UInt32 ItemSlotBase = masterAddress;
        public static UInt32 ItemSlot21Base = masterAddress - 0xB8;

        //AE6022CC

        public static UInt32 MasterRecyclingBase = 0xAEA6F978; //0xAEA5E978;
        public static UInt32 MasterRecycling21Base = MasterRecyclingBase + 0xA0;

        public static UInt32 TurnipPurchasePriceAddr = 0xAE6140F4; //0xAE6030F4;
        public static UInt32 TurnipSellPriceAddr = TurnipPurchasePriceAddr + 0xC;
        public static UInt32 TurnipBuffer = 0x8F1BD0;

        //=================================================================

        public static UInt32 VillagerAddress = 0xAE1B5EB8; //0xAE1A4EB8;
        public static UInt32 VillagerSize = 0x13230;
        public static UInt32 VillagerOldSize = 0x12AB0;
        public static UInt32 VillagerMemorySize = 0x5F0;

        public static UInt32 VillagerMemoryTinySize = 0x47;

        public static UInt32 VillagerPlayerOffset = 0x5F0;

        public static UInt32 VillagerMoveoutOffset = 0x1267A;
        public static UInt32 VillagerForceMoveoutOffset = 0x126AC;
        public static UInt32 VillagerAbandonHouseOffset = 0x1268E;
        public static UInt32 VillagerFriendshipOffset = 0x46;
        public static UInt32 VillagerCatchphraseOffset = 0x10794;

        public static UInt32 VillagerHouseAddress = 0xAE6056A4; //0xAE5F46A4;
        public static UInt32 VillagerHouseSize = 0x12E8;
        public static UInt32 VillagerHouseOldSize = 0x1D4;
        public static UInt32 VillagerHouseBufferDiff = 0x8F1BD0;
        public static UInt32 VillagerHouseOwnerOffset = 0x1C4;

        public static UInt32 MysIslandVillagerAddress = 0x38B33C1C; //0x38B23C1C;
        public static UInt32 MysIslandVillagerSpecies = MysIslandVillagerAddress + 0x110;

        //=================================================================

        public static UInt32 weatherSeed = 0xAE3994B0; //0xAE3884B0;

        public static UInt32 coordinate = 0x3E33A288; //0x3E32A288;

        public static UInt32 mapZero = 0xAE3E5298; //0xAE3D4298;

        public static UInt32 mapOffset = 0x8F1BD0;

        public static UInt32 mapSize = 0x54000;

        //=================================================================

        public static UInt32 VisitorNameAddress = 0xB710ED78; //0xB70FDD30;

        public static UInt32 dodoAddress = 0xABE015C; //0xABCF15C;
        public static UInt32 OnlineSessionAddress = 0x945F740; //0x944E740;

        public static UInt32 VisitorList = VisitorNameAddress - 0x118;
        public static UInt32 VisitorListSize = 0x1C;

        public static UInt32 TextSpeedAddress = 0x0BD43084; //0xBD32084;

        public static UInt32 ChineseLanguageOffset = 0x7000;

        public static UInt32 savingOffset = 0x457B05AC; //0x457A05AC;

        //=================================================================

        public static UInt32 TerrainOffset = mapZero + 0xAAA00; //TODO?

        public static UInt32 AcreOffset = mapZero + 0xCF998;

        private const int AcreWidth = 7 + (2 * 1);
        private const int AcreHeight = 6 + (2 * 1);
        private const int AcreMax = AcreWidth * AcreHeight;
        public const int AllAcreSize = AcreMax * 2;
        public const int AcreAndPlaza = AllAcreSize + 2 + 2 + 4 + 4;

        public static UInt32 BuildingOffset = mapZero + 0xCF600;
        private const int BuildingSize = 0x14;
        private const int AllBuildingSize = 46 * BuildingSize;

        public static UInt32 player1SlotBase = masterAddress;
        public static UInt32 playerOffset = 0x11B968;
        public static UInt32 Slot21Offset = 0xB8;
        public static UInt32 HomeOffset = 0xC4;
        public static UInt32 ReactionOffset = 0xAFB4;
        public static UInt32 InventoryNameOffset = 0x3EAE0;

        public static UInt32 TownNameddress = player1SlotBase + InventoryNameOffset;

        public static UInt32 player1Slot21Base = player1SlotBase - Slot21Offset;
        public static UInt32 player1HouseBase = player1SlotBase + HomeOffset;
        public static UInt32 player1House21Base = player1HouseBase + 0xA0;

        public static UInt32 playerReactionAddress = player1SlotBase + ReactionOffset;

        public static UInt32 player2SlotBase = player1SlotBase + playerOffset;
        public static UInt32 player2Slot21Base = player2SlotBase - Slot21Offset;
        public static UInt32 player2HouseBase = player2SlotBase + HomeOffset;
        public static UInt32 player2House21Base = player2HouseBase + 0xA0;

        public static UInt32 player3SlotBase = player2SlotBase + playerOffset;
        public static UInt32 player3Slot21Base = player3SlotBase - Slot21Offset;
        public static UInt32 player3HouseBase = player3SlotBase + HomeOffset;
        public static UInt32 player3House21Base = player3HouseBase + 0xA0;

        public static UInt32 player4SlotBase = player3SlotBase + playerOffset;
        public static UInt32 player4Slot21Base = player4SlotBase - Slot21Offset;
        public static UInt32 player4HouseBase = player4SlotBase + HomeOffset;
        public static UInt32 player4House21Base = player4HouseBase + 0xA0;

        public static UInt32 player5SlotBase = player4SlotBase + playerOffset;
        public static UInt32 player5Slot21Base = player5SlotBase - Slot21Offset;
        public static UInt32 player5HouseBase = player5SlotBase + HomeOffset;
        public static UInt32 player5House21Base = player5HouseBase + 0xA0;

        public static UInt32 player6SlotBase = player5SlotBase + playerOffset;
        public static UInt32 player6Slot21Base = player6SlotBase - Slot21Offset;
        public static UInt32 player6HouseBase = player6SlotBase + HomeOffset;
        public static UInt32 player6House21Base = player6HouseBase + 0xA0;

        public static UInt32 player7SlotBase = player6SlotBase + playerOffset;
        public static UInt32 player7Slot21Base = player7SlotBase - Slot21Offset;
        public static UInt32 player7HouseBase = player7SlotBase + HomeOffset;
        public static UInt32 player7House21Base = player7HouseBase + 0xA0;

        public static UInt32 player8SlotBase = player7SlotBase + playerOffset;
        public static UInt32 player8Slot21Base = player8SlotBase - Slot21Offset;
        public static UInt32 player8HouseBase = player8SlotBase + HomeOffset;
        public static UInt32 player8House21Base = player8HouseBase + 0xA0;

        // ---- Critter
        public static UInt32 InsectAppearPointer = 0x404DB718; //0x404C4D18; //0x404C4BF8;
        public static Int32 InsectDataSize = 2 * (1 + 6 * 12 + 5);
        public static Int32 InsectNumRecords = 166;

        public static Int32 FishDataSize = 88;

        public static UInt32 FishRiverAppearPointer = 0x4051AEA8; //0x405044A8; //0x40504388; 
        public static Int32 FishRiverNumRecords = 100;

        public static UInt32 FishSeaAppearPointer = 0x40531088; //0x4051A688; //0x4051A568;
        public static Int32 FishSeaNumRecords = 76;

        public static UInt32 CreatureSeaAppearPointer = 0x4049D3AC; //0x404869AC; //0x4048688C;
        public static Int32 SeaCreatureDataSize = 88;
        public static Int32 SeaCreatureNumRecords = 41 * 2;
        // ----

        public static UInt32 staminaAddress = 0xB6872358; //0xB6861358;
        public static UInt32 readTimeAddress = 0x0BD3A188; //0x0BD29188;

        public static readonly UInt32 MaxSpeedAddress = 0x0BF5934C; //0x0BF4834C;
        public static readonly string MaxSpeedX1 = "0000A03F";
        public static readonly string MaxSpeedX2 = "00002040";
        public static readonly string MaxSpeedX3 = "00007040";
        public static readonly string MaxSpeedX5 = "0000C840";
        public static readonly string MaxSpeedX100 = "0000FA42";

        // ---- Main
        public static UInt32 freezeTimeAddress = 0x00328BB0; //0x00328530;
        public static readonly string freezeTimeValue = "D503201F";
        public static readonly string unfreezeTimeValue = "F9203260";

        public static UInt32 wSpeedAddress = 0x01612720; //0x01605EB0; //0x01605E90; // 0x01605E20; //0x01605DF0;
        public static readonly string wSpeedX1 = "BD578661";
        public static readonly string wSpeedX2 = "1E201001";
        public static readonly string wSpeedX3 = "1E211001";
        public static readonly string wSpeedX4 = "1E221001";

        public static UInt32 CollisionAddress = 0x0155FD40; //0x01554E10; //0x01554DF0; // 0x01554D80; //0x01554D50;
        public static readonly string CollisionDisable = "12800014";
        public static readonly string CollisionEnable = "B95BA014";

        public static UInt32 aSpeedAddress = 0x043BC3C0; //0x043A4B30;
        public static readonly string aSpeedX1 = "3F800000";
        public static readonly string aSpeedX2 = "40000000";
        public static readonly string aSpeedX5 = "40A00000";
        public static readonly string aSpeedX50 = "42480000";
        public static readonly string aSpeedX01 = "3DCCCCCD";




        public static string csvFolder = @"csv\";
        public static string imagePath = @"img\";
        public static string villagerPath = @"villager\";
        public static string saveFolder = @"save\";

        public static string itemFile = @"items.csv";
        public static string itemPath = csvFolder + itemFile;
        public static string overrideFile = @"override.csv";
        public static string overridePath = csvFolder + overrideFile;
        public static string recipeFile = @"recipes.csv";
        public static string recipePath = csvFolder + recipeFile;
        public static string flowerFile = @"flowers.csv";
        public static string flowerPath = csvFolder + flowerFile;
        public static string variationFile = @"variations.csv";
        public static string variationPath = csvFolder + variationFile;
        public static string favFile = @"fav.csv";
        public static string favPath = csvFolder + favFile;
        public static string fieldFile = @"field.csv";
        public static string fieldPath = csvFolder + fieldFile;
        public static string kindFile = @"kind.csv";
        public static string kindPath = csvFolder + kindFile;

        public static string dodoFile = @"dodo.txt";
        public static string dodoPath = saveFolder + dodoFile;
        public static string webhookFile = @"webhook.txt";
        public static string webhookPath = saveFolder + webhookFile;
        public static string TwitchSettingFile = @"twitch.json";
        public static string TwitchSettingPath = saveFolder + TwitchSettingFile;

        public static string logFile = @"ApplicationLog.csv";
        public static string logPath = saveFolder + logFile;

        public static string VisitorLogFile = @"VisitorLog.csv";
        public static string VisitorLogPath = saveFolder + VisitorLogFile;
        public static string CurrentVillagerFile = @"villager.txt";
        public static string CurrentVillagerPath = saveFolder + CurrentVillagerFile;
        public static string CurrentVisitorFile = @"visitor.txt";
        public static string CurrentVisitorPath = saveFolder + CurrentVisitorFile;

        public static string teleportFile = @"teleport.bin";
        public static string teleportPath = saveFolder + teleportFile;
        public static string anchorPath = @"Anchors.bin";

        public static string MissingImage = @"QuestionMark.png";
        public static string RecipeOverlayFile = @"PaperRecipe.png";
        public static string RecipeOverlayPath = imagePath + RecipeOverlayFile;

        public static Dictionary<string, string> itemkind = new Dictionary<string, string>();

        private static Object botLock = new Object();

        public static void buildDictionary()
        {
            if (File.Exists(kindPath))
            {
                string[] lines = File.ReadAllLines(kindPath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(new[] { " ; " }, StringSplitOptions.RemoveEmptyEntries);
                    if (line.Contains("Kind_"))
                    {
                        if (parts[1].Contains("Fake"))
                        {
                            itemkind.Add(parts[0], parts[1].Replace("Fake", ""));
                        }
                        else
                            itemkind.Add(parts[0], parts[1]);
                    }
                }
            }
        }

        private static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static string GetItemSlotAddress(int slot)
        {
            if (slot <= 20)
            {
                return "0x" + (ItemSlotBase + ((Clamp(slot, 1, 20) - 1) * 0x8)).ToString("X");
            }
            else
            {
                return "0x" + (ItemSlot21Base + ((Clamp(slot, 21, 40) - 21) * 0x8)).ToString("X");
            }
        }

        public static uint GetItemSlotUIntAddress(int slot)
        {
            if (slot <= 20)
            {
                return (uint)(ItemSlotBase + ((Clamp(slot, 1, 20) - 1) * 0x8));
            }
            else
            {
                return (uint)(ItemSlot21Base + ((Clamp(slot, 21, 40) - 21) * 0x8));
            }
        }

        public static string GetItemCountAddress(int slot)
        {
            if (slot <= 20)
            {
                return "0x" + (ItemSlotBase + ((Clamp(slot, 1, 20) - 1) * 0x8) + 0x4).ToString("X");
            }
            else
            {
                return "0x" + (ItemSlot21Base + ((Clamp(slot, 21, 40) - 21) * 0x8) + 0x4).ToString("X");
            }
        }

        public static uint GetItemCountUIntAddress(int slot)
        {
            if (slot <= 20)
            {
                return (uint)(ItemSlotBase + ((Clamp(slot, 1, 20) - 1) * 0x8) + 0x4);
            }
            else
            {
                return (uint)(ItemSlot21Base + ((Clamp(slot, 21, 40) - 21) * 0x8) + 0x4);
            }
        }

        public static string GetItemFlag1Address(int slot)
        {
            if (slot <= 20)
            {
                return (0x3 + ItemSlotBase + ((Clamp(slot, 1, 20) - 1) * 0x8)).ToString("X");
            }
            else
            {
                return (0x3 + ItemSlot21Base + ((Clamp(slot, 21, 40) - 21) * 0x8)).ToString("X");
            }
        }

        public static byte[] stringToByte(string Bank)
        {
            if (Bank.Length <= 1)
            {
                byte[] small = new byte[1];
                small[0] = Convert.ToByte(Bank, 16);
                return small;
            }

            byte[] save = new byte[Bank.Length / 2];

            for (int i = 0; i < Bank.Length / 2; i++)
            {

                string data = String.Concat(Bank[(i * 2)].ToString(), Bank[((i * 2) + 1)].ToString());
                //Debug.Print(i.ToString() + " " + data);
                save[i] = Convert.ToByte(data, 16);
            }

            return save;
        }

        public static string ByteToHexString(byte[] b)
        {
            String hexString = BitConverter.ToString(b);
            hexString = hexString.Replace("-", "");

            return hexString;
        }

        public static byte[] ByteTrim(byte[] input)
        {
            int newLength = 1;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == 0x0)
                {
                    newLength = i;
                    break;
                }
            }

            byte[] newArray = new byte[newLength];
            Array.Copy(input, newArray, newArray.Length);

            return newArray;
        }

        public static byte[] GetInventoryBank(Socket socket, USBBot bot, int slot)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : Inventory " + GetItemSlotUIntAddress(slot).ToString("X") + " " + slot);

                    byte[] b = ReadByteArray(socket, GetItemSlotUIntAddress(slot), 160);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n GetItemSlotUIntAddress(" + slot + ")");
                    }

                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : Inventory " + GetItemSlotUIntAddress(slot).ToString("X") + " " + slot);

                    byte[] b = bot.ReadBytes(GetItemSlotUIntAddress(slot), 160);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n GetItemSlotUIntAddress(" + slot + ")");
                    }

                    return b;
                }
            }
        }

        public static void SpawnItem(Socket socket, USBBot bot, int slot, String value, String amount)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    string msg = String.Format("poke {0:X8} 0x{1}\r\n", GetItemSlotAddress(slot), flip(precedingZeros(value, 8)));
                    SendString(socket, Encoding.UTF8.GetBytes(msg));

                    string countMsg = String.Format("poke {0:X8} 0x{1}\r\n", GetItemCountAddress(slot), flip(precedingZeros(amount, 8)));
                    SendString(socket, Encoding.UTF8.GetBytes(countMsg));
                }
                else
                {
                    bot.WriteBytes(stringToByte(flip(precedingZeros(value, 8))), GetItemSlotUIntAddress(slot));

                    bot.WriteBytes(stringToByte(flip(precedingZeros(amount, 8))), GetItemCountUIntAddress(slot));
                }

                //Debug.Print("Slot : " + slot + " | ID : " + value + " | Amount : " + amount);
                //Debug.Print("Spawn Item : poke " + GetItemSlotAddress(slot) + " 0x" + flip(precedingZeros(value, 8)) + " 0x" + flip(precedingZeros(amount, 8)));
            }
        }

        public static bool SpawnRecipe(Socket socket, USBBot bot, int slot, String value, String recipeValue)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        string msg = String.Format("poke {0:X8} 0x{1}\r\n", GetItemSlotAddress(slot), flip(precedingZeros(value, 8)));
                        SendString(socket, Encoding.UTF8.GetBytes(msg));

                        string countMsg = String.Format("poke {0:X8} 0x{1}\r\n", GetItemCountAddress(slot), flip(precedingZeros(recipeValue, 8)));
                        SendString(socket, Encoding.UTF8.GetBytes(countMsg));
                    }
                    else
                    {
                        bot.WriteBytes(stringToByte(flip(precedingZeros(value, 8))), GetItemSlotUIntAddress(slot));

                        bot.WriteBytes(stringToByte(flip(precedingZeros(recipeValue, 8))), GetItemCountUIntAddress(slot));
                    }

                    //Debug.Print("Slot : " + slot + " | ID : " + value + " | RecipeValue : " + recipeValue);
                    //Debug.Print("Spawn recipe : poke " + GetItemSlotAddress(slot) + " 0x" + flip(precedingZeros(value, 8)) + " 0x" + flip(precedingZeros(recipeValue, 8)));
                    return true;
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }

                return false;
            }
        }

        public static bool SpawnFlower(Socket socket, USBBot bot, int slot, String value, String flowerValue)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        string msg = String.Format("poke {0:X8} 0x{1}\r\n", GetItemSlotAddress(slot), flip(precedingZeros(value, 8)));
                        SendString(socket, Encoding.UTF8.GetBytes(msg));

                        string countMsg = String.Format("poke {0:X8} 0x{1}\r\n", GetItemCountAddress(slot), flip(precedingZeros(flowerValue, 8)));
                        SendString(socket, Encoding.UTF8.GetBytes(countMsg));
                    }
                    else
                    {
                        bot.WriteBytes(stringToByte(flip(precedingZeros(value, 8))), GetItemSlotUIntAddress(slot));

                        bot.WriteBytes(stringToByte(flip(precedingZeros(flowerValue, 8))), GetItemCountUIntAddress(slot));
                    }

                    //Debug.Print("Slot : " + slot + " | ID : " + value + " | FlowerValue : " + flowerValue);
                    //Debug.Print("Spawn Flower : poke " + GetItemSlotAddress(slot) + " 0x" + flip(precedingZeros(value, 8)) + " 0x" + flip(precedingZeros(flowerValue, 8)));
                    return true;
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }

                return false;
            }
        }

        public static string flip(string value)
        {
            if (value.Length == 4)
            {
                string first = value.Substring(0, 2);
                string second = value.Substring(2, 2);
                string postFlip = second + first;
                return postFlip;
            }
            else if (value.Length == 8)
            {
                string first = value.Substring(0, 2);
                string second = value.Substring(2, 2);
                string third = value.Substring(4, 2);
                string fourth = value.Substring(6, 2);
                string postFlip = fourth + third + second + first;
                return postFlip;
            }
            else
            {
                return value;
            }
        }

        public static string precedingZeros(string value, int length)
        {
            if (value.Length >= length)
                return value;
            string n0 = String.Concat(Enumerable.Repeat("0", length - value.Length));
            string result = String.Concat(n0, value);
            return result;
        }

        public static string turn2bytes(string value)
        {
            if (value.Length < 4)
                return precedingZeros(value, 4);
            else
                return value.Substring(value.Length - 4, 4);
        }

        public static void DeleteSlot(Socket s, USBBot bot, int slot)
        {
            SpawnItem(s, bot, slot, "FFFE", "0");
        }

        public static void OverwriteAll(Socket socket, USBBot bot, byte[] buffer1, byte[] buffer2, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    SendByteArray8(socket, GetItemSlotUIntAddress(1), buffer1, 160, ref counter);
                    SendByteArray8(socket, GetItemSlotUIntAddress(21), buffer2, 160, ref counter);
                }
                else
                {
                    bot.WriteBytes(buffer1, GetItemSlotUIntAddress(1));
                    bot.WriteBytes(buffer2, GetItemSlotUIntAddress(21));
                }
            }
        }

        public static void OverwriteAll(Socket socket, USBBot bot, byte[] buffer1, byte[] buffer2)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    SendByteArray8(socket, GetItemSlotUIntAddress(1), buffer1, 160);
                    SendByteArray8(socket, GetItemSlotUIntAddress(21), buffer2, 160);
                }
                else
                {
                    bot.WriteBytes(buffer1, GetItemSlotUIntAddress(1));
                    bot.WriteBytes(buffer2, GetItemSlotUIntAddress(21));
                }
            }
        }

        public static UInt64[] GetTurnipPrices(Socket socket, USBBot bot)
        {
            lock (botLock)
            {
                UInt64[] result = new UInt64[13];
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : TurnipPurchasePrice " + TurnipPurchasePriceAddr.ToString("X"));

                    ReadUInt64Array(socket, TurnipPurchasePriceAddr, result, 4, 12);

                    Debug.Print("[Sys] Peek : TurnipSellPriceAddr " + TurnipSellPriceAddr.ToString("X"));

                    ReadUInt64Array(socket, TurnipSellPriceAddr, result, 4 * 12, 0);
                }
                else
                {
                    Debug.Print("[Usb] Peek : TurnipPrice " + TurnipPurchasePriceAddr.ToString("X") + " " + TurnipSellPriceAddr.ToString("X"));

                    byte[] b = bot.ReadBytes(TurnipPurchasePriceAddr, 57);

                    result[12] = b[0];

                    for (int i = 0; i < 12; i++)
                    {
                        result[i] = b[12 + (i * 4)];
                    }
                }
                return result;
            }
        }

        public static bool ChangeTurnipPrices(Socket socket, USBBot bot, UInt32[] prices)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    SendUInt32Array(socket, TurnipPurchasePriceAddr, prices, 4, 12);
                    SendUInt32Array(socket, TurnipPurchasePriceAddr + TurnipBuffer, prices, 4, 12);
                    SendUInt32Array(socket, TurnipSellPriceAddr, prices, 4 * 12);
                    SendUInt32Array(socket, TurnipSellPriceAddr + TurnipBuffer, prices, 4 * 12);
                }
                else
                {
                    byte[] BuyPrice = stringToByte(flip(precedingZeros(prices[12].ToString("X"), 8)));
                    bot.WriteBytes(BuyPrice, TurnipPurchasePriceAddr);
                    bot.WriteBytes(BuyPrice, TurnipPurchasePriceAddr + TurnipBuffer);

                    for (int i = 0; i < 12; i++)
                    {
                        bot.WriteBytes(stringToByte(flip(precedingZeros(prices[i].ToString("X"), 8))), (uint)(TurnipSellPriceAddr + (4 * i)));
                        bot.WriteBytes(stringToByte(flip(precedingZeros(prices[i].ToString("X"), 8))), (uint)(TurnipSellPriceAddr + (4 * i) + TurnipBuffer));
                    }
                }
                return false;
            }
        }

        public static void setAddress(int player)
        {
            if (player == 1)
            {
                ItemSlotBase = player1SlotBase;
                ItemSlot21Base = player1Slot21Base;
            }
            else if (player == 2)
            {
                ItemSlotBase = player2SlotBase;
                ItemSlot21Base = player2Slot21Base;
            }
            else if (player == 3)
            {
                ItemSlotBase = player3SlotBase;
                ItemSlot21Base = player3Slot21Base;
            }
            else if (player == 4)
            {
                ItemSlotBase = player4SlotBase;
                ItemSlot21Base = player4Slot21Base;
            }
            else if (player == 5)
            {
                ItemSlotBase = player5SlotBase;
                ItemSlot21Base = player5Slot21Base;
            }
            else if (player == 6)
            {
                ItemSlotBase = player6SlotBase;
                ItemSlot21Base = player6Slot21Base;
            }
            else if (player == 7)
            {
                ItemSlotBase = player7SlotBase;
                ItemSlot21Base = player7Slot21Base;
            }
            else if (player == 8)
            {
                ItemSlotBase = player8SlotBase;
                ItemSlot21Base = player8Slot21Base;
            }
            else if (player == 9) //Recycling
            {
                ItemSlotBase = MasterRecyclingBase;
                ItemSlot21Base = MasterRecycling21Base;
            }
            else if (player == 11) //House 1
            {
                ItemSlotBase = player1HouseBase;
                ItemSlot21Base = player1House21Base;
            }
            else if (player == 12) //House 2
            {
                ItemSlotBase = player2HouseBase;
                ItemSlot21Base = player2House21Base;
            }
            else if (player == 13) //House 3
            {
                ItemSlotBase = player3HouseBase;
                ItemSlot21Base = player3House21Base;
            }
            else if (player == 14) //House 4
            {
                ItemSlotBase = player4HouseBase;
                ItemSlot21Base = player4House21Base;
            }
            else if (player == 15) //House 5
            {
                ItemSlotBase = player5HouseBase;
                ItemSlot21Base = player5House21Base;
            }
            else if (player == 16) //House 6
            {
                ItemSlotBase = player6HouseBase;
                ItemSlot21Base = player6House21Base;
            }
            else if (player == 17) //House 7
            {
                ItemSlotBase = player7HouseBase;
                ItemSlot21Base = player7House21Base;
            }
            else if (player == 18) //House 8
            {
                ItemSlotBase = player8HouseBase;
                ItemSlot21Base = player8House21Base;
            }
        }

        public static void gotoRecyclingPage(uint page)
        {
            ItemSlotBase = MasterRecyclingBase + ((page - 1) * 0x140);
            ItemSlot21Base = MasterRecycling21Base + ((page - 1) * 0x140);
        }

        public static void gotoHousePage(uint page, int player)
        {
            switch (player)
            {
                case 1:
                    ItemSlotBase = player1HouseBase + ((page - 1) * 0x140);
                    ItemSlot21Base = player1House21Base + ((page - 1) * 0x140);
                    break;
                case 2:
                    ItemSlotBase = player2HouseBase + ((page - 1) * 0x140);
                    ItemSlot21Base = player2House21Base + ((page - 1) * 0x140);
                    break;
                case 3:
                    ItemSlotBase = player3HouseBase + ((page - 1) * 0x140);
                    ItemSlot21Base = player3House21Base + ((page - 1) * 0x140);
                    break;
                case 4:
                    ItemSlotBase = player4HouseBase + ((page - 1) * 0x140);
                    ItemSlot21Base = player4House21Base + ((page - 1) * 0x140);
                    break;
                case 5:
                    ItemSlotBase = player5HouseBase + ((page - 1) * 0x140);
                    ItemSlot21Base = player5House21Base + ((page - 1) * 0x140);
                    break;
                case 6:
                    ItemSlotBase = player6HouseBase + ((page - 1) * 0x140);
                    ItemSlot21Base = player6House21Base + ((page - 1) * 0x140);
                    break;
                case 7:
                    ItemSlotBase = player7HouseBase + ((page - 1) * 0x140);
                    ItemSlot21Base = player7House21Base + ((page - 1) * 0x140);
                    break;
                case 8:
                    ItemSlotBase = player8HouseBase + ((page - 1) * 0x140);
                    ItemSlot21Base = player8House21Base + ((page - 1) * 0x140);
                    break;
            }
        }

        public static byte[] peekAddress(Socket socket, USBBot bot, long address, int size)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        /*
                        string msg = String.Format("peek {0:X8} {1}\r\n", address, size);
                        Debug.Print("Peek : " + msg);
                        SendString(socket, Encoding.UTF8.GetBytes(msg));

                        byte[] b = new byte[330];
                        socket.Receive(b);
                        */
                        Debug.Print("[Sys] Peek : Address " + address.ToString("X") + " " + size);

                        byte[] b = ReadByteArray(socket, address, size);

                        if (b == null)
                        {
                            MessageBox.Show("Wait something is wrong here!? \n\n peek " + address.ToString("X") + " " + size);
                        }

                        return b;
                    }
                    else
                    {
                        Debug.Print("[Usb] Peek : Address " + address.ToString("X") + " " + size);

                        byte[] b = bot.ReadBytes((uint)address, size);

                        if (b == null)
                        {
                            MessageBox.Show("Wait something is wrong here!? \n\n peek " + address);
                        }

                        return b;
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                    return null;
                }
            }
        }

        public static void pokeAddress(Socket socket, USBBot bot, string address, string value)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        string msg = String.Format("poke 0x{0:X8} {1}\r\n", address, "0x" + value);
                        Debug.Print("Poke : " + msg);
                        SendString(socket, Encoding.UTF8.GetBytes(msg));
                    }
                    else
                    {
                        bot.WriteBytes(stringToByte(value), Convert.ToUInt32(address, 16));
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static void pokeMainAddress(Socket socket, USBBot bot, string address, string value)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        string msg = String.Format("pokeMain 0x{0:X8} 0x{1}\r\n", address, flip(value));
                        Debug.Print("PokeMain : " + msg);
                        SendString(socket, Encoding.UTF8.GetBytes(msg));
                    }
                    else
                    {
                        bot.WriteBytes(stringToByte(value), Convert.ToUInt32(address, 16));
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static byte[] peekMainAddress(Socket socket, string address, int size)
        {
            lock (botLock)
            {
                byte[] result = new byte[size];

                string msg = String.Format("peekMain 0x{0:X8} 0x{1}\r\n", address, size);
                //Debug.Print("PeekMain : " + msg);
                SendString(socket, Encoding.UTF8.GetBytes(msg));

                byte[] b = new byte[size * 2 + 64];
                int first_rec = ReceiveString(socket, b);
                string buffer = Encoding.ASCII.GetString(b, 0, size * 2);

                if (buffer == null)
                {
                    return null;
                }
                for (int i = 0; i < size; i++)
                {
                    result[i] = Convert.ToByte(buffer.Substring(i * 2, 2), 16);
                }

                return result;
            }
        }

        public static byte[] peekAbsoluteAddress(Socket socket, string address, int size)
        {
            lock (botLock)
            {
                byte[] result = new byte[size];

                string msg = String.Format("peekAbsolute 0x{0:X8} {1}\r\n", address, size);
                SendString(socket, Encoding.UTF8.GetBytes(msg));
                byte[] b = new byte[size * 2 + 64];
                int first_rec = ReceiveString(socket, b);
                string buffer = Encoding.ASCII.GetString(b, 0, size * 2);

                if (buffer == null)
                {
                    return null;
                }
                for (int i = 0; i < size; i++)
                {
                    result[i] = Convert.ToByte(buffer.Substring(i * 2, 2), 16);
                }

                return result;
            }
        }

        public static void pokeAbsoluteAddress(Socket socket, string address, string value)
        {
            lock (botLock)
            {
                string msg = String.Format("pokeAbsolute 0x{0:X8} 0x{1}\r\n", address, value);
                SendString(socket, Encoding.UTF8.GetBytes(msg));
            }
        }

        public static void setStamina(Socket socket, USBBot bot, string value)
        {
            pokeAddress(socket, bot, staminaAddress.ToString("X"), value);
        }

        public static void setFlag1(Socket socket, USBBot bot, int slot, string flag)
        {
            pokeAddress(socket, bot, GetItemFlag1Address(slot), flag);
        }

        public static byte[] ReadByteArray(Socket socket, long initAddr, int size)
        {
            //try
            //{
            // Read in small chunks
            byte[] result = new byte[size];
            const int maxBytesToReceive = 1536;
            int received = 0;
            int bytesToReceive;
            while (received < size)
            {
                bytesToReceive = (size - received > maxBytesToReceive) ? maxBytesToReceive : size - received;
                string bufferRepr = ReadToIntermediateString(socket, initAddr + received, bytesToReceive);
                if (bufferRepr == null)
                {
                    return null;
                }
                for (int i = 0; i < bytesToReceive; i++)
                {
                    result[received + i] = Convert.ToByte(bufferRepr.Substring(i * 2, 2), 16);
                }
                received += bytesToReceive;
            }
            return result;
            /*}
            catch
            {
                MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                formControl.ClearRefresh();
                return null;
            }*/
        }
        public static byte[] ReadByteArray(Socket socket, long initAddr, int size, ref int counter)
        {
            try
            {
                // Read in small chunks
                byte[] result = new byte[size];
                const int maxBytesToReceive = 1536;
                int received = 0;
                int bytesToReceive;
                while (received < size)
                {
                    bytesToReceive = (size - received > maxBytesToReceive) ? maxBytesToReceive : size - received;
                    string bufferRepr = ReadToIntermediateString(socket, initAddr + received, bytesToReceive);
                    for (int i = 0; i < bytesToReceive; i++)
                    {
                        result[received + i] = Convert.ToByte(bufferRepr.Substring(i * 2, 2), 16);
                    }
                    received += bytesToReceive;
                    counter++;
                }
                return result;
            }
            catch
            {
                MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                return null;
            }
        }

        /*
        public static bool SendByteArray(Socket socket, long initAddr, byte[] buffer, int size, ref int counter)
        {
            // Send in small chunks
            const int maxBytesTosend = 1536;
            int sent = 0;
            int bytesToSend = 0;
            StringBuilder dataTemp = new StringBuilder();
            string msg;
            while (sent < size)
            {
                dataTemp.Clear();
                bytesToSend = (size - sent > maxBytesTosend) ? maxBytesTosend : size - sent;
                for (int i = 0; i < bytesToSend; i++)
                {
                    dataTemp.Append(String.Format("{0:X2}", buffer[sent + i]));
                }
                msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", initAddr + sent, dataTemp.ToString());
                SendString(socket, Encoding.UTF8.GetBytes(msg));
                sent += bytesToSend;
                counter++;
            }

            return false;
        }

        public static bool SendByteArray(Socket socket, long initAddr, byte[] buffer, int size)
        {
            // Send in small chunks
            const int maxBytesTosend = 1536;
            int sent = 0;
            int bytesToSend = 0;
            StringBuilder dataTemp = new StringBuilder();
            string msg;
            while (sent < size)
            {
                dataTemp.Clear();
                bytesToSend = (size - sent > maxBytesTosend) ? maxBytesTosend : size - sent;
                for (int i = 0; i < bytesToSend; i++)
                {
                    dataTemp.Append(String.Format("{0:X2}", buffer[sent + i]));
                }
                msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", initAddr + sent, dataTemp.ToString());
                //Debug.Print(msg);
                SendString(socket, Encoding.UTF8.GetBytes(msg));
                sent += bytesToSend;
            }

            return false;
        }
        */

        private static string ReadToIntermediateString(Socket socket, long address, int size)
        {
            //try
            //{
            string msg = String.Format("peek 0x{0:X8} {1}\r\n", address, size);
            //Debug.Print(msg);
            SendString(socket, Encoding.UTF8.GetBytes(msg));
            byte[] b = new byte[size * 2 + 64];
            int first_rec = ReceiveString(socket, b);
            //Debug.Print(String.Format("Received {0} Bytes", first_rec));
            return Encoding.ASCII.GetString(b, 0, size * 2);
            /*}
            catch
            {
                MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                formControl.ClearRefresh();
                return null;
            }*/
        }

        public static void ReadUInt64Array(Socket socket, long initAddr, UInt64[] buffer, int size, int offset = 0)
        {
            try
            {
                // Read in small chunks
                const int maxBytesToReceive = 1536;  // Absolutely needs to be multiple of 4
                int received = 0;
                int bytesToReceive;
                while (received < size)
                {
                    bytesToReceive = (size - received > maxBytesToReceive) ? maxBytesToReceive : size - received;
                    string bufferRepr = ReadToIntermediateString(socket, initAddr + received, bytesToReceive);
                    for (int i = 0; i < (bytesToReceive / 4); i++)
                    {
                        buffer[offset + (received / 4) + i] = Convert.ToUInt32(bufferRepr.Substring(i * 8 + 6, 2) +
                                                    bufferRepr.Substring(i * 8 + 4, 2) +
                                                    bufferRepr.Substring(i * 8 + 2, 2) +
                                                    bufferRepr.Substring(i * 8, 2), 16);
                    }
                    received += bytesToReceive;
                }
            }
            catch
            {
                MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
            }
        }

        public static bool SendUInt32Array(Socket socket, long initAddr, UInt32[] buffer, int size, int offset = 0)
        {
            // Send in small chunks
            const int maxUInt32Tosend = 125;
            size /= 4;
            int sent = 0;
            int UInt32ToSend;
            StringBuilder dataTemp = new StringBuilder();
            string msg;
            while (sent < size)
            {
                dataTemp.Clear();
                UInt32ToSend = (size - sent > maxUInt32Tosend) ? maxUInt32Tosend : size - sent;
                for (int i = 0; i < UInt32ToSend; i++)
                {
                    dataTemp.Append(String.Format("{0:X2}{1:X2}{2:X2}{3:X2}",
                        (buffer[offset + sent + i] & 0xFF), (buffer[offset + sent + i] & 0xFF00) >> 8,
                        (buffer[offset + sent + i] & 0xFF0000) >> 16, (buffer[offset + sent + i] & 0xFF000000) >> 24));
                }
                msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", initAddr + sent * 4, dataTemp.ToString());
                Debug.Print(msg);
                SendString(socket, Encoding.UTF8.GetBytes(msg));
                sent += UInt32ToSend;
            }

            return false;
        }

        public static void SendString(Socket socket, byte[] buffer, int offset = 0, int size = 0, int timeout = 100)
        {
            int startTickCount = Environment.TickCount;
            int sent = 0;  // how many bytes is already sent
            if (size == 0)
                for (int i = offset; i < buffer.Length; i++)
                    if (buffer[i] == 0xA)
                    {
                        size = i + 1 - offset;
                        break;
                    }
            if (size == 0) size = buffer.Length - offset;
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Timeout.");
                try
                {
                    sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably full, wait and try again
                        //Thread.Sleep(10);
                    }
                    else
                        throw ex;  // any serious error occurr
                }
            } while (sent < size);
        }

        public static int ReceiveString(Socket socket, byte[] buffer, int offset = 0, int size = 0, int timeout = 30000)
        {
            int startTickCount = Environment.TickCount;
            int received = 0;  // how many bytes is already received
            if (size == 0) size = buffer.Length - offset;
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                    throw new Exception("Timeout.");
                try
                {
                    received += socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                        ex.SocketErrorCode == SocketError.IOPending ||
                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably empty, wait and try again
                        //Thread.Sleep(30);
                    }
                    else
                        throw ex;  // any serious error occurr
                }
            } while (received < size && buffer[received - 1] != 0xA);
            return received;
        }

        public static byte[] GetTownID(Socket socket, USBBot bot)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : TownID " + TownNameddress.ToString("X"));

                    byte[] b = ReadByteArray(socket, TownNameddress, 0x1C);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n TownNameddress");
                    }
                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : TownID " + TownNameddress.ToString("X"));

                    byte[] b = bot.ReadBytes(TownNameddress, 0x1C);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n TownNameddress");
                    }
                    return b;
                }
            }
        }

        public static byte[] GetWeatherSeed(Socket socket, USBBot bot)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : WeatherSeed " + weatherSeed.ToString("X"));

                    byte[] b = ReadByteArray(socket, weatherSeed, 0x4);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n WeatherSeed");
                    }
                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : WeatherSeed " + weatherSeed.ToString("X"));

                    byte[] b = bot.ReadBytes(weatherSeed, 0x4);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n WeatherSeed");
                    }
                    return b;
                }
            }
        }

        public static byte[] getReaction(Socket socket, USBBot bot, int player)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        /*
                        string msg = String.Format("peek 0x{0:X8} {1}\r\n", reactionAddress.ToString("X"), 8);
                        //Debug.Print("Peek Reaction : " + msg);
                        SendString(socket, Encoding.UTF8.GetBytes(msg));

                        byte[] b = new byte[4096];
                        socket.Receive(b);
                        */
                        Debug.Print("[Sys] Peek : Reaction " + (playerReactionAddress + (player * playerOffset)).ToString("X"));

                        byte[] b = ReadByteArray(socket, (playerReactionAddress + (player * playerOffset)), 8);

                        if (b == null)
                        {
                            MessageBox.Show("Wait something is wrong here!? \n\n Reaction ");
                        }

                        return b;
                    }
                    else
                    {
                        Debug.Print("[Usb] Peek : Reaction " + (playerReactionAddress + (player * playerOffset)).ToString("X"));

                        byte[] b = bot.ReadBytes((uint)(playerReactionAddress + (player * playerOffset)), 8);

                        if (b == null)
                        {
                            MessageBox.Show("Wait something is wrong here!? \n\n Reaction");
                        }

                        return b;
                    }

                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                    return null;
                }
            }
        }

        public static void setReaction(Socket socket, USBBot bot, int player, string reactionFirstHalf, string reactionSecondHalf)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        string msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (playerReactionAddress + (player * playerOffset)).ToString("x"), reactionFirstHalf);
                        Debug.Print("Poke Reaction: " + msg);
                        SendString(socket, Encoding.UTF8.GetBytes(msg));

                        msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", ((playerReactionAddress + (player * playerOffset)) + 4).ToString("x"), reactionSecondHalf);
                        Debug.Print("Poke Reaction: " + msg);
                        SendString(socket, Encoding.UTF8.GetBytes(msg));
                    }
                    else
                    {
                        bot.WriteBytes(stringToByte(reactionFirstHalf), (uint)(playerReactionAddress + (player * playerOffset)));

                        bot.WriteBytes(stringToByte(reactionSecondHalf), (uint)((playerReactionAddress + (player * playerOffset)) + 4));
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static void SendSpawnRate(Socket socket, USBBot bot, byte[] buffer, int index, int type, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    if (type == 0)
                    {
                        SendByteArray8(socket, InsectAppearPointer + InsectDataSize * index + 0x2, buffer, 12 * 6 * 2, ref counter);
                    }
                    else if (type == 1)
                    {
                        SendByteArray8(socket, FishRiverAppearPointer + FishDataSize * index + 0x2, buffer, 78, ref counter);
                    }
                    else if (type == 2)
                    {
                        SendByteArray8(socket, FishSeaAppearPointer + FishDataSize * index + 0x2, buffer, 78, ref counter);
                    }
                    else if (type == 3)
                    {
                        SendByteArray8(socket, CreatureSeaAppearPointer + SeaCreatureDataSize * index + 0x2, buffer, 78, ref counter);
                    }
                }
                else
                {
                    if (type == 0)
                    {
                        bot.WriteBytes(buffer, (uint)(InsectAppearPointer + InsectDataSize * index + 0x2));
                    }
                    else if (type == 1)
                    {
                        bot.WriteBytes(buffer, (uint)(FishRiverAppearPointer + FishDataSize * index + 0x2));
                    }
                    else if (type == 2)
                    {
                        bot.WriteBytes(buffer, (uint)(FishSeaAppearPointer + FishDataSize * index + 0x2));
                    }
                    else if (type == 3)
                    {
                        bot.WriteBytes(buffer, (uint)(CreatureSeaAppearPointer + SeaCreatureDataSize * index + 0x2));
                    }
                }
            }
        }

        public static byte[] GetCritterData(Socket socket, USBBot bot, int mode)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    if (mode == 0)
                    {
                        Debug.Print("[Sys] Peek : Insect " + InsectAppearPointer.ToString("X") + " " + InsectDataSize * InsectNumRecords);
                        return ReadByteArray(socket, InsectAppearPointer, InsectDataSize * InsectNumRecords);
                    }
                    else if (mode == 1)
                    {
                        Debug.Print("[Sys] Peek : FishRiver " + FishRiverAppearPointer.ToString("X") + " " + FishDataSize * FishRiverNumRecords);
                        return ReadByteArray(socket, FishRiverAppearPointer, FishDataSize * FishRiverNumRecords);
                    }
                    else if (mode == 2)
                    {
                        Debug.Print("[Sys] Peek : FishSea " + FishSeaAppearPointer.ToString("X") + " " + FishDataSize * FishSeaNumRecords);
                        return ReadByteArray(socket, FishSeaAppearPointer, FishDataSize * FishSeaNumRecords);
                    }
                    else if (mode == 3)
                    {
                        Debug.Print("[Sys] Peek : CreatureSea " + CreatureSeaAppearPointer.ToString("X") + " " + SeaCreatureDataSize * SeaCreatureNumRecords);
                        return ReadByteArray(socket, CreatureSeaAppearPointer, SeaCreatureDataSize * SeaCreatureNumRecords);
                    }
                    return null;
                }
                else
                {
                    if (mode == 0)
                    {
                        Debug.Print("[Usb] Peek : Insect " + InsectAppearPointer.ToString("X") + " " + InsectDataSize * InsectNumRecords);
                        return ReadLargeBytes(bot, InsectAppearPointer, InsectDataSize * InsectNumRecords);
                    }
                    else if (mode == 1)
                    {
                        Debug.Print("[Usb] Peek : FishRiver " + FishRiverAppearPointer.ToString("X") + " " + FishDataSize * FishRiverNumRecords);
                        return ReadLargeBytes(bot, FishRiverAppearPointer, FishDataSize * FishRiverNumRecords);
                    }
                    else if (mode == 2)
                    {
                        Debug.Print("[Usb] Peek : FishSea " + FishSeaAppearPointer.ToString("X") + " " + FishDataSize * FishSeaNumRecords);
                        return ReadLargeBytes(bot, FishSeaAppearPointer, FishDataSize * FishSeaNumRecords);
                    }
                    else if (mode == 3)
                    {
                        Debug.Print("[Usb] Peek : CreatureSea " + CreatureSeaAppearPointer.ToString("X") + " " + SeaCreatureDataSize * SeaCreatureNumRecords);
                        return ReadLargeBytes(bot, CreatureSeaAppearPointer, SeaCreatureDataSize * SeaCreatureNumRecords);
                    }
                    return null;
                }
            }
        }

        private static byte[] ReadLargeBytes(USBBot bot, uint address, int size)
        {
            // Read in small chunks
            byte[] result = new byte[size];
            const int maxBytesToReceive = 468;
            int received = 0;
            int bytesToReceive;
            while (received < size)
            {
                bytesToReceive = (size - received > maxBytesToReceive) ? maxBytesToReceive : size - received;
                byte[] buffer = bot.ReadBytes((uint)(address + received), bytesToReceive);
                for (int i = 0; i < bytesToReceive; i++)
                {
                    result[received + i] = buffer[i];
                }
                received += bytesToReceive;
            }
            return result;
        }

        private static byte[] ReadLargeBytes(USBBot bot, uint address, int size, ref int counter)
        {
            // Read in small chunks
            byte[] result = new byte[size];
            const int maxBytesToReceive = 468;
            int received = 0;
            int bytesToReceive;
            while (received < size)
            {
                bytesToReceive = (size - received > maxBytesToReceive) ? maxBytesToReceive : size - received;
                byte[] buffer = bot.ReadBytes((uint)(address + received), bytesToReceive);
                for (int i = 0; i < bytesToReceive; i++)
                {
                    result[received + i] = buffer[i];
                }
                received += bytesToReceive;
                counter++;
            }
            return result;
        }

        private static void WriteLargeBytes(USBBot bot, long initAddr, byte[] buffer, int size, ref int counter)
        {

            const int maxBytesTosend = 468;
            int sent = 0;
            int bytesToSend = 0;
            byte[] temp;
            while (sent < size)
            {
                bytesToSend = (size - sent > maxBytesTosend) ? maxBytesTosend : size - sent;
                temp = new byte[bytesToSend];
                for (int i = 0; i < bytesToSend; i++)
                {
                    temp[i] = buffer[sent + i];
                }
                /*
                for (int i = 0; i < bytesToSend; i++)
                {
                    dataTemp.Append(String.Format("{0:X2}", buffer[sent + i]));
                }
                msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", initAddr + sent, dataTemp.ToString());
                */
                //Debug.Print(msg);
                //SendString(socket, Encoding.UTF8.GetBytes(msg));
                bot.WriteBytes(temp, (uint)(initAddr + sent));
                sent += bytesToSend;
                counter++;
            }
        }

        public static byte[] GetVillager(Socket socket, USBBot bot, int num, int size, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : Villager " + (VillagerAddress + (num * VillagerSize)).ToString("X") + " " + num + " " + size);

                    byte[] b = ReadByteArray(socket, VillagerAddress + (num * VillagerSize), size, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Villager");
                    }

                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : Villager " + (VillagerAddress + (num * VillagerSize)).ToString("X") + " " + num + " " + size);

                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerAddress + (num * VillagerSize)), size, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Villager");
                    }

                    return b;
                }
            }
        }

        public static byte[] GetVillager(Socket socket, USBBot bot, int num, int size)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    byte[] b = ReadByteArray(socket, VillagerAddress + (num * VillagerSize), size);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Villager");
                    }

                    return b;
                }
                else
                {
                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerAddress + (num * VillagerSize)), size);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Villager");
                    }

                    return b;
                }
            }
        }

        public static void LoadVillager(Socket socket, USBBot bot, int num, byte[] villager, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    SendByteArray8(socket, VillagerAddress + (num * VillagerSize), villager, (int)VillagerSize, ref counter);

                    //SendByteArray(socket, VillagerAddress + (num * VillagerSize) + VillagerHouseBufferDiff, villager, (int)VillagerSize, ref counter);
                }
                else
                {
                    WriteLargeBytes(bot, VillagerAddress + (num * VillagerSize), villager, (int)VillagerSize, ref counter);

                    //WriteLargeBytes(bot, VillagerAddress + (num * VillagerSize) + VillagerHouseBufferDiff, villager, (int)VillagerSize, ref counter);
                }
            }
        }

        public static byte[] GetMoveout(Socket socket, USBBot bot, int num, int size, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    //Debug.Print("[Sys] Peek : Moveout " + (VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset).ToString("X") + " " + size);

                    byte[] b = ReadByteArray(socket, VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset, size, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Moveout");
                    }

                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : Moveout " + (VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset).ToString("X") + " " + size);

                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset), size, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Moveout");
                    }

                    return b;
                }
            }
        }

        public static byte[] GetMoveout(Socket socket, USBBot bot, int num, int size)
        {
            lock (botLock)
            {
                if (bot == null)
                {

                    byte[] b = ReadByteArray(socket, VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset, size);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Moveout");
                    }

                    return b;
                }
                else
                {

                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset), size);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Moveout");
                    }

                    return b;
                }
            }
        }

        public static void SetMoveout(Socket socket, USBBot bot, int num, byte[] flagData, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    SendByteArray8(socket, VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset, flagData, flagData.Length, ref counter);
                }
                else
                {
                    WriteLargeBytes(bot, VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset, flagData, flagData.Length, ref counter);
                }
            }
        }

        public static byte[] GetHouse(Socket socket, USBBot bot, int num, ref int counter, uint diff = 0)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : House " + (VillagerHouseAddress + (num * (VillagerHouseSize)) + diff).ToString("X") + " " + (int)VillagerHouseSize);

                    byte[] b = ReadByteArray(socket, VillagerHouseAddress + (num * (VillagerHouseSize)) + diff, (int)VillagerHouseSize, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n House");
                    }

                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : House " + (VillagerHouseAddress + (num * (VillagerHouseSize)) + diff).ToString("X") + " " + (int)VillagerHouseSize);

                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerHouseAddress + (num * (VillagerHouseSize)) + diff), (int)VillagerHouseSize);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n House");
                    }

                    return b;
                }
            }
        }

        public static void LoadHouse(Socket socket, USBBot bot, int num, byte[] house, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    SendByteArray8(socket, VillagerHouseAddress + (num * (VillagerHouseSize)), house, (int)VillagerHouseSize, ref counter);

                    SendByteArray8(socket, VillagerHouseAddress + (num * (VillagerHouseSize)) + VillagerHouseBufferDiff, house, (int)VillagerHouseSize, ref counter);
                }
                else
                {
                    WriteLargeBytes(bot, VillagerHouseAddress + (num * (VillagerHouseSize)), house, (int)VillagerHouseSize, ref counter);

                    WriteLargeBytes(bot, VillagerHouseAddress + (num * (VillagerHouseSize)) + VillagerHouseBufferDiff, house, (int)VillagerHouseSize, ref counter);
                }
            }
        }

        public static byte GetHouseOwner(Socket socket, USBBot bot, int num, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : HouseOwner " + (VillagerHouseAddress + (num * (VillagerHouseSize)) + VillagerHouseOwnerOffset).ToString("X"));

                    byte[] b = ReadByteArray(socket, VillagerHouseAddress + (num * (VillagerHouseSize)) + VillagerHouseOwnerOffset, 1, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n HouseOwner");
                        return 0xDD;
                    }

                    return b[0];
                }
                else
                {
                    Debug.Print("[Usb] Peek : HouseOwner " + (VillagerHouseAddress + (num * (VillagerHouseSize)) + VillagerHouseOwnerOffset).ToString("X"));

                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerHouseAddress + (num * (VillagerHouseSize)) + VillagerHouseOwnerOffset), 1, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n HouseOwner");
                        return 0xDD;
                    }

                    return b[0];
                }
            }
        }

        public static byte GetHouseOwner(Socket socket, USBBot bot, int num)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    byte[] b = ReadByteArray(socket, VillagerHouseAddress + (num * (VillagerHouseSize)) + VillagerHouseOwnerOffset, 1);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n HouseOwner");
                        return 0xDD;
                    }

                    return b[0];
                }
                else
                {
                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerHouseAddress + (num * (VillagerHouseSize)) + VillagerHouseOwnerOffset), 1);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n HouseOwner");
                        return 0xDD;
                    }

                    return b[0];
                }
            }
        }

        public static byte[] GetCatchphrase(Socket socket, USBBot bot, int num, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : Catchphrase " + (VillagerAddress + (num * VillagerSize) + VillagerCatchphraseOffset).ToString("X"));

                    byte[] b = ReadByteArray(socket, VillagerAddress + (num * VillagerSize) + VillagerCatchphraseOffset, 0x2C, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Catchphrase");
                    }

                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : Catchphrase " + (VillagerAddress + (num * VillagerSize) + VillagerCatchphraseOffset).ToString("X"));

                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerAddress + (num * VillagerSize) + VillagerCatchphraseOffset), 0x2C, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Catchphrase");
                    }

                    return b;
                }
            }
        }

        public static void SetCatchphrase(Socket socket, USBBot bot, int num, byte[] pharse)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        string msg;

                        msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (num * VillagerSize) + VillagerCatchphraseOffset).ToString("X"), ByteToHexString(pharse));
                        Debug.Print("Poke Catchphrase: " + msg);
                        SendString(socket, Encoding.UTF8.GetBytes(msg));

                        //msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (num * VillagerSize) + VillagerCatchphraseOffset + VillagerHouseBufferDiff).ToString("X"), ByteToHexString(pharse));
                        //Debug.Print("Poke Catchphrase: " + msg);
                        //SendString(socket, Encoding.UTF8.GetBytes(msg));
                    }
                    else
                    {
                        bot.WriteBytes(pharse, (uint)(VillagerAddress + (num * VillagerSize) + VillagerCatchphraseOffset));

                        //bot.WriteBytes(pharse, (uint)(VillagerAddress + (num * VillagerSize) + VillagerCatchphraseOffset + VillagerHouseBufferDiff));
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static byte GetVillagerFlag(Socket socket, USBBot bot, int num, uint offset)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : VillagerFlag " + (VillagerAddress + (num * VillagerSize) + offset).ToString("X"));

                    byte[] b = ReadByteArray(socket, VillagerAddress + (num * VillagerSize) + offset, 1);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n VillagerFlag");
                    }

                    return b[0];
                }
                else
                {
                    Debug.Print("[Usb] Peek : VillagerFlag " + (VillagerAddress + (num * VillagerSize) + offset).ToString("X"));

                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerAddress + (num * VillagerSize) + offset), 1);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n VillagerFlag");
                    }

                    return b[0];
                }
            }
        }

        public static byte GetVillagerHouseFlag(Socket socket, USBBot bot, int num, uint offset, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : VillagerHouseFlag " + (VillagerHouseAddress + (num * (VillagerHouseSize)) + offset).ToString("X"));

                    byte[] b = ReadByteArray(socket, VillagerHouseAddress + (num * (VillagerHouseSize)) + offset, 1, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n VillagerHouseFlag");
                    }

                    return b[0];
                }
                else
                {
                    Debug.Print("[Usb] Peek : VillagerHouseFlag " + (VillagerHouseAddress + (num * (VillagerHouseSize)) + offset).ToString("X"));

                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerHouseAddress + (num * (VillagerHouseSize)) + offset), 1, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n VillagerHouseFlag");
                    }

                    return b[0];
                }
            }
        }

        public static int FindHouseIndex(int VillagerNum, int[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == VillagerNum)
                    return i;
            }
            return -1;
        }

        public static void SetMoveout(Socket socket, USBBot bot, int num, string MoveoutFlag = "2", string ForceMoveoutFlag = "1")
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        string msg;

                        msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset).ToString("X"), MoveoutFlag);
                        Debug.Print("Poke Moveout: " + msg);
                        SendString(socket, Encoding.UTF8.GetBytes(msg));

                        //msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset + VillagerHouseBufferDiff).ToString("X"), MoveoutFlag);
                        //Debug.Print("Poke Moveout: " + msg);
                        //SendString(socket, Encoding.UTF8.GetBytes(msg));

                        msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (num * VillagerSize) + VillagerForceMoveoutOffset).ToString("X"), ForceMoveoutFlag);
                        Debug.Print("Poke ForceMoveout: " + msg);
                        SendString(socket, Encoding.UTF8.GetBytes(msg));

                        //msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (num * VillagerSize) + VillagerForceMoveoutOffset + VillagerHouseBufferDiff).ToString("X"), ForceMoveoutFlag);
                        //Debug.Print("Poke ForceMoveout: " + msg);
                        //SendString(socket, Encoding.UTF8.GetBytes(msg));

                        msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (num * VillagerSize) + VillagerAbandonHouseOffset).ToString("X"), "0");
                        Debug.Print("Poke AbandonHouse: " + msg);
                        SendString(socket, Encoding.UTF8.GetBytes(msg));

                        //msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (num * VillagerSize) + VillagerAbandonHouseOffset + VillagerHouseBufferDiff).ToString("X"), "0");
                        //Debug.Print("Poke AbandonHouse: " + msg);
                        //SendString(socket, Encoding.UTF8.GetBytes(msg));
                    }
                    else
                    {
                        bot.WriteBytes(stringToByte(MoveoutFlag), (uint)(VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset));

                        //bot.WriteBytes(stringToByte(MoveoutFlag), (uint)(VillagerAddress + (num * VillagerSize) + VillagerMoveoutOffset + VillagerHouseBufferDiff));

                        bot.WriteBytes(stringToByte(ForceMoveoutFlag), (uint)(VillagerAddress + (num * VillagerSize) + VillagerForceMoveoutOffset));

                        //bot.WriteBytes(stringToByte(ForceMoveoutFlag), (uint)(VillagerAddress + (num * VillagerSize) + VillagerForceMoveoutOffset + VillagerHouseBufferDiff));

                        bot.WriteBytes(stringToByte("0"), (uint)(VillagerAddress + (num * VillagerSize) + VillagerAbandonHouseOffset));

                        //bot.WriteBytes(stringToByte("0"), (uint)(VillagerAddress + (num * VillagerSize) + VillagerAbandonHouseOffset + VillagerHouseBufferDiff));
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static void SetFriendship(Socket socket, USBBot bot, int num, int player, string FriendshipFlag = "FF")
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        string msg;
                        msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset) + VillagerFriendshipOffset).ToString("X"), FriendshipFlag);
                        Debug.Print("Poke Friendship: " + msg);
                        SendString(socket, Encoding.UTF8.GetBytes(msg));

                        //msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset) + VillagerFriendshipOffset + VillagerHouseBufferDiff).ToString("X"), FriendshipFlag);
                        //Debug.Print("Poke Friendship: " + msg);
                        //SendString(socket, Encoding.UTF8.GetBytes(msg));
                    }
                    else
                    {
                        bot.WriteBytes(stringToByte(FriendshipFlag), (uint)(VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset) + VillagerFriendshipOffset));

                        //bot.WriteBytes(stringToByte(FriendshipFlag), (uint)(VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset) + VillagerFriendshipOffset + VillagerHouseBufferDiff));
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static byte[] GetPlayerDataVillager(Socket socket, USBBot bot, int num, int player, int size, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : Villager " + player + " " + (VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset)).ToString("X") + " " + num + " " + size);

                    byte[] b = ReadByteArray(socket, VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset), size, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Villager");
                    }

                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : Villager " + player + " " + (VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset)).ToString("X") + " " + num + " " + size);

                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset)), size, ref counter);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Villager");
                    }

                    return b;
                }
            }
        }

        public static byte[] GetPlayerDataVillager(Socket socket, USBBot bot, int num, int player, int size)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : Villager " + player + " " + (VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset)).ToString("X") + " " + num + " " + size);

                    byte[] b = ReadByteArray(socket, VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset), size);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Villager");
                    }

                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : Villager " + player + " " + (VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset)).ToString("X") + " " + num + " " + size);

                    byte[] b = ReadLargeBytes(bot, (uint)(VillagerAddress + (num * VillagerSize) + (player * VillagerPlayerOffset)), size);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Villager");
                    }

                    return b;
                }
            }
        }

        public static void SetMysVillager(Socket socket, USBBot bot, byte[] buffer, byte[] species, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    SendByteArray8(socket, MysIslandVillagerAddress, buffer, buffer.Length, ref counter);
                    SendByteArray8(socket, MysIslandVillagerSpecies, species, species.Length, ref counter);
                }
                else
                {
                    bot.WriteBytes(buffer, MysIslandVillagerAddress);
                    bot.WriteBytes(species, MysIslandVillagerSpecies);
                }
            }
        }

        public static byte[] GetMysVillagerName(Socket socket, USBBot bot)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : MysVillager " + MysIslandVillagerAddress.ToString("X"));

                    byte[] b = ReadByteArray(socket, MysIslandVillagerAddress, 8);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n MysVillagerName");
                    }
                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : MysVillager " + MysIslandVillagerAddress.ToString("X"));

                    byte[] b = bot.ReadBytes(MysIslandVillagerAddress, 8);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n MysVillagerName");
                    }
                    return b;
                }
            }
        }

        public static string GetVillagerInternalName(byte Species, byte Variant)
        {
            int s = Convert.ToInt32(Species);
            int v = Convert.ToInt32(Variant);
            return $"{(VillagerSpecies)Species}{Variant:00}";
        }
        public static string GetVillagerRealName(byte Species, byte Variant)
        {
            string internalName = GetVillagerInternalName(Species, Variant);
            if (RealName.ContainsKey(internalName))
                return RealName[internalName];
            else
                return "ERROR";
        }

        public static string GetVillagerRealName(string IName)
        {
            if (RealName.ContainsKey(IName))
                return RealName[IName];
            else
                return "ERROR";
        }

        public static string GetVillagerImage(string name)
        {
            string path = imagePath + villagerPath + name + ".png";
            if (File.Exists(path))
                return path;
            else
            {
                path = imagePath + villagerPath + MissingImage;
                if (File.Exists(path))
                    return path;
                else
                    return "";
            }
        }

        public static void dropItem(Socket socket, USBBot bot, long address, string itemId, string count, string flag1, string flag2)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        SendByteArray8(socket, address, stringToByte(buildDropStringLeft(itemId, count, flag1, flag2)), 16);
                        SendByteArray8(socket, address + mapOffset, stringToByte(buildDropStringLeft(itemId, count, flag1, flag2)), 16);

                        SendByteArray8(socket, address + 0x600, stringToByte(buildDropStringRight(itemId)), 16);
                        SendByteArray8(socket, address + 0x600 + mapOffset, stringToByte(buildDropStringRight(itemId)), 16);

                        Debug.Print("Drop: " + address + " " + itemId + " " + count + " " + flag1 + " " + flag2);
                    }
                    else
                    {

                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static void deleteFloorItem(Socket socket, USBBot bot, long address)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        SendByteArray8(socket, address, stringToByte(buildDropStringLeft("FFFE", "00000000", "00", "00", true)), 16);
                        SendByteArray8(socket, address + mapOffset, stringToByte(buildDropStringLeft("FFFE", "00000000", "00", "00", true)), 16);

                        SendByteArray8(socket, address + 0x600, stringToByte(buildDropStringRight("FFFE", true)), 16);
                        SendByteArray8(socket, address + 0x600 + mapOffset, stringToByte(buildDropStringRight("FFFE", true)), 16);

                        Debug.Print("Delete: " + address);
                    }
                    else
                    {

                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static byte[] getMapLayer(Socket socket, USBBot bot, long address, ref int counter)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        Debug.Print("[Sys] Peek : Map Layer " + address.ToString("X"));

                        byte[] b = ReadByteArray8(socket, address, (int)mapSize, ref counter);

                        if (b == null)
                        {
                            MessageBox.Show("Wait something is wrong here!? \n\n Map Layer");
                        }
                        return b;
                    }
                    else
                    {
                        Debug.Print("[Usb] Peek : Map Layer " + address.ToString("X"));

                        byte[] b = ReadLargeBytes(bot, (uint)address, (int)mapSize, ref counter);

                        if (b == null)
                        {
                            MessageBox.Show("Wait something is wrong here!? \n\n Map Layer");
                        }
                        return b;
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                    return null;
                }
            }
        }

        public static byte[] getAcre(Socket socket, USBBot bot)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        Debug.Print("[Sys] Peek : Acre " + AcreOffset.ToString("X"));

                        byte[] b = ReadByteArray(socket, AcreOffset, AcreAndPlaza);

                        if (b == null)
                        {
                            MessageBox.Show("Wait something is wrong here!? \n\n Acre");
                        }
                        return b;
                    }
                    else
                    {
                        Debug.Print("[Usb] Peek : Acre " + AcreOffset.ToString("X"));

                        byte[] b = bot.ReadBytes(AcreOffset, AcreAndPlaza);

                        if (b == null)
                        {
                            MessageBox.Show("Wait something is wrong here!? \n\n Acre");
                        }
                        return b;
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                    return null;
                }
            }
        }

        public static void sendAcre(Socket socket, USBBot bot, byte[] acre, ref int counter)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        Debug.Print("[Sys] Poke : Acre " + AcreOffset.ToString("X"));

                        SendByteArray8(socket, AcreOffset, acre, acre.Length, ref counter);
                        SendByteArray8(socket, AcreOffset + mapOffset, acre, acre.Length, ref counter);
                    }
                    else
                    {
                        Debug.Print("[Usb] Poke : Acre " + AcreOffset.ToString("X"));

                        WriteLargeBytes(bot, AcreOffset, acre, acre.Length, ref counter);
                        WriteLargeBytes(bot, AcreOffset + mapOffset, acre, acre.Length, ref counter);
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static void sendPlaza(Socket socket, USBBot bot, byte[] plaza, ref int counter)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        Debug.Print("[Sys] Poke : Plaza " + (AcreOffset + 0x94).ToString("X"));

                        SendByteArray8(socket, AcreOffset + 0x94, plaza, plaza.Length, ref counter);
                        SendByteArray8(socket, AcreOffset + 0x94 + mapOffset, plaza, plaza.Length, ref counter);
                    }
                    else
                    {
                        Debug.Print("[Usb] Poke : Plaza " + (AcreOffset + 0x94).ToString("X"));

                        WriteLargeBytes(bot, AcreOffset + 0x94, plaza, plaza.Length, ref counter);
                        WriteLargeBytes(bot, AcreOffset + 0x94 + mapOffset, plaza, plaza.Length, ref counter);
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static void sendBuilding(Socket socket, USBBot bot, byte[] building, ref int counter)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        Debug.Print("[Sys] Poke : Building " + BuildingOffset.ToString("X"));

                        SendByteArray8(socket, BuildingOffset, building, building.Length, ref counter);
                        SendByteArray8(socket, BuildingOffset + mapOffset, building, building.Length, ref counter);
                    }
                    else
                    {
                        Debug.Print("[Usb] Poke : Building " + BuildingOffset.ToString("X"));

                        WriteLargeBytes(bot, BuildingOffset, building, building.Length, ref counter);
                        WriteLargeBytes(bot, BuildingOffset + mapOffset, building, building.Length, ref counter);
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static byte[] getBuilding(Socket socket, USBBot bot)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        Debug.Print("[Sys] Peek : Building " + BuildingOffset.ToString("X"));

                        byte[] b = ReadByteArray(socket, BuildingOffset, AllBuildingSize);

                        if (b == null)
                        {
                            MessageBox.Show("Wait something is wrong here!? \n\n Building");
                        }
                        return b;
                    }
                    else
                    {
                        Debug.Print("[Usb] Peek : Building " + BuildingOffset.ToString("X"));

                        byte[] b = bot.ReadBytes(BuildingOffset, AllBuildingSize);

                        if (b == null)
                        {
                            MessageBox.Show("Wait something is wrong here!? \n\n Building");
                        }
                        return b;
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                    return null;
                }
            }
        }

        public static byte[] getCoordinate(Socket socket, USBBot bot)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : Coordinate " + coordinate.ToString("X"));

                    byte[] b = ReadByteArray(socket, coordinate, 8);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Coordinate");
                    }
                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : Coordinate " + coordinate.ToString("X"));

                    byte[] b = bot.ReadBytes(coordinate, 8);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Coordinate");
                    }
                    return b;
                }
            }
        }

        public static byte[] getSaving(Socket socket, USBBot bot = null)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : Save " + savingOffset.ToString("X"));

                    byte[] b = ReadByteArray(socket, savingOffset, 32);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Save");
                    }
                    return b;
                }
                else
                {
                    Debug.Print("[Usb] Peek : Save " + savingOffset.ToString("X"));

                    byte[] b = bot.ReadBytes(savingOffset, 32);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Save");
                    }
                    return b;
                }
            }
        }

        public static void dropColumn(Socket socket, USBBot bot, uint address1, uint address2, byte[] buffer1, byte[] buffer2, ref int counter)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    SendByteArray8(socket, address1, buffer1, buffer1.Length, ref counter);
                    SendByteArray8(socket, address1 + mapOffset, buffer1, buffer1.Length, ref counter);
                    SendByteArray8(socket, address2, buffer2, buffer2.Length, ref counter);
                    SendByteArray8(socket, address2 + mapOffset, buffer2, buffer2.Length, ref counter);
                }
                else
                {
                    WriteLargeBytes(bot, address1, buffer1, buffer1.Length, ref counter);
                    WriteLargeBytes(bot, address1 + mapOffset, buffer1, buffer1.Length, ref counter);
                    WriteLargeBytes(bot, address2, buffer2, buffer2.Length, ref counter);
                    WriteLargeBytes(bot, address2 + mapOffset, buffer2, buffer2.Length, ref counter);
                }
            }
        }

        public static void dropColumn2(Socket socket, USBBot bot, uint address1, uint address2, byte[] buffer1, byte[] buffer2)
        {
            lock (botLock)
            {
                if (bot == null)
                {
                    SendByteArray8(socket, address1, buffer1, buffer1.Length);
                    SendByteArray8(socket, address1 + mapOffset, buffer1, buffer1.Length);
                    SendByteArray8(socket, address2, buffer2, buffer2.Length);
                    SendByteArray8(socket, address2 + mapOffset, buffer2, buffer2.Length);
                }
                else
                {

                }
            }
        }

        public static string buildDropStringLeft(string itemId, string count, string flag1, string flag2, Boolean empty = false)
        {
            string partID = "FDFF0000";
            if (empty || itemId == "FFFE")
                return flip(itemId) + flag2 + flag1 + flip(count) + flip(itemId) + "0000" + "0000" + "00" + "00";
            else
                return flip(itemId) + flag2 + flag1 + flip(count) + partID + flip(itemId) + "00" + "01";
        }
        public static string buildDropStringRight(string itemId, Boolean empty = false)
        {
            string partID = "FDFF0000";
            if (empty || itemId == "FFFE")
                return flip(itemId) + "0000" + "0000" + "00" + "00" + flip(itemId) + "0000" + "0000" + "00" + "00";
            else
                return partID + flip(itemId) + "01" + "00" + partID + flip(itemId) + "01" + "01";
        }

        public static byte[] ReadByteArray8(Socket socket, long initAddr, int size, ref int counter)
        {
            lock (botLock)
            {
                // Read in small chunks
                byte[] result = new byte[size];
                const int maxBytesToReceive = 8192;
                int received = 0;
                int bytesToReceive;
                while (received < size)
                {
                    bytesToReceive = (size - received > maxBytesToReceive) ? maxBytesToReceive : size - received;
                    string bufferRepr = ReadToIntermediateString8(socket, initAddr + received, bytesToReceive);
                    if (bufferRepr == null)
                        return null;
                    for (int i = 0; i < bytesToReceive; i++)
                    {
                        result[received + i] = Convert.ToByte(bufferRepr.Substring(i * 2, 2), 16);
                    }
                    received += bytesToReceive;
                    counter++;
                }
                return result;
            }
        }

        public static byte[] ReadByteArray8(Socket socket, long initAddr, int size)
        {
            lock (botLock)
            {
                // Read in small chunks
                byte[] result = new byte[size];
                const int maxBytesToReceive = 8192;
                int received = 0;
                int bytesToReceive;
                while (received < size)
                {
                    bytesToReceive = (size - received > maxBytesToReceive) ? maxBytesToReceive : size - received;
                    string bufferRepr = ReadToIntermediateString8(socket, initAddr + received, bytesToReceive);
                    if (bufferRepr == null)
                        return null;
                    for (int i = 0; i < bytesToReceive; i++)
                    {
                        result[received + i] = Convert.ToByte(bufferRepr.Substring(i * 2, 2), 16);
                    }
                    received += bytesToReceive;
                }
                return result;
            }
        }

        public static void SendByteArray8(Socket socket, long initAddr, byte[] buffer, int size, ref int counter)
        {
            lock (botLock)
            {
                const int maxBytesTosend = 8192;
                int sent = 0;
                int bytesToSend = 0;
                StringBuilder dataTemp = new StringBuilder();
                string msg;
                while (sent < size)
                {
                    dataTemp.Clear();
                    bytesToSend = (size - sent > maxBytesTosend) ? maxBytesTosend : size - sent;
                    for (int i = 0; i < bytesToSend; i++)
                    {
                        dataTemp.Append(String.Format("{0:X2}", buffer[sent + i]));
                    }
                    msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", initAddr + sent, dataTemp.ToString());
                    //Debug.Print(msg);
                    SendString(socket, Encoding.UTF8.GetBytes(msg));
                    sent += bytesToSend;
                    counter++;
                }
            }
        }

        public static void SendByteArray8(Socket socket, long initAddr, byte[] buffer, int size)
        {
            lock (botLock)
            {
                const int maxBytesTosend = 8192;
                int sent = 0;
                int bytesToSend = 0;
                StringBuilder dataTemp = new StringBuilder();
                string msg;
                while (sent < size)
                {
                    dataTemp.Clear();
                    bytesToSend = (size - sent > maxBytesTosend) ? maxBytesTosend : size - sent;
                    for (int i = 0; i < bytesToSend; i++)
                    {
                        dataTemp.Append(String.Format("{0:X2}", buffer[sent + i]));
                    }
                    msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", initAddr + sent, dataTemp.ToString());
                    //Debug.Print(msg);
                    SendString(socket, Encoding.UTF8.GetBytes(msg));
                    sent += bytesToSend;
                }
            }
        }

        private static string ReadToIntermediateString8(Socket socket, long address, int size)
        {
            lock (botLock)
            {
                string msg = String.Format("peek 0x{0:X8} {1}\r\n", address, size);
                //Debug.Print(msg);
                SendString(socket, Encoding.UTF8.GetBytes(msg));
                byte[] b = new byte[size * 2 + 64];
                int first_rec = ReceiveString(socket, b);
                //Debug.Print(String.Format("Received {0} Bytes", first_rec));
                return Encoding.ASCII.GetString(b, 0, size * 2);
            }
        }

        public static byte[] getVisitorName(Socket socket)
        {
            lock (botLock)
            {

                byte[] b = ReadByteArray(socket, VisitorNameAddress, 20);
                //Debug.Print("[Sys] Peek Visitor Name : " + VisitorNameAddress.ToString("X") + " " + "24");
                if (b == null)
                {
                    MessageBox.Show("Wait something is wrong here!? \n\n peek " + VisitorNameAddress.ToString("X"));
                }

                return b;
            }
        }

        public static string getDodo(Socket socket, bool chi = false, USBBot bot = null)
        {
            lock (botLock)
            {
                byte[] b;

                if (bot == null)
                {
                    Debug.Print("[Sys] Peek : Dodo " + dodoAddress.ToString("X"));
                    if (chi)
                        b = ReadByteArray(socket, dodoAddress + ChineseLanguageOffset, 5);
                    else
                        b = ReadByteArray(socket, dodoAddress, 5);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Dodo");
                        return "";
                    }
                }
                else
                {
                    Debug.Print("[Usb] Peek : Dodo " + dodoAddress.ToString("X"));

                    b = bot.ReadBytes(dodoAddress, 5);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Dodo");
                        return "";
                    }
                }

                return Encoding.ASCII.GetString(b);
            }
        }

        public static void sendBlankName(Socket socket)
        {
            lock (botLock)
            {
                byte[] empty = new byte[20];
                SendByteArray8(socket, VisitorNameAddress, empty, 20);
                Debug.Print("Send Blank Name");
            }
        }

        public static void SetTextSpeed(Socket socket, USBBot bot, bool chi)
        {
            lock (botLock)
            {
                try
                {
                    if (bot == null)
                    {
                        string msg;

                        if (chi)
                        {
                            msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (TextSpeedAddress + ChineseLanguageOffset).ToString("X"), "3");
                            Debug.Print("Poke TextSpeedChi: " + msg);
                            SendString(socket, Encoding.UTF8.GetBytes(msg));
                        }
                        else
                        {
                            msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", TextSpeedAddress.ToString("X"), "3");
                            Debug.Print("Poke TextSpeed: " + msg);
                            SendString(socket, Encoding.UTF8.GetBytes(msg));
                        }
                    }
                    else
                    {
                        if (chi)
                        {
                            bot.WriteBytes(stringToByte("3"), TextSpeedAddress);
                        }
                        else
                        {
                            bot.WriteBytes(stringToByte("3"), TextSpeedAddress + ChineseLanguageOffset);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Exception, try restarting the program or reconnecting to the switch.");
                }
            }
        }

        public static string TrimFromZero(string input) => TrimFromFirst(input, '\0');

        private static string TrimFromFirst(string input, char c)
        {
            int index = input.IndexOf(c);
            return index < 0 ? input : input.Substring(0, index);
        }

        public static string GetString(byte[] data, int offset, int maxLength)
        {
            var str = Encoding.Unicode.GetString(data, offset, maxLength * 2);
            return TrimFromZero(str);
        }


        public static byte[] GetBytes(string value, int maxLength)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength);
            else if (value.Length < maxLength)
                value = value.PadRight(maxLength, '\0');
            return Encoding.Unicode.GetBytes(value);
        }

        public static byte[] Slice(byte[] src, int offset, int length)
        {
            byte[] data = new byte[length];
            Buffer.BlockCopy(src, offset, data, 0, data.Length);
            return data;
        }

        public static byte[] add(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, c, 0, a.Length);
            Buffer.BlockCopy(b, 0, c, a.Length, b.Length);

            return c;
        }

        public static void overrideAddresses(Dictionary<string, UInt32> config)
        {
            masterAddress = config["PlayerSlot"];

            ItemSlotBase = masterAddress;
            ItemSlot21Base = masterAddress - 0xB8;

            player1SlotBase = config["PlayerSlot"];
            playerOffset = config["PlayerOffset"];
            Slot21Offset = config["Slot21Offset"];
            HomeOffset = config["HomeOffset"];
            ReactionOffset = config["ReactionOffset"];
            VillagerAddress = config["Villager"];
            VillagerSize = config["VillagerOffset"];
            VillagerHouseAddress = config["VillagerHouse"];
            VillagerHouseSize = config["VillagerHouseOffset"];
            VillagerHouseBufferDiff = config["VillagerHouseBuffer"];
            MasterRecyclingBase = config["RecyclingBin"];
            TurnipPurchasePriceAddr = config["Turnip"];
            staminaAddress = config["Stamina"];
            wSpeedAddress = config["WalkSpeed"];
            aSpeedAddress = config["AnimationSpeed"];
            CollisionAddress = config["Collision"];
            freezeTimeAddress = config["FreezeTime"];
            readTimeAddress = config["ReadTime"];
            weatherSeed = config["WeatherSeed"];
            mapZero = config["MapZero"];


            MasterRecycling21Base = MasterRecyclingBase + 0xA0;
            TurnipSellPriceAddr = TurnipPurchasePriceAddr + 0xC;

            player1Slot21Base = player1SlotBase - Slot21Offset;
            player1HouseBase = player1SlotBase + HomeOffset;
            player1House21Base = player1HouseBase + 0xA0;

            playerReactionAddress = player1SlotBase + ReactionOffset;

            player2SlotBase = player1SlotBase + playerOffset;
            player2Slot21Base = player2SlotBase - Slot21Offset;
            player2HouseBase = player2SlotBase + HomeOffset;
            player2House21Base = player2HouseBase + 0xA0;

            player3SlotBase = player2SlotBase + playerOffset;
            player3Slot21Base = player3SlotBase - Slot21Offset;
            player3HouseBase = player3SlotBase + HomeOffset;
            player3House21Base = player3HouseBase + 0xA0;

            player4SlotBase = player3SlotBase + playerOffset;
            player4Slot21Base = player4SlotBase - Slot21Offset;
            player4HouseBase = player4SlotBase + HomeOffset;
            player4House21Base = player4HouseBase + 0xA0;

            player5SlotBase = player4SlotBase + playerOffset;
            player5Slot21Base = player5SlotBase - Slot21Offset;
            player5HouseBase = player5SlotBase + HomeOffset;
            player5House21Base = player5HouseBase + 0xA0;

            player6SlotBase = player5SlotBase + playerOffset;
            player6Slot21Base = player6SlotBase - Slot21Offset;
            player6HouseBase = player6SlotBase + HomeOffset;
            player6House21Base = player6HouseBase + 0xA0;

            player7SlotBase = player6SlotBase + playerOffset;
            player7Slot21Base = player7SlotBase - Slot21Offset;
            player7HouseBase = player7SlotBase + HomeOffset;
            player7House21Base = player7HouseBase + 0xA0;

            player8SlotBase = player7SlotBase + playerOffset;
            player8Slot21Base = player8SlotBase - Slot21Offset;
            player8HouseBase = player8SlotBase + HomeOffset;
            player8House21Base = player8HouseBase + 0xA0;
        }

        private static readonly Encoding Encoder = Encoding.UTF8;
        private static byte[] Encode(string command, bool addrn = true) => Encoder.GetBytes(addrn ? command + "\r\n" : command);

        public static byte[] Version() => Encode("getVersion");

        public static byte[] Freeze(uint offset, byte[] data) => Encode($"freeze 0x{offset:X8} 0x{string.Concat(data.Select(z => $"{z:X2}"))}");

        public static byte[] UnFreeze(uint offset) => Encode($"unFreeze 0x{offset:X8}");

        public static byte[] FreezeCount() => Encode("freezeCount");

        public static byte[] FreezeClear() => Encode("freezeClear");

        public static byte[] FreezeRate(string rate) => Encode("configure freezeRate " + rate);

        public static void FreezeBig(Socket socket, uint offset, byte[] data, uint size)
        {
            uint max = 0x2000;

            if (size <= max)
                Utilities.SendString(socket, Freeze(offset, data));
            else
            {
                byte[] current = new byte[max];
                byte[] remain = new byte[size - max];

                Buffer.BlockCopy(data, 0, current, 0, (int)max);
                Buffer.BlockCopy(data, (int)max, remain, 0, (int)(size - max));

                Utilities.SendString(socket, Freeze(offset, current));

                FreezeBig(socket, offset + max, remain, size - max);
            }
        }

        public static void unFreezeBig(Socket socket, uint offset, uint size)
        {
            uint max = 0x2000;

            if (size <= max)
                Utilities.SendString(socket, UnFreeze(offset));
            else
            {
                Utilities.SendString(socket, UnFreeze(offset));

                unFreezeBig(socket, offset + max, size - max);
            }
        }

        public static string getVersion(Socket socket)
        {
            lock (botLock)
            {
                byte[] b = new byte[20];

                Debug.Print("[Sys] GetVersion");

                SendString(socket, Version());
                ReceiveString(socket, b);

                return TrimFromZero(Encoding.UTF8.GetString(b).Replace("\n", String.Empty));
            }
        }

        public static int GetFreezeCount(Socket socket)
        {
            lock (botLock)
            {
                byte[] b = new byte[3];

                Debug.Print("[Sys] GetFreezeCount");
                Thread.Sleep(250);
                SendString(socket, FreezeCount());
                ReceiveString(socket, b);

                return ConvertHexByteStringToBytes(b)[0];
            }
        }

        public static byte[] ConvertHexByteStringToBytes(byte[] bytes)
        {
            var dest = new byte[bytes.Length / 2];
            for (int i = 0; i < dest.Length; i++)
            {
                int ofs = i * 2;
                var _0 = (char)bytes[ofs + 0];
                var _1 = (char)bytes[ofs + 1];
                dest[i] = DecodeTuple(_0, _1);
            }
            return dest;
        }

        private static byte DecodeTuple(char _0, char _1)
        {
            byte result;
            if (IsNum(_0))
                result = (byte)((_0 - '0') << 4);
            else if (IsHexUpper(_0))
                result = (byte)((_0 - 'A' + 10) << 4);
            else
                throw new ArgumentOutOfRangeException(nameof(_0));

            if (IsNum(_1))
                result |= (byte)(_1 - '0');
            else if (IsHexUpper(_1))
                result |= (byte)(_1 - 'A' + 10);
            else
                throw new ArgumentOutOfRangeException(nameof(_1));
            return result;
        }

        private static bool IsNum(char c) => (uint)(c - '0') <= 9;
        private static bool IsHexUpper(char c) => (uint)(c - 'A') <= 5;
        public static bool IsConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }

        public static string GetVisitorName(Socket socket, USBBot bot, int i)
        {
            lock (botLock)
            {
                byte[] b;

                if (bot == null)
                {

                    b = ReadByteArray(socket, VisitorList + i * VisitorListSize, 20);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Dodo");
                        return "";
                    }
                }
                else
                {
                    b = bot.ReadBytes((uint)(VisitorList + i * VisitorListSize), 20);

                    if (b == null)
                    {
                        MessageBox.Show("Wait something is wrong here!? \n\n Dodo");
                        return "";
                    }
                }
                //Debug.Print("Byte : " + i + " " + ByteToHexString(b));
                string tempName = Encoding.Unicode.GetString(b, 0, 20);
                return tempName.Replace("\0", string.Empty);
            }
        }

        public static string GetJsonSetting(string path, string key)
        {
            JObject o = JObject.Parse(File.ReadAllText(path));
            var value = o.SelectToken(key);
            if (value == null)
                return string.Empty;
            else
                return value.ToString();
        }

        public static string getChannelId(string channelName)
        {
            HttpWebRequest rq = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/users?login=" + channelName + "&api_version=5");
            rq.Method = "GET";
            rq.Headers.Add("20", "application/vnd.twitchtv.v5+json");
            rq.Headers["Client-ID"] = "roo8qlza39yjj8tzgzhcrsxrbpwqaw";

            string target = string.Empty;

            HttpWebResponse resp = (HttpWebResponse)rq.GetResponse();
            try
            {
                StreamReader streamReader = new StreamReader(resp.GetResponseStream(), true);
                try
                {
                    target = streamReader.ReadToEnd();
                }
                finally
                {
                    streamReader.Close();
                }
            }
            finally
            {
                resp.Close();
            }

            JObject o = JObject.Parse(target);
            var value = o.SelectToken("users[0]._id");
            if (value == null)
                return string.Empty;
            else
                return value.ToString();
        }

        public static bool hasItemInFirstSlot(Socket socket, USBBot bot = null)
        {
            lock (botLock)
            {
                byte[] b;
                if (bot == null)
                {
                    b = ReadByteArray(socket, ItemSlotBase, 4);
                }
                else
                {
                    b = bot.ReadBytes(ItemSlotBase, 4);
                }

                if (ByteToHexString(b).Equals("FEFF0000"))
                    return false;
                else
                    return true;
            }
        }

        public static List<string> GetVillagerList(Socket socket, USBBot bot = null)
        {
            lock (botLock)
            {
                List<string> VillagerList = new List<string>();
                byte[] b;

                for (int i = 0; i < 10; i++)
                {
                    b = GetVillager(socket, bot, i, 0x2);
                    string InternalName = GetVillagerInternalName(b[0], b[1]);
                    VillagerList.Add(InternalName);
                }
                return VillagerList;
            }
        }

        public static async Task loadBoth(Socket socket, int villagerIndex, byte[] villager, int houseIndex, byte[] house)
        {
            await Task.Run(() => SendByteArray8(socket, VillagerAddress + (villagerIndex * VillagerSize), villager, (int)VillagerSize));
            await Task.Run(() => SendByteArray8(socket, VillagerHouseAddress + (houseIndex * (VillagerHouseSize)), house, (int)VillagerHouseSize));
        }

        public static async Task SetMoveout(Socket socket, int villagerIndex, string MoveoutFlag = "2", string ForceMoveoutFlag = "1")
        {
            await Task.Run(() =>
            {
                string msg;
                msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (villagerIndex * VillagerSize) + VillagerMoveoutOffset).ToString("X"), MoveoutFlag);
                SendString(socket, Encoding.UTF8.GetBytes(msg));

                msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (villagerIndex * VillagerSize) + VillagerForceMoveoutOffset).ToString("X"), ForceMoveoutFlag);
                SendString(socket, Encoding.UTF8.GetBytes(msg));

                msg = String.Format("poke 0x{0:X8} 0x{1}\r\n", (VillagerAddress + (villagerIndex * VillagerSize) + VillagerAbandonHouseOffset).ToString("X"), "0");
                SendString(socket, Encoding.UTF8.GetBytes(msg));
            });
        }

        public static bool isChinese(Socket s, USBBot bot = null)
        {
            byte[] b = Utilities.peekAddress(s, bot, Utilities.readTimeAddress, 6);
            string time = Utilities.ByteToHexString(b);

            Debug.Print(time);

            Int32 year = Convert.ToInt32(Utilities.flip(time.Substring(0, 4)), 16);
            Int32 month = Convert.ToInt32((time.Substring(4, 2)), 16);
            Int32 day = Convert.ToInt32((time.Substring(6, 2)), 16);
            Int32 hour = Convert.ToInt32((time.Substring(8, 2)), 16);
            Int32 min = Convert.ToInt32((time.Substring(10, 2)), 16);

            if (year > 3000 || month > 12 || day > 31 || hour > 24 || min > 60) //Try for Chineses
            {
                b = Utilities.peekAddress(s, bot, Utilities.readTimeAddress + Utilities.ChineseLanguageOffset, 6);
                time = Utilities.ByteToHexString(b);

                year = Convert.ToInt32(Utilities.flip(time.Substring(0, 4)), 16);
                month = Convert.ToInt32((time.Substring(4, 2)), 16);
                day = Convert.ToInt32((time.Substring(6, 2)), 16);
                hour = Convert.ToInt32((time.Substring(8, 2)), 16);
                min = Convert.ToInt32((time.Substring(10, 2)), 16);

                if (!(year > 3000 || month > 12 || day > 31 || hour > 24 || min > 60))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static string translateVariationValueBack(string input)
        {
            if (input.Length > 4)
                return "0000";

            int hexValue = Convert.ToUInt16("0x" + input, 16);

            if (hexValue < 0x8)
            {
                return Utilities.precedingZeros(input, 4);
            }
            else if (hexValue < 0x10)
            {
                return Utilities.precedingZeros((hexValue + 0x20 - 0x8).ToString("X"), 4);
            }
            else if (hexValue < 0x18)
            {
                return Utilities.precedingZeros((hexValue + 0x40 - 0x10).ToString("X"), 4);
            }
            else if (hexValue < 0x20)
            {
                return Utilities.precedingZeros((hexValue + 0x60 - 0x18).ToString("X"), 4);
            }
            else if (hexValue < 0x28)
            {
                return Utilities.precedingZeros((hexValue + 0x80 - 0x20).ToString("X"), 4);
            }
            else if (hexValue < 0x30)
            {
                return Utilities.precedingZeros((hexValue + 0xA0 - 0x28).ToString("X"), 4);
            }
            else if (hexValue < 0x38)
            {
                return Utilities.precedingZeros((hexValue + 0xC0 - 0x30).ToString("X"), 4);
            }
            else if (hexValue < 0x40)
            {
                return Utilities.precedingZeros((hexValue + 0xE0 - 0x38).ToString("X"), 4);
            }
            else
                return "0000";
        }

        #region Villager
        public enum VillagerPersonality : byte
        {
            Lazy,
            Jock,
            Cranky,
            Smug,
            Normal,
            Peppy,
            Snooty,
            Uchi,
            None,
        }
        public enum VillagerSpecies
        {
            ant = 0x0,
            bea = 0x1,
            brd = 0x2,
            bul = 0x3,
            cat = 0x4,
            cbr = 0x5,
            chn = 0x6,
            cow = 0x7,
            crd = 0x8,
            der = 0x9,
            dog = 0xA,
            duk = 0xB,
            elp = 0xC,
            flg = 0xD,
            goa = 0xE,
            gor = 0xF,
            ham = 0x10,
            hip = 0x11,
            hrs = 0x12,
            kal = 0x13,
            kgr = 0x14,
            lon = 0x15,
            mnk = 0x16,
            mus = 0x17,
            ocp = 0x18,
            ost = 0x19,
            pbr = 0x1A,
            pgn = 0x1B,
            pig = 0x1C,
            rbt = 0x1D,
            rhn = 0x1E,
            shp = 0x1F,
            squ = 0x20,
            tig = 0x21,
            wol = 0x22,
            non = 0x23,
        }

        public static Dictionary<string, byte> CheckSpecies = new Dictionary<string, byte>
        {
            {"ant", 0x0},
            {"bea", 0x1},
            {"brd", 0x2},
            {"bul", 0x3},
            {"cat", 0x4},
            {"cbr", 0x5},
            {"chn", 0x6},
            {"cow", 0x7},
            {"crd", 0x8},
            {"der", 0x9},
            {"dog", 0xA},
            {"duk", 0xB},
            {"elp", 0xC},
            {"flg", 0xD},
            {"goa", 0xE},
            {"gor", 0xF},
            {"ham", 0x10},
            {"hip", 0x11},
            {"hrs", 0x12},
            {"kal", 0x13},
            {"kgr", 0x14},
            {"lon", 0x15},
            {"mnk", 0x16},
            {"mus", 0x17},
            {"ocp", 0x18},
            {"ost", 0x19},
            {"pbr", 0x1A},
            {"pgn", 0x1B},
            {"pig", 0x1C},
            {"rbt", 0x1D},
            {"rhn", 0x1E},
            {"shp", 0x1F},
            {"squ", 0x20},
            {"tig", 0x21},
            {"wol", 0x22},
            {"non", 0x23},
        };

        public static Dictionary<string, string> RealName = new Dictionary<string, string>
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

        public static readonly Dictionary<string, int> CountByKind = new Dictionary<string, int>
        {
            {"Kind_Ftr", 1},
            {"Kind_Dishes", 1},
            {"Kind_Drink", 1},
            {"Kind_CookingMaterial", 50},
            {"Kind_RoomWall", 1},
            {"Kind_RoomFloor", 1},
            {"Kind_Rug", 1},
            {"Kind_RugMyDesign", 1},
            {"Kind_Socks", 1},
            {"Kind_Cap", 1},
            {"Kind_Helmet", 1},
            {"Kind_Accessory", 1},
            {"Kind_Bag", 1},
            {"Kind_Umbrella", 1},
            {"Kind_FtrWall", 1},
            {"Kind_Counter", 1},
            {"Kind_Pillar", 1},
            {"Kind_FishingRod", 1},
            {"Kind_Net", 1},
            {"Kind_Shovel", 1},
            {"Kind_Axe", 1},
            {"Kind_Watering", 1},
            {"Kind_Slingshot", 1},
            {"Kind_ChangeStick", 1},
            {"Kind_WoodenStickTool", 1},
            {"Kind_Ladder", 1},
            {"Kind_GroundMaker", 1},
            {"Kind_RiverMaker", 1},
            {"Kind_CliffMaker", 1},
            {"Kind_HandBag", 1},
            {"Kind_PartyPopper", 10},
            {"Kind_Ocarina", 1},
            {"Kind_Panflute", 1},
            {"Kind_Tambourine", 1},
            {"Kind_MaracasCarnival", 1},
            {"Kind_StickLight", 1},
            {"Kind_StickLightColorful", 1},
            {"Kind_Uchiwa", 1},
            {"Kind_SubToolSensu", 1},
            {"Kind_Windmill", 1},
            {"Kind_Partyhorn", 1},
            {"Kind_BlowBubble", 10},
            {"Kind_FierworkHand", 10},
            {"Kind_Balloon", 1},
            {"Kind_HandheldPennant", 1},
            {"Kind_BigbagPresent", 1},
            {"Kind_JuiceFuzzyapple", 1},
            {"Kind_Megaphone", 1},
            {"Kind_SoySet", 1},
            {"Kind_FlowerShower", 1},
            {"Kind_Candyfloss", 1},
            {"Kind_SubToolDonut", 1},
            {"Kind_SubToolEat", 1},
            {"Kind_SubToolEatRemakeable", 1},
            {"Kind_Tapioca", 1},
            {"Kind_SubToolCan", 1},
            {"Kind_Icecandy", 1},
            {"Kind_SubToolIcecream", 1},
            {"Kind_SubToolIcesoft", 1},
            {"Kind_SubToolEatDrop", 1},
            {"Kind_SubToolGeneric", 1},
            {"Kind_Basket", 1},
            {"Kind_Lantern", 1},
            {"Kind_SubToolRemakeable", 1},
            {"Kind_Timer", 1},
            {"Kind_Gyroid", 1},
            {"Kind_GyroidScrap", 1},
            {"Kind_TreeSeedling", 10},
            {"Kind_Tree", 1},
            {"Kind_BushSeedling", 10},
            {"Kind_Bush", 1},
            {"Kind_VegeSeedling", 10},
            {"Kind_VegeTree", 1},
            {"Kind_Vegetable", 10},
            {"Kind_Weed", 99},
            {"Kind_WeedLight", 50},
            {"Kind_FlowerSeed", 10},
            {"Kind_FlowerBud", 1},
            {"Kind_Flower", 10},
            {"Kind_Fruit", 10},
            {"Kind_Mushroom", 10},
            {"Kind_Turnip", 10},
            {"Kind_TurnipExpired", 1},
            {"Kind_FishBait", 10},
            {"Kind_PitFallSeed", 10},
            {"Kind_Medicine", 10},
            {"Kind_CraftMaterial", 30},
            {"Kind_CraftRemake", 50},
            {"Kind_Ore", 30},
            {"Kind_CraftPhoneCase", 1},
            {"Kind_Honeycomb", 10},
            {"Kind_Trash", 1},
            {"Kind_SnowCrystal", 10},
            {"Kind_AutumnLeaf", 10},
            {"Kind_Sakurapetal", 10},
            {"Kind_XmasDeco", 10},
            {"Kind_StarPiece", 10},
            {"Kind_Insect", 1},
            {"Kind_Fish", 1},
            {"Kind_DiveFish", 1},
            {"Kind_ShellDrift", 10},
            {"Kind_ShellFish", 1},
            {"Kind_FishToy", 1},
            {"Kind_InsectToy", 1},
            {"Kind_Fossil", 1},
            {"Kind_FossilUnknown", 1},
            {"Kind_Music", 1},
            {"Kind_MusicMiss", 1},
            {"Kind_Bromide", 1},
            {"Kind_Poster", 1},
            {"Kind_HousePost", 1},
            {"Kind_DoorDeco", 1},
            {"Kind_Fence", 50},
            {"Kind_DummyRecipe", 1},
            {"Kind_DummyDIYRecipe", 1},
            {"Kind_DummyHowtoBook", 1},
            {"Kind_LicenseItem", 1},
            {"Kind_BridgeItem", 1},
            {"Kind_SlopeItem", 1},
            {"Kind_DIYRecipe", 1},
            {"Kind_MessageBottle", 1},
            {"Kind_WrappingPaper", 10},
            {"Kind_Otoshidama", 10},
            {"Kind_HousingKit", 1},
            {"Kind_HousingKitRcoQuest", 1},
            {"Kind_HousingKitBirdge", 1},
            {"Kind_Money", 10},
            {"Kind_FireworkM", 1},
            {"Kind_BdayCupcake", 10},
            {"Kind_YutaroWisp", 5},
            {"Kind_JohnnyQuest", 10},
            {"Kind_JohnnyQuestDust", 10},
            {"Kind_PirateQuest", 10},
            {"Kind_QuestWrapping", 1},
            {"Kind_QuestChristmasPresentbox", 1},
            {"Kind_LostQuest", 1},
            {"Kind_LostQuestDust", 1},
            {"Kind_TailorTicket", 10},
            {"Kind_TreasureQuest", 1},
            {"Kind_TreasureQuestDust", 1},
            {"Kind_MilePlaneTicket", 10},
            {"Kind_RollanTicket", 5},
            {"Kind_EasterEgg", 30},
            {"Kind_LoveCrystal", 30},
            {"Kind_Candy", 30},
            {"Kind_HarvestDish", 1},
            {"Kind_Feather", 00003},
            {"Kind_RainbowFeather", 1},
            {"Kind_Vine", 30},
            {"Kind_SettingLadder", 1},
            {"Kind_SincerityTowel", 1},
            {"Kind_SouvenirChocolate", 10},
            {"Kind_Giftbox", 1},
            {"Kind_PinataStick", 1},
            {"Kind_NpcOutfit", 1},
            {"Kind_PlayerDemoOutfit", 1},
            {"Kind_Picture", 1},
            {"Kind_Sculpture", 1},
            {"Kind_PictureFake", 1},
            {"Kind_SculptureFake", 1},
            {"Kind_SmartPhone", 1},
            {"Kind_DummyFtr", 1},
            {"Kind_SequenceOnly", 1},
            {"Kind_MyDesignObject", 1},
            {"Kind_MyDesignTexture", 1},
            {"Kind_CommonFabricRug", 1},
            {"Kind_CommonFabricObject", 1},
            {"Kind_CommonFabricTexture", 1},
            {"Kind_OneRoomBox", 1},
            {"Kind_DummyWrapping", 1},
            {"Kind_DummyPresentbox", 1},
            {"Kind_DummyCardboard", 1},
            {"Kind_EventObjFtr", 1},
            {"Kind_NnpcRoomMarker", 1},
            {"Kind_PhotoStudioList", 1},
            {"Kind_ShopTorso", 1},
            {"Kind_DummyWrappingOtoshidama", 1},
            {"Kind_GardenEditList", 1},
        };
        #endregion
    }
}
