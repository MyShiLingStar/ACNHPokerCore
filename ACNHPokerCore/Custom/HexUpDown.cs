using System;
using System.Linq;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    class HexUpDown : NumericUpDown
    {
        public HexUpDown()
        {
            this.Hexadecimal = true;
        }

        protected override void ValidateEditText()
        {
            try
            {
                var txt = this.Text;
                if (!string.IsNullOrEmpty(txt))
                {
                    var value = Convert.ToDecimal(Convert.ToInt64(txt, 16));
                    value = Math.Max(value, this.Minimum);
                    value = Math.Min(value, this.Maximum);
                    this.Value = value;
                }
            }
            catch { }
            base.UserEdit = false;
            UpdateEditText();
        }

        [System.ComponentModel.DefaultValue(4)]
        public int HexLength
        {
            get { return length; }
            set { length = value; }
        }

        protected override void UpdateEditText()
        {
            long value = Convert.ToInt64(this.Value);
            string hexvalue = value.ToString("X");

            string n0 = String.Concat(Enumerable.Repeat("0", HexLength - hexvalue.Length));
            string result = String.Concat(n0, hexvalue);

            this.Text = result;
        }

        private int length = 8;
    }
}