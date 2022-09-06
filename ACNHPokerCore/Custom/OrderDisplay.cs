﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public partial class OrderDisplay : Form
    {
        readonly LinkedList<Image> imageLinkedList = new();
        public OrderDisplay()
        {
            InitializeComponent();
        }

        public void SetItemdisplay(Image image)
        {
            imageLinkedList.AddFirst(image);
            DisplayImage();
        }

        private void DisplayImage()
        {
            if (imageLinkedList.Count > 3)
                imageLinkedList.RemoveLast();

            if (imageLinkedList.Count >= 1)
                itemImage1.Image = imageLinkedList.ElementAt(0);
            if (imageLinkedList.Count >= 2)
                itemImage2.Image = new Bitmap(imageLinkedList.ElementAt(1), new Size(itemImage2.Width, itemImage2.Height));
            if (imageLinkedList.Count >= 3)
                itemImage3.Image = new Bitmap(imageLinkedList.ElementAt(2), new Size(itemImage3.Width, itemImage3.Height));
        }
    }
}
