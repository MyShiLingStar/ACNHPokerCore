using Discord;
using Discord.Webhook;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    class controller
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
        private static byte[] detach() => Encode("detachController");

        private static byte[] LstickTR() => Encode("setStick LEFT 0x7FFF 0x7FFF");
        private static byte[] LstickTL() => Encode("setStick LEFT -0x8000 0x7FFF");
        private static byte[] LstickBR() => Encode("setStick LEFT 0x7FFF -0x8000");
        private static byte[] LstickBL() => Encode("setStick LEFT -0x8000 -0x8000");
        private static byte[] LstickU() => Encode("setStick LEFT 0x0 0x7FFF");
        private static byte[] LstickL() => Encode("setStick LEFT -0x8000 0x0");
        private static byte[] LstickD() => Encode("setStick LEFT 0x0 -0x8000");
        private static byte[] LstickR() => Encode("setStick LEFT 0x7FFF 0x0");
        private static byte[] resetLeft() => Encode("setStick LEFT 0 0");
        private static byte[] RstickU() => Encode("setStick RIGHT 0x0 0x7FFF");
        private static byte[] RstickL() => Encode("setStick RIGHT -0x8000 0x0");
        private static byte[] RstickD() => Encode("setStick RIGHT 0x0 -0x8000");
        private static byte[] RstickR() => Encode("setStick RIGHT 0x7FFF 0x0");
        private static byte[] resetRight() => Encode("setStick RIGHT 0 0");

        private static string IslandName = "";

        public controller(Socket S, string islandName)
        {
            s = S;
            IslandName = islandName;
        }

        public static void clickA()
        {
            Utilities.SendString(s, A());
        }

        public static void pressA()
        {
            Utilities.SendString(s, pA());
        }

        public static void releaseA()
        {
            Utilities.SendString(s, rA());
        }

        public static void clickB()
        {
            Utilities.SendString(s, B());
        }

        public static void pressB()
        {
            Utilities.SendString(s, pB());
        }

        public static void releaseB()
        {
            Utilities.SendString(s, rB());
        }

        public static void clickX()
        {
            Utilities.SendString(s, X());
        }

        public static void pressX()
        {
            Utilities.SendString(s, pX());
        }

        public static void releaseX()
        {
            Utilities.SendString(s, rX());
        }

        public static void clickY()
        {
            Utilities.SendString(s, Y());
        }

        public static void pressY()
        {
            Utilities.SendString(s, pY());
        }

        public static void releaseY()
        {
            Utilities.SendString(s, rY());
        }

        public static void clickL()
        {
            Utilities.SendString(s, L());
        }
        public static void clickR()
        {
            Utilities.SendString(s, R());
        }
        public static void clickZL()
        {
            Utilities.SendString(s, ZL());
        }
        public static void clickZR()
        {
            Utilities.SendString(s, ZR());
        }

        public static void clickPLUS()
        {
            Utilities.SendString(s, PLUS());
        }
        public static void clickMINUS()
        {
            Utilities.SendString(s, MINUS());
        }

        public static void clickHOME()
        {
            Utilities.SendString(s, Home());
        }
        public static void clickCAPTURE()
        {
            Utilities.SendString(s, Capture());
        }

        public static void clickUp()
        {
            Utilities.SendString(s, Up());
        }
        public static void clickLeft()
        {
            Utilities.SendString(s, Left());
        }
        public static void clickDown()
        {
            Utilities.SendString(s, Down());
        }
        public static void clickRight()
        {
            Utilities.SendString(s, Right());
        }

        public static void clickRightStick()
        {
            Utilities.SendString(s, RSTICK());
        }
        public static void clickLeftStick()
        {
            Utilities.SendString(s, LSTICK());
        }

        public static void pressL()
        {
            Utilities.SendString(s, pL());
        }
        public static void releaseL()
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
        public static void resetLeftStick()
        {
            Utilities.SendString(s, resetLeft());
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
        public static void resetRightStick()
        {
            Utilities.SendString(s, resetRight());
        }

        public static void detachController()
        {
            Utilities.SendString(s, detach());
        }

        public static void EnterAirport()
        {
            LstickTopRight();
            Thread.Sleep(1500);
            resetLeftStick();
            Thread.Sleep(500);
        }

        public static void ExitAirport()
        {
            LstickDown();
            Thread.Sleep(1000);
            resetLeftStick();
            Thread.Sleep(500);
        }

        public static void emote(int num)
        {
            clickZR();
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
            resetLeftStick();
            Thread.Sleep(1000);
            clickA();
            clickA();
            clickB();
            clickB();
            clickB();
        }

        public static void skip(int before = 900, int after = 500)
        {
            Thread.Sleep(before);
            Utilities.SetTextSpeed(s, null, Utilities.isChinese(s));
            Thread.Sleep(after);
        }

        public static string talkAndGetDodoCode()
        {
            clickA(); // Talk
            Thread.Sleep(1800); // He might need to put away the stupid book
            skip();
            clickA(); // End Line "Hey Hey Hey"
            Thread.Sleep(1000);
            //clickA(); // End Line "How can"
            //Thread.Sleep(1000);

            clickDown(); // move to "I want visitors"
            Thread.Sleep(500);

            clickA(); // Click "I want visitors"

            skip(); // Thread.Sleep(3000);
            clickA(); // End Line "You wanna"
            Thread.Sleep(1000);

            clickDown(); // move to "Online"
            Thread.Sleep(500);
            clickA(); // Via online play

            skip();

            clickA(); // End Line "Gotcha"
            Thread.Sleep(1000);

            clickA(); // Roger!
            Thread.Sleep(30000); // Saving

            clickA(); // End Line "So who"
            Thread.Sleep(1000);



            clickUp(); // move to "Actually, I'm good."
            Thread.Sleep(500);
            clickUp(); // move to "Invite via Dodo Code"
            Thread.Sleep(500);
            clickA(); // Click "Invite via Dodo Code"

            skip();

            clickA(); // End Line "Dodo Code TM"
            Thread.Sleep(1000);

            clickUp(); // move to "The more the merrier"
            Thread.Sleep(500);
            clickA(); // Click "The more the merrier"

            skip();

            clickA(); // End Line "Just so you know"
            //Thread.Sleep(1000);
            //clickA(); // End Line "You good"
            Thread.Sleep(1000);

            clickA(); // Click "Yeah, invite anyone"

            Thread.Sleep(6000); // fucking gate open animation

            clickA(); // End Line "Alright"

            skip();

            clickA(); // End Line "Dodo"
            string dodo = setupDodo();

            skip();

            clickA(); // End Line "Just tell"
            Thread.Sleep(2000);
            //releaseL();

            return dodo;
        }

        public static string talkAndGetDodoCodeLegacy()
        {
            releaseL();
            Thread.Sleep(500);
            pressL(); // Speed Up
            Thread.Sleep(500);

            clickA(); // Talk
            Thread.Sleep(4000);
            clickA(); // End Line "Hey Hey Hey"
            Thread.Sleep(1000);
            clickA(); // End Line "How can"
            Thread.Sleep(1000);
            clickDown(); // move to "I want visitors"
            Thread.Sleep(500);

            clickA(); // Click "I want visitors"
            Thread.Sleep(3000);
            clickA(); // End Line "You wanna"
            Thread.Sleep(1000);

            clickDown(); // move to "Online"
            Thread.Sleep(500);
            clickA(); // Via online play
            Thread.Sleep(3000);
            clickA(); // End Line "Gotcha"
            Thread.Sleep(1000);

            clickA(); // Roger!
            Thread.Sleep(30000); // Saving

            clickA(); // End Line "So who"
            Thread.Sleep(1000);

            /*
            if (noBestFriend)
            {
                clickDown(); // move to "Invite via Dodo Code"
                Thread.Sleep(500);
                clickA(); // Click "Invite via Dodo Code"
                Thread.Sleep(2000);

                clickA(); // End Line "Dodo Code TM"
                Thread.Sleep(1000);

                clickDown(); // move to "The more the merrier"
                Thread.Sleep(500);
                clickA(); // Click "The more the merrier"
                Thread.Sleep(3000);
            }
            else
            {
                clickDown(); // move to "Only my Best Friend!"
                Thread.Sleep(500);
                clickDown(); // move to "Invite via Dodo Code"
                Thread.Sleep(500);
                clickA(); // Click "Invite via Dodo Code"
                Thread.Sleep(2000);

                clickA(); // End Line "Dodo Code TM"
                Thread.Sleep(1000);

                clickDown(); // move to "Only my Best Friend"
                Thread.Sleep(500);
                clickDown(); // move to "The more the merrier"
                Thread.Sleep(500);
                clickA(); // Click "The more the merrier"
                Thread.Sleep(3000);
            }*/

            clickUp(); // move to "Actually, I'm good."
            Thread.Sleep(500);
            clickUp(); // move to "Invite via Dodo Code"
            Thread.Sleep(500);
            clickA(); // Click "Invite via Dodo Code"
            Thread.Sleep(2000);

            clickA(); // End Line "Dodo Code TM"
            Thread.Sleep(1000);

            clickUp(); // move to "The more the merrier"
            Thread.Sleep(500);
            clickA(); // Click "The more the merrier"
            Thread.Sleep(3000);

            clickA(); // End Line "Just so you know"
            Thread.Sleep(1000);
            clickA(); // End Line "You good"
            Thread.Sleep(1000);

            clickA(); // Click "Yeah, invite anyone"
            Thread.Sleep(5000);

            clickA(); // End Line "Alright"
            Thread.Sleep(5000);
            clickA(); // End Line "Dodo"
            string dodo = setupDodo();
            Thread.Sleep(5000);
            clickA(); // End Line "Just tell"
            Thread.Sleep(3000);
            releaseL();

            return dodo;
        }

        public static void dropItem()
        {
            clickX();
            Thread.Sleep(1000);
            clickA();
            Thread.Sleep(500);
            clickA();
            Thread.Sleep(1500);
            clickB();
            Thread.Sleep(1000);
            clickB();
            Thread.Sleep(1000);
        }

        public static void dropRecipe()
        {
            clickX();
            Thread.Sleep(1000);
            clickA();
            Thread.Sleep(500);
            clickDown();
            Thread.Sleep(500);
            clickA();
            Thread.Sleep(1500);
            clickB();
            Thread.Sleep(1000);
            clickB();
            Thread.Sleep(1000);
        }

        public static void talkAndCloseGate()
        {
            releaseL();
            pressL(); // Speed Up

            clickA(); // Talk "Hey there"
            Thread.Sleep(4000);
            clickA(); // End Line "Hey there"
            Thread.Sleep(1000);

            clickA(); // Click "Please close the gate"
            Thread.Sleep(3000);
            clickA(); // End Line "So you want"
            Thread.Sleep(6000); //Close gate

            clickA(); // End Line "Hope you"
            Thread.Sleep(3000);
            releaseL();
        }

        public static string setupDodo()
        {
            try
            {
                //string dodo = "12345";
                string dodo = Utilities.getDodo(s).Replace("\0", "");

                if (dodo == "") // Try again for Chinese
                    dodo = Utilities.getDodo(s, true).Replace("\0", "");

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
                    using (StreamReader sr = new StreamReader(Utilities.webhookPath))
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
                        SideColor = System.Drawing.ColorTranslator.FromHtml(color);
                    }
                    if (imageURL == null)
                    {
                        imageURL = "";
                    }

                    DiscordWebhook hook = new DiscordWebhook();
                    hook.Url = url;

                    DiscordMessage message = new DiscordMessage();
                    message.Content = content;
                    //message.TTS = true; //read message to everyone on the channel

                    //embeds
                    DiscordEmbed embed = new DiscordEmbed();
                    embed.Title = "New Dodo Code for " + IslandName + " :";
                    embed.Description = dodo;
                    embed.Timestamp = DateTime.Now;
                    embed.Color = SideColor; //alpha will be ignored, you can use any RGB color
                    embed.Thumbnail = new EmbedMedia() { Url = imageURL };
                    embed.Footer = new EmbedFooter() { Text = "Sent From ACNHPoker" };

                    message.Embeds = new[] { embed };
                    try
                    {
                        hook.Send(message);
                    }
                    catch
                    {

                    }
                }

                return dodo;
            }
            catch (Exception ex)
            {
                MyLog.logEvent("Controller", "Dodo: " + ex.Message.ToString());
                return "";
            }
        }

        public static void clearDodo()
        {
            string msg = "[Closed]";
            using (StreamWriter sw = File.CreateText(Utilities.dodoPath))
            {
                sw.WriteLine(msg);
            }
        }

        public static string changeDodoPath()
        {
            OpenFileDialog file = new OpenFileDialog()
            {
                Filter = "Normal text file (*.txt)|*.txt",
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

            string[] s = file.FileName.Split('\\');

            //logName.Text = s[s.Length - 1];

            Utilities.dodoPath = file.FileName;

            return file.FileName;
        }
    }
}
