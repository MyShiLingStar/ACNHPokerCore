using System;
using System.Drawing;

namespace ACNHPokerCore
{
    internal class ImageObject
    {
        public string FileName { get; set; }
        public Image itemImage { get; set; }
        public DateTime SubmitDate { get; set; }

        public ImageObject(string fn,Image image, DateTime dt)
        {
            FileName = fn;
            itemImage = image;
            SubmitDate = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, DateTimeKind.Utc);
        }
    }
}
