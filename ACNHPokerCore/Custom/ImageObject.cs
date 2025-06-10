using System;
using System.Drawing;

namespace ACNHPokerCore
{
    internal class ImageObject(string fn, Image image, DateTime dt)
    {
        public string FileName { get; set; } = fn;
        public Image ItemImage { get; set; } = image;
        public DateTime SubmitDate { get; set; } = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, DateTimeKind.Utc);
    }
}
