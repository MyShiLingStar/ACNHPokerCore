using System.Drawing;

namespace Twitch
{
    public class ItemOrder
    {
        public string owner { get; set; }
        public string id { get; set; }
        public string count { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public Image image { get; set; }
    }
}
