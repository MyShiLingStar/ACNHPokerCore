using Microsoft.Extensions.Caching.Memory;
using System;
using System.Drawing;
using System.IO;

namespace ACNHPokerCore
{
    internal static class ImageCacher
    {
        private static IMemoryCache m_memoryCache;
        private static ImageObject m_imageObject;

        public static void setup(IMemoryCache memoryCache)
        {
            m_memoryCache = memoryCache;
            m_imageObject = null;
        }

        public static Image GetImage(string pickedImagePath)
        {
            if (pickedImagePath.Equals(string.Empty)) { return null; }
            if (File.Exists(pickedImagePath))
            {
                m_memoryCache.TryGetValue(pickedImagePath, out m_imageObject);

                if (m_imageObject == null)
                {
                    DateTime ourFileDate = File.GetLastWriteTime(pickedImagePath);
                    ourFileDate = ourFileDate.AddMilliseconds(-ourFileDate.Millisecond);

                    Image image = Image.FromFile(pickedImagePath);
                    m_imageObject = new ImageObject(pickedImagePath, image, ourFileDate);

                    var memCacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(120)).SetAbsoluteExpiration(TimeSpan.FromSeconds(300));

                    m_memoryCache.Set(pickedImagePath, m_imageObject, memCacheEntryOptions);

                    //Debug.Print("Drive");
                }
                else
                {
                    //Debug.Print("Cache");
                }

                return m_imageObject.itemImage;
            }
            else
            {
                return null;
                //throw new FileNotFoundException();
            }
        }
    }
}
