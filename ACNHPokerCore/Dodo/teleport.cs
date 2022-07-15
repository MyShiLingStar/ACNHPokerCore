using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public class teleport
    {
        private static Socket s;

        private static byte[] teleportByte;
        private static byte[] anchorByte;

        private static string offset = "[[[[main+4627088]+18]+178]+D0]+DA"; //"[[[[main+460ED68]+18]+178]+D0]+DA"; //"[[[[main+3A33980]+18]+178]+D0]+DA"; //"[[[[main+3A32980]+18]+178]+D0]+DA"; //"[[[[main+3A08B40]+18]+178]+D0]+DA"; //"[[[[main+39DC030]+18]+178]+D0]+DA";//"[[[[main+398C380]+18]+178]+D0]+DA";//"[[[[main+396F5A0]+18]+178]+D0]+DA";

        private static int coordinateSize = 20;
        private static int turningSize = 4;
        private static int teleportSize = coordinateSize + turningSize;

        private static object lockObject = new object();
        public enum OverworldState
        {
            Null,
            OverworldOrInAirport,
            Loading,
            UserArriveLeavingOrTitleScreen,
            Unknown,
            ItemDropping
        }

        public enum LocationState
        {
            Loading,
            Indoor,
            Announcement,
            Unknown
        }

        public teleport(Socket S)
        {
            if (!File.Exists(Utilities.teleportPath))
            {
                byte[] teleportByte = new byte[teleportSize * 10];
                File.WriteAllBytes(Utilities.teleportPath, teleportByte);
            }
            if (!File.Exists(Utilities.anchorPath))
            {
                byte[] anchorByte = new byte[teleportSize * 5];
                File.WriteAllBytes(Utilities.anchorPath, anchorByte);
            }

            s = S;

            teleportByte = File.ReadAllBytes(Utilities.teleportPath);
            anchorByte = File.ReadAllBytes(Utilities.anchorPath);

            if (anchorByte.Length < 120)
            {
                MyMessageBox.Show("It seems you are using a smaller \"Anchors.bin\"... \nOr your \"Anchors.bin\" is totally corrupted.", "Bigger is not always better", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FixAnchorByte();
            }
        }

        public static ulong GetCoordinateAddress(string strInput)
        {
            lock (lockObject)
            {
                // Regex pattern to get operators and offsets from pointer expression.	
                string pattern = @"(\+|\-)([A-Fa-f0-9]+)";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(strInput);

                // Get first offset from pointer expression and read address at that offset from main start.	
                var ofs = Convert.ToUInt64(match.Groups[2].Value, 16);
                var address = BitConverter.ToUInt64(Utilities.peekMainAddress(s, ofs.ToString("X"), 0x8), 0);
                match = match.NextMatch();

                // Matches the rest of the operators and offsets in the pointer expression.	
                while (match.Success)
                {
                    // Get operator and offset from match.	
                    string opp = match.Groups[1].Value;
                    ofs = Convert.ToUInt64(match.Groups[2].Value, 16);

                    // Add or subtract the offset from the current stored address based on operator in front of offset.	
                    switch (opp)
                    {
                        case "+":
                            address += ofs;
                            break;
                        case "-":
                            address -= ofs;
                            break;
                    }

                    // Attempt another match and if successful read bytes at address and store the new address.	
                    match = match.NextMatch();
                    if (match.Success)
                    {
                        byte[] bytes = Utilities.peekAbsoluteAddress(s, address.ToString("X"), 0x8);
                        address = BitConverter.ToUInt64(bytes, 0);
                    }
                }

                return address;
            }
        }

        public static Boolean TeleportTo(int num)
        {
            byte[] coordinate = new byte[coordinateSize];
            byte[] turning = new byte[turningSize];

            Buffer.BlockCopy(teleportByte, teleportSize * num, coordinate, 0, coordinateSize);
            Buffer.BlockCopy(teleportByte, teleportSize * num + coordinateSize, turning, 0, turningSize);

            if (!isValidCoordinate(coordinate))
            {
                MyMessageBox.Show("Invalid Coordinates!", "3C3C1D119440927", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            ulong address = GetCoordinateAddress(offset);

            int trials = 0;

            do
            {
                if (trials > 5)
                {
                    return false;
                }
                Utilities.pokeAbsoluteAddress(s, (address - 0x2).ToString("X"), Utilities.ByteToHexString(coordinate));
                Utilities.pokeAbsoluteAddress(s, (address + 0x3A).ToString("X"), Utilities.ByteToHexString(turning));
                Thread.Sleep(500);
                trials++;
            }
            while (confirmSuccessTeleport(num, teleportByte));

            return true;
        }

        public static Boolean TeleportToAnchor(int num)
        {
            byte[] coordinate = new byte[coordinateSize];
            byte[] turning = new byte[turningSize];

            Buffer.BlockCopy(anchorByte, teleportSize * num, coordinate, 0, coordinateSize);
            Buffer.BlockCopy(anchorByte, teleportSize * num + coordinateSize, turning, 0, turningSize);

            ulong address = GetCoordinateAddress(offset);

            int trials = 0;

            do
            {
                if (trials > 10)
                {
                    return false;
                }
                Utilities.pokeAbsoluteAddress(s, (address - 0x2).ToString("X"), Utilities.ByteToHexString(coordinate));
                Utilities.pokeAbsoluteAddress(s, (address + 0x3A).ToString("X"), Utilities.ByteToHexString(turning));
                Thread.Sleep(500);
                trials++;
            }
            while (confirmSuccessTeleport(num, anchorByte));

            return true;
        }

        private static bool confirmSuccessTeleport(int num, byte[] ByteUsing)
        {
            byte[] coordinate = new byte[coordinateSize];
            byte[] turning = new byte[turningSize];

            Buffer.BlockCopy(ByteUsing, teleportSize * num, coordinate, 0, coordinateSize);
            Buffer.BlockCopy(ByteUsing, teleportSize * num + coordinateSize, turning, 0, turningSize);

            ulong address = GetCoordinateAddress(offset);

            byte[] CurCoordinate = Utilities.peekAbsoluteAddress(s, (address - 0x2).ToString("X"), coordinateSize);
            byte[] CurTurning = Utilities.peekAbsoluteAddress(s, (address + 0x3A).ToString("X"), turningSize);

            //Debug.Print(Utilities.ByteToHexString(Utilities.add(CurCoordinate, CurTurning)));

            if (Utilities.add(CurCoordinate, CurTurning).SequenceEqual(Utilities.add(coordinate, turning)))
                return false;
            else
                return true;
        }

        private static bool isValidCoordinate(byte[] coordinate)
        {
            for (int i = 0; i < coordinate.Length; i++)
            {
                if (coordinate[i] != 0x0)
                    return true;
            }
            return false;
        }

        public static bool allAnchorValid()
        {
            byte[] temp = new byte[teleportSize];

            if (anchorByte == null)
                return false;

            for (int i = 0; i < 5; i++)
            {
                Buffer.BlockCopy(anchorByte, teleportSize * i, temp, 0, teleportSize);
                if (!isValidCoordinate(temp))
                    return false;
            }
            return true;
        }

        public static void SetTeleport(int num)
        {
            ulong address = GetCoordinateAddress(offset);

            byte[] CurCoordinate = Utilities.peekAbsoluteAddress(s, (address - 0x2).ToString("X"), coordinateSize);
            byte[] CurTurning = Utilities.peekAbsoluteAddress(s, (address + 0x3A).ToString("X"), turningSize);

            Buffer.BlockCopy(CurCoordinate, 0, teleportByte, teleportSize * num, coordinateSize);
            Buffer.BlockCopy(CurTurning, 0, teleportByte, teleportSize * num + coordinateSize, turningSize);

            File.WriteAllBytes(Utilities.teleportPath, teleportByte);
        }

        public static void SetAnchor(int num)
        {
            ulong address = GetCoordinateAddress(offset);

            byte[] CurCoordinate = Utilities.peekAbsoluteAddress(s, (address - 0x2).ToString("X"), coordinateSize);
            byte[] CurTurning = Utilities.peekAbsoluteAddress(s, (address + 0x3A).ToString("X"), turningSize);

            Buffer.BlockCopy(CurCoordinate, 0, anchorByte, teleportSize * num, coordinateSize);
            Buffer.BlockCopy(CurTurning, 0, anchorByte, teleportSize * num + coordinateSize, turningSize);

            File.WriteAllBytes(Utilities.anchorPath, anchorByte);
        }
        public static byte checkOnlineStatus(bool chi = false)
        {
            byte[] b;
            if (chi)
                b = Utilities.ReadByteArray(s, Utilities.OnlineSessionAddress + Utilities.ChineseLanguageOffset, 0x1);
            else
                b = Utilities.ReadByteArray(s, Utilities.OnlineSessionAddress, 0x1);
            return b[0];
        }

        public static OverworldState GetOverworldState()
        {
            ulong address = GetCoordinateAddress(offset);
            uint value = BitConverter.ToUInt32(Utilities.peekAbsoluteAddress(s, (address + 0x1E).ToString("X"), 0x4), 0);

            return DecodeOverworldState(value);
        }

        public static OverworldState DecodeOverworldState(uint value)
        {
            Debug.Print("Overworld : 0x" + value.ToString("X"));

            if ($"{value:X8}".EndsWith("C906"))
                return OverworldState.Loading;
            else if ($"{value:X8}".EndsWith("33D0"))
                return OverworldState.Loading;
            else if ($"{value:X8}".EndsWith("AE07"))
                return OverworldState.Loading;
            else if ($"{value:X8}".EndsWith("EC17"))
                return OverworldState.Loading;
            else if ($"{value:X8}".EndsWith("AC00"))
                return OverworldState.Loading;
            else if ($"{value:X8}".EndsWith("BC10"))
                return OverworldState.Loading;
            switch (value)
            {
                case 0x00000000: return OverworldState.Null;
                case 0xC0066666: return OverworldState.OverworldOrInAirport;
                case 0xBE200000: return OverworldState.UserArriveLeavingOrTitleScreen;
                default: return OverworldState.Unknown;
            };
        }

        public static void dump()
        {
            ulong address = GetCoordinateAddress(offset);

            SaveFileDialog file = new SaveFileDialog()
            {
                Filter = "binbin (*.bin)|*.bin",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath.Replace(".exe", ".dll"));

            string savepath;

            if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + @"\save";
            else
                savepath = config.AppSettings.Settings["LastSave"].Value;

            if (Directory.Exists(savepath))
            {
                file.InitialDirectory = savepath;
            }
            else
            {
                file.InitialDirectory = @"C:\";
            }

            if (file.ShowDialog() != DialogResult.OK)
                return;

            byte[] b = Utilities.peekAbsoluteAddress(s, (address).ToString("X"), 8192);

            File.WriteAllBytes(file.FileName, b);
        }

        private static void FixAnchorByte()
        {
            byte[] temp = new byte[teleportSize * 5];

            int smallSize = anchorByte.Length / 5;

            for (int i = 0; i < 5; i++)
            {
                Buffer.BlockCopy(anchorByte, i * smallSize, temp, i * teleportSize, smallSize - turningSize);
                Buffer.BlockCopy(anchorByte, (i + 1) * smallSize - turningSize, temp, (i + 1) * teleportSize - turningSize, turningSize);
            }

            anchorByte = temp;

            File.WriteAllBytes(Utilities.anchorPath, anchorByte);

            Debug.Print(Utilities.ByteToHexString(anchorByte));
        }

        public static LocationState GetLocationState()
        {
            ulong address = GetCoordinateAddress(offset);
            uint value = BitConverter.ToUInt32(Utilities.peekAbsoluteAddress(s, (address + 0x6E).ToString("X"), 0x4), 0);
            Debug.Print("Location : " + value.ToString("X"));
            switch (value)
            {
                case 0x3EB44F1A: return LocationState.Announcement;
                case 0xB5CA1578:
                case 0x43140000:
                case 0x432A0CCD:
                case 0x430C0CCD:
                case 0x43200CCD: return LocationState.Indoor;
                case 0x0:
                case 0xFF: return LocationState.Loading;
                default: return LocationState.Unknown;
            };
        }
    }
}
