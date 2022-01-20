using System;
using System.Text;

namespace ACNHPokerCore
{
    public class Villager
    {
        private byte[] Data;

        public const int PlayerMemoryCount = 8;

        public int Index;
        public int HouseIndex { get; set; }
        public int MoveInFlag { get; set; }
        public int AbandonedHouseFlag { get; set; }
        public int ForceMoveOutFlag { get; set; }
        public int InvitedFlag { get; set; }
        public byte[] catchphrase { get; set; }

        public byte[] Friendship;

        public byte[][] TempData;

        public byte Species
        {
            get => Data[0];
            set => Data[0] = value;
        }

        public byte Variant
        {
            get => Data[1];
            set => Data[1] = value;
        }

        public byte Personality
        {
            get => Data[2];
            set => Data[2] = value;
        }

        public Villager(byte[] data, int i)
        {
            Data = data;
            Index = i;
            Friendship = new byte[8];
            TempData = new byte[8][];
            Friendship[0] = Data[70];
        }

        public string GetInternalName()
        {
            return Utilities.GetVillagerInternalName(Species, Variant);
        }
        public string GetRealName()
        {
            return Utilities.GetVillagerRealName(Species, Variant);
        }
        public string GetPersonality()
        {
            return ((Utilities.VillagerPersonality)Personality).ToString();
        }
        public bool IsReal()
        {
            if (GetInternalName() == "non00")
                return false;
            else
                return true;
        }
        public bool IsInvited()
        {
            if (Data[4] == 0x0 && Data[5] == 0x0 && Data[6] == 0x0)
                return true;
            else
                return false;
        }

        public VillagerMemory GetMemory(int index)
        {
            if (index >= PlayerMemoryCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            var bytes = Utilities.Slice(Data, (int)(0x4 + (index * Utilities.VillagerMemoryTinySize)), (int)Utilities.VillagerMemoryTinySize);
            return new VillagerMemory(bytes);
        }

        public void LoadData(byte[] data)
        {
            Data = data;
        }

        public byte[] GetHeader()
        {
            byte[] header = new byte[52];
            Buffer.BlockCopy(Data, 0x4, header, 0x0, 52);
            return header;
        }

        public string GetPlayerName(int player)
        {
            if (player <= 0)
                return Encoding.Unicode.GetString(Data, 36, 20);
            else
            {
                string name = Encoding.Unicode.GetString(TempData[player], 36, 20);
                if (name == "\0\0\0\0\0\0\0\0\0\0")
                    return "[ N/A ]";
                else
                    return name;
            }
        }
    }
}
