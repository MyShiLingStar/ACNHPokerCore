using System;

namespace ACNHPokerCore
{
    public class VillagerMemory
    {
        private byte[] Data;

        public VillagerMemory(byte[] data) => Data = data;
        public uint TownID
        {
            get => BitConverter.ToUInt32(Data, 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x00);
        }

        public string TownName
        {
            get => Utilities.GetString(Data, 0x04, 10);
            set => Utilities.GetBytes(value, 10).CopyTo(Data, 0x04);
        }
        public byte[] GetTownIdentity() => Utilities.Slice(Data, 0x00, 4 + 20);

        public uint PlayerID
        {
            get => BitConverter.ToUInt32(Data, 0x1C);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x1C);
        }

        public string PlayerName
        {
            get => Utilities.GetString(Data, 0x20, 10);
            set => Utilities.GetBytes(value, 10).CopyTo(Data, 0x20);
        }
        public byte[] GetPlayerIdentity() => Utilities.Slice(Data, 0x1C, 4 + 20);
        public byte[] GetEventFlags() => Utilities.Slice(Data, 0x38, 0x100);
        public void SetEventFlags(byte[] value) => value.CopyTo(Data, 0x38);
        public byte Friendship { get => Data[0x38 + 10]; set => Data[0x38 + 10] = value; }
    }
}
