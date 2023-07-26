using DiscordWebhook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public static class Controller
    {
        private static Socket s;

        private static readonly Encoding Encoder = Encoding.UTF8;
        private static byte[] Encode(string command, bool addrn = true) => Encoder.GetBytes(addrn ? command + "\r\n" : command);
        private static byte[] X() => Encode("click X");
        private static byte[] pX() => Encode("press X");
        private static byte[] rX() => Encode("release X");
        private static byte[] Y() => Encode("click Y");
        private static byte[] pY() => Encode("press Y");
        private static byte[] rY() => Encode("release Y");
        private static byte[] A() => Encode("click A");
        private static byte[] pA() => Encode("press A");
        private static byte[] rA() => Encode("release A");
        private static byte[] B() => Encode("click B");
        private static byte[] pB() => Encode("press B");
        private static byte[] rB() => Encode("release B");

        private static byte[] L() => Encode("click L");
        private static byte[] R() => Encode("click R");
        private static byte[] ZL() => Encode("click ZL");
        private static byte[] ZR() => Encode("click ZR");

        private static byte[] PLUS() => Encode("click PLUS");
        private static byte[] MINUS() => Encode("click MINUS");

        private static byte[] Home() => Encode("click HOME");
        private static byte[] Capture() => Encode("click CAPTURE");

        private static byte[] Up() => Encode("click DUP");
        private static byte[] Right() => Encode("click DRIGHT");
        private static byte[] Down() => Encode("click DDOWN");
        private static byte[] Left() => Encode("click DLEFT");

        private static byte[] LSTICK() => Encode("click LSTICK");
        private static byte[] RSTICK() => Encode("click RSTICK");


        private static byte[] pL() => Encode("press L");
        private static byte[] rL() => Encode("release L");
        private static byte[] Detach() => Encode("detachController");

        private static byte[] LstickTR() => Encode("setStick LEFT 0x7FFF 0x7FFF");
        private static byte[] LstickTL() => Encode("setStick LEFT -0x8000 0x7FFF");
        private static byte[] LstickBR() => Encode("setStick LEFT 0x7FFF -0x8000");
        private static byte[] LstickBL() => Encode("setStick LEFT -0x8000 -0x8000");
        private static byte[] LstickU() => Encode("setStick LEFT 0x0 0x7FFF");
        private static byte[] LstickL() => Encode("setStick LEFT -0x8000 0x0");
        private static byte[] LstickD() => Encode("setStick LEFT 0x0 -0x8000");
        private static byte[] LstickR() => Encode("setStick LEFT 0x7FFF 0x0");
        private static byte[] ResetLeft() => Encode("setStick LEFT 0 0");
        private static byte[] RstickU() => Encode("setStick RIGHT 0x0 0x7FFF");
        private static byte[] RstickL() => Encode("setStick RIGHT -0x8000 0x0");
        private static byte[] RstickD() => Encode("setStick RIGHT 0x0 -0x8000");
        private static byte[] RstickR() => Encode("setStick RIGHT 0x7FFF 0x0");
        private static byte[] ResetRight() => Encode("setStick RIGHT 0 0");

        private static string IslandName = "";

        public static void Init(Socket S, string islandName)
        {
            s = S;
            IslandName = islandName;
        }

        public static void ClickA()
        {
            Utilities.SendString(s, A());
        }

        public static void PressA()
        {
            Utilities.SendString(s, pA());
        }

        public static void ReleaseA()
        {
            Utilities.SendString(s, rA());
        }

        public static void ClickB()
        {
            Utilities.SendString(s, B());
        }

        public static void PressB()
        {
            Utilities.SendString(s, pB());
        }

        public static void ReleaseB()
        {
            Utilities.SendString(s, rB());
        }

        public static void ClickX()
        {
            Utilities.SendString(s, X());
        }

        public static void PressX()
        {
            Utilities.SendString(s, pX());
        }

        public static void ReleaseX()
        {
            Utilities.SendString(s, rX());
        }

        public static void ClickY()
        {
            Utilities.SendString(s, Y());
        }

        public static void PressY()
        {
            Utilities.SendString(s, pY());
        }

        public static void ReleaseY()
        {
            Utilities.SendString(s, rY());
        }

        public static void ClickL()
        {
            Utilities.SendString(s, L());
        }
        public static void ClickR()
        {
            Utilities.SendString(s, R());
        }
        public static void ClickZL()
        {
            Utilities.SendString(s, ZL());
        }
        public static void ClickZR()
        {
            Utilities.SendString(s, ZR());
        }

        public static void ClickPLUS()
        {
            Utilities.SendString(s, PLUS());
        }
        public static void ClickMINUS()
        {
            Utilities.SendString(s, MINUS());
        }

        public static void ClickHOME()
        {
            Utilities.SendString(s, Home());
        }
        public static void ClickCAPTURE()
        {
            Utilities.SendString(s, Capture());
        }

        public static void ClickUp()
        {
            Utilities.SendString(s, Up());
        }
        public static void ClickLeft()
        {
            Utilities.SendString(s, Left());
        }
        public static void ClickDown()
        {
            Utilities.SendString(s, Down());
        }
        public static void ClickRight()
        {
            Utilities.SendString(s, Right());
        }

        public static void ClickRightStick()
        {
            Utilities.SendString(s, RSTICK());
        }
        public static void ClickLeftStick()
        {
            Utilities.SendString(s, LSTICK());
        }

        public static void PressL()
        {
            Utilities.SendString(s, pL());
        }
        public static void ReleaseL()
        {
            Utilities.SendString(s, rL());
        }

        public static void LstickTopRight()
        {
            Utilities.SendString(s, LstickTR());
        }
        public static void LstickTopLeft()
        {
            Utilities.SendString(s, LstickTL());
        }
        public static void LstickBottomRight()
        {
            Utilities.SendString(s, LstickBR());
        }
        public static void LstickBottomLeft()
        {
            Utilities.SendString(s, LstickBL());
        }

        public static void LstickUp()
        {
            Utilities.SendString(s, LstickU());
        }
        public static void LstickLeft()
        {
            Utilities.SendString(s, LstickL());
        }
        public static void LstickDown()
        {
            Utilities.SendString(s, LstickD());
        }
        public static void LstickRight()
        {
            Utilities.SendString(s, LstickR());
        }
        public static void ResetLeftStick()
        {
            Utilities.SendString(s, ResetLeft());
        }

        public static void RstickUp()
        {
            Utilities.SendString(s, RstickU());
        }
        public static void RstickLeft()
        {
            Utilities.SendString(s, RstickL());
        }
        public static void RstickDown()
        {
            Utilities.SendString(s, RstickD());
        }
        public static void RstickRight()
        {
            Utilities.SendString(s, RstickR());
        }
        public static void ResetRightStick()
        {
            Utilities.SendString(s, ResetRight());
        }

        public static void DetachController()
        {
            Utilities.SendString(s, Detach());
        }

        public static void EnterAirport()
        {
            LstickTopRight();
            Thread.Sleep(1500);
            ResetLeftStick();
            Thread.Sleep(500);
        }

        public static void ExitAirport()
        {
            LstickDown();
            Thread.Sleep(1000);
            ResetLeftStick();
            Thread.Sleep(500);
        }

        public static void Emote(int num)
        {
            ClickZR();
            Thread.Sleep(1000);
            switch (num)
            {
                case 0:
                    LstickUp();
                    break;
                case 1:
                    LstickTopRight();
                    break;
                case 2:
                    LstickRight();
                    break;
                case 3:
                    LstickBottomRight();
                    break;
                case 4:
                    LstickDown();
                    break;
                case 5:
                    LstickBottomLeft();
                    break;
                case 6:
                    LstickLeft();
                    break;
                case 7:
                    LstickTopLeft();
                    break;
            }
            Thread.Sleep(500);
            ResetLeftStick();
            Thread.Sleep(1000);
            ClickA();
            ClickA();
            ClickB();
            ClickB();
            ClickB();
        }

        public static void Skip(int before = 900, int after = 500)
        {
            Thread.Sleep(before);
            Utilities.SetTextSpeed(s, null, Utilities.IsChinese(s));
            Thread.Sleep(after);
        }

        public static string TalkAndGetDodoCode(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return "";
            ClickA(); // Talk
            Thread.Sleep(1800); // He might need to put away the stupid book
            Skip();
            if (token.IsCancellationRequested)
                return "";
            ClickA(); // End Line "Hey Hey Hey"
            Thread.Sleep(1000);
            //clickA(); // End Line "How can"
            //Thread.Sleep(1000);

            if (token.IsCancellationRequested)
                return "";
            ClickDown(); // move to "I want visitors"
            Thread.Sleep(500);

            ClickA(); // Click "I want visitors"

            if (token.IsCancellationRequested)
                return "";
            Skip(); // Thread.Sleep(3000);
            ClickA(); // End Line "You wanna"
            Thread.Sleep(1000);

            ClickDown(); // move to "Online"
            Thread.Sleep(500);
            ClickA(); // Via online play

            Skip();

            if (token.IsCancellationRequested)
                return "";
            ClickA(); // End Line "Gotcha"
            Thread.Sleep(1000);

            if (token.IsCancellationRequested)
                return "";
            ClickA(); // Roger!
            Thread.Sleep(30000); // Saving

            if (token.IsCancellationRequested)
                return "";
            ClickA(); // End Line "So who"
            Thread.Sleep(1000);


            if (token.IsCancellationRequested)
                return "";
            ClickUp(); // move to "Actually, I'm good."
            Thread.Sleep(500);
            ClickUp(); // move to "Invite via Dodo Code"
            Thread.Sleep(500);
            ClickA(); // Click "Invite via Dodo Code"

            Skip();

            ClickA(); // End Line "Dodo Code TM"
            Thread.Sleep(1000);

            ClickUp(); // move to "The more the merrier"
            Thread.Sleep(500);
            ClickA(); // Click "The more the merrier"

            Skip();

            if (token.IsCancellationRequested)
                return "";
            ClickA(); // End Line "Just so you know"
            //Thread.Sleep(1000);
            //clickA(); // End Line "You good"
            Thread.Sleep(1000);

            if (token.IsCancellationRequested)
                return "";
            ClickA(); // Click "Yeah, invite anyone"

            Thread.Sleep(6000); // fucking gate open animation

            ClickA(); // End Line "Alright"

            Skip();

            ClickA(); // End Line "Dodo"
            string dodo = SetupDodo();

            Skip();

            if (token.IsCancellationRequested)
                return dodo;
            ClickA(); // End Line "Just tell"
            Thread.Sleep(2000);
            //releaseL();

            return dodo;
        }

        public static string TalkAndGetDodoCodeLegacy(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return "";
            ReleaseL();
            Thread.Sleep(500);
            PressL(); // Speed Up
            Thread.Sleep(500);

            if (token.IsCancellationRequested)
                return "";
            ClickA(); // Talk
            Thread.Sleep(4000);
            ClickA(); // End Line "Hey Hey Hey"
            Thread.Sleep(1000);
            ClickA(); // End Line "How can"
            Thread.Sleep(1000);
            ClickDown(); // move to "I want visitors"
            Thread.Sleep(500);

            if (token.IsCancellationRequested)
                return "";
            ClickA(); // Click "I want visitors"
            Thread.Sleep(3000);
            ClickA(); // End Line "You wanna"
            Thread.Sleep(1000);

            if (token.IsCancellationRequested)
                return "";
            ClickDown(); // move to "Online"
            Thread.Sleep(500);
            ClickA(); // Via online play
            Thread.Sleep(3000);
            ClickA(); // End Line "Gotcha"
            Thread.Sleep(1000);

            if (token.IsCancellationRequested)
                return "";
            ClickA(); // Roger!
            Thread.Sleep(30000); // Saving

            ClickA(); // End Line "So who"
            Thread.Sleep(1000);

            if (token.IsCancellationRequested)
                return "";
            ClickUp(); // move to "Actually, I'm good."
            Thread.Sleep(500);
            ClickUp(); // move to "Invite via Dodo Code"
            Thread.Sleep(500);
            ClickA(); // Click "Invite via Dodo Code"
            Thread.Sleep(2000);

            ClickA(); // End Line "Dodo Code TM"
            Thread.Sleep(1000);

            if (token.IsCancellationRequested)
                return "";
            ClickUp(); // move to "The more the merrier"
            Thread.Sleep(500);
            ClickA(); // Click "The more the merrier"
            Thread.Sleep(3000);

            if (token.IsCancellationRequested)
                return "";
            ClickA(); // End Line "Just so you know"
            Thread.Sleep(1000);
            ClickA(); // End Line "You good"
            Thread.Sleep(1000);

            if (token.IsCancellationRequested)
                return "";
            ClickA(); // Click "Yeah, invite anyone"
            Thread.Sleep(5000);

            if (token.IsCancellationRequested)
                return "";
            ClickA(); // End Line "Alright"
            Thread.Sleep(5000);
            ClickA(); // End Line "Dodo"
            string dodo = SetupDodo();
            Thread.Sleep(5000);
            ClickA(); // End Line "Just tell"
            Thread.Sleep(3000);
            ReleaseL();

            return dodo;
        }

        public static void DropItem()
        {
            ClickX();
            Thread.Sleep(1000);
            ClickA();
            Thread.Sleep(500);
            ClickA();
            Thread.Sleep(1500);
            ClickB();
            Thread.Sleep(1000);
            ClickB();
            Thread.Sleep(1000);
        }

        public static void DropRecipe()
        {
            ClickX();
            Thread.Sleep(1000);
            ClickA();
            Thread.Sleep(500);
            ClickDown();
            Thread.Sleep(500);
            ClickA();
            Thread.Sleep(1500);
            ClickB();
            Thread.Sleep(1000);
            ClickB();
            Thread.Sleep(1000);
        }

        public static void TalkAndCloseGate()
        {
            ReleaseL();
            PressL(); // Speed Up

            ClickA(); // Talk "Hey there"
            Thread.Sleep(4000);
            ClickA(); // End Line "Hey there"
            Thread.Sleep(1000);

            ClickA(); // Click "Please close the gate"
            Thread.Sleep(3000);
            ClickA(); // End Line "So you want"
            Thread.Sleep(6000); //Close gate

            ClickA(); // End Line "Hope you"
            Thread.Sleep(3000);
            ReleaseL();
        }

        public static string SetupDodo()
        {
            try
            {
                //string dodo = "12345";
                string dodo = Utilities.GetDodo(s).Replace("\0", "");

                if (dodo == "") // Try again for Chinese
                    dodo = Utilities.GetDodo(s, true).Replace("\0", "");

                if (File.Exists(Utilities.dodoPath))
                {
                    foreach (string line in File.ReadLines(Utilities.dodoPath))
                    {
                        if (line == dodo)
                            return dodo;
                        else
                            break;
                    }
                }

                using (StreamWriter sw = File.CreateText(Utilities.dodoPath))
                {
                    sw.WriteLine(dodo);
                }

                if (File.Exists(Utilities.webhookPath))
                {
                    string url;
                    string content;
                    string color;
                    Color SideColor;
                    string imageURL;
                    using (StreamReader sr = new(Utilities.webhookPath))
                    {
                        url = sr.ReadLine();
                        content = sr.ReadLine();
                        color = sr.ReadLine();
                        imageURL = sr.ReadLine();
                    }

                    if (content == null)
                    {
                        content = "";
                    }
                    if (color == null)
                    {
                        SideColor = Color.Pink;
                    }
                    else
                    {
                        SideColor = ColorTranslator.FromHtml(color);
                    }
                    if (imageURL == null)
                    {
                        imageURL = "";
                    }

                    DiscordWebhook.DiscordWebhook hook = new()
                    {
                        Uri = new Uri(url)
                    };

                    DiscordMessage msg = new()
                    {
                        Content = content
                    };
                    msg.Embeds = new List<DiscordEmbed>();
                    msg.Embeds.Add(new DiscordEmbed()
                    {
                        Title = "New Dodo Code for " + IslandName + " :",
                        Description = dodo,
                        Timestamp = DateTime.Now,
                        Color = SideColor, //alpha will be ignored, you can use any RGB color
                        Thumbnail = new EmbedMedia() { Url = imageURL },
                        Footer = new EmbedFooter() { Text = "Sent From ACNHPokerCore" }
                    });

                    //message.TTS = true; //read message to everyone on the channel

                    _ = hook.SendAsync(msg);
                }

                return dodo;
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Controller", "Dodo: " + ex.Message);
                return "";
            }
        }

        public static void testWebhook()
        {
            try
            {
                string dodo = "TEST";

                using (StreamWriter sw = File.CreateText(Utilities.dodoPath))
                {
                    sw.WriteLine(dodo);
                }

                if (File.Exists(Utilities.webhookPath))
                {
                    string url;
                    string content;
                    string color;
                    Color SideColor;
                    string imageURL;
                    using (StreamReader sr = new(Utilities.webhookPath))
                    {
                        url = sr.ReadLine();
                        content = sr.ReadLine();
                        color = sr.ReadLine();
                        imageURL = sr.ReadLine();
                    }

                    if (content == null)
                    {
                        content = "";
                    }

                    if (color == null)
                    {
                        SideColor = Color.Pink;
                    }
                    else
                    {
                        SideColor = ColorTranslator.FromHtml(color);
                    }

                    if (imageURL == null)
                    {
                        imageURL = "";
                    }

                    DiscordWebhook.DiscordWebhook hook = new()
                    {
                        Uri = new Uri(url)
                    };

                    DiscordMessage msg = new()
                    {
                        Content = content
                    };
                    msg.Embeds = new List<DiscordEmbed>();
                    msg.Embeds.Add(new DiscordEmbed()
                    {
                        Title = "New Dodo Code for " + IslandName + " :",
                        Description = dodo,
                        Timestamp = DateTime.Now,
                        Color = SideColor, //alpha will be ignored, you can use any RGB color
                        Thumbnail = new EmbedMedia() { Url = imageURL },
                        Footer = new EmbedFooter() { Text = "Sent From ACNHPokerCore" }
                    });

                    //message.TTS = true; //read message to everyone on the channel

                    _ = hook.SendAsync(msg);
                }
            }
            catch (Exception ex)
            {
                MyLog.LogEvent("Controller", "Dodo: " + ex.Message);
            }
        }

        public static void ClearDodo()
        {
            string msg = "[Closed]";
            using StreamWriter sw = File.CreateText(Utilities.dodoPath);
            sw.WriteLine(msg);
        }

        public static string ChangeDodoPath()
        {
            OpenFileDialog file = new()
            {
                Filter = @"Normal text file (*.txt)|*.txt",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            string savepath;

            if (config.AppSettings.Settings["LastLoad"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + @"\save";
            else
                savepath = config.AppSettings.Settings["LastLoad"].Value;

            if (Directory.Exists(savepath))
            {
                file.InitialDirectory = savepath;
            }
            else
            {
                file.InitialDirectory = @"C:\";
            }

            if (file.ShowDialog() != DialogResult.OK)
                return "";

            string[] temp = file.FileName.Split('\\');
            string path = "";
            for (int i = 0; i < temp.Length - 1; i++)
                path = path + temp[i] + "\\";

            config.AppSettings.Settings["LastLoad"].Value = path;
            config.Save(ConfigurationSaveMode.Minimal);

            //string[] s = file.FileName.Split('\\');

            //logName.Text = s[s.Length - 1];

            Utilities.dodoPath = file.FileName;

            return file.FileName;
        }
    }
}
