using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ACNHPokerCore
{
    public class DesignPattern
    {
        public const int Width = 32;
        public const int Height = 32;

        public const int SIZE = 0x2A8;
        private const int PersonalOffset = 0x38;
        private const int PaletteDataStart = 0x78;
        public const int PaletteColorCount = 15;
        private const int PaletteColorSize = 3;
        private const int PixelDataOffset = PaletteDataStart + (PaletteColorCount * PaletteColorSize);

        public byte[] Data;

        public uint Hash
        {
            get => BitConverter.ToUInt32(Data, 0x00);
        }

        public uint Version
        {
            get => BitConverter.ToUInt32(Data, 0x04);
        }

        public string DesignName
        {
            get => Utilities.GetString(Data, 0x10, 20);
        }

        public uint TownID
        {
            get => BitConverter.ToUInt32(Data, PersonalOffset);
        }

        public string TownName
        {
            get => Utilities.GetString(Data, PersonalOffset + 0x04, 10);
        }

        public uint PlayerID
        {
            get => BitConverter.ToUInt32(Data, PersonalOffset + 0x1C);
        }

        public string PlayerName
        {
            get => Utilities.GetString(Data, PersonalOffset + 0x20, 10);
        }

        public DesignPattern(byte[] data)
        {
            Data = data;
        }

        public static int GetColorOffset(int index)
        {
            return PaletteDataStart + (index * PaletteColorSize);
        }
        public Bitmap GetBitmap()
        {
            byte[] data = new byte[4 * Width * Height];

            for (int i = 0; i < Width * Height; i++)
            {
                int PaletteValue;

                if ((i & 1) == 0)
                    PaletteValue = Data[PixelDataOffset + (i / 2)] & 0x0F;
                else
                    PaletteValue = Data[PixelDataOffset + (i / 2)] >> 4;

                if (PaletteValue == PaletteColorCount)
                    continue;
                var palette = PaletteDataStart + (PaletteValue * PaletteColorSize);
                var ofs = i * 4;
                data[ofs + 2] = Data[palette + 0];
                data[ofs + 1] = Data[palette + 1];
                data[ofs + 0] = Data[palette + 2];
                data[ofs + 3] = 0xFF;
            }

            var bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            var bmpData = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            var ptr = bmpData.Scan0;
            Marshal.Copy(data, 0, ptr, data.Length);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public Bitmap GetBitmap(int size)
        {
            Bitmap bmp = GetBitmap();
            var destRect = new Rectangle(0, 0, size, size);
            var destImage = new Bitmap(size, size);
            destImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(bmp, destRect, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, wrapMode);
            }
            return destImage;
        }
    }
}
