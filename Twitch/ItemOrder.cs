using System.Drawing;

namespace Twitch
{
    public class ItemOrder
    {
        public string Owner { get; set; }
        public string Id { get; set; }
        public string Count { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public Image Image { get; set; }
    }
}
