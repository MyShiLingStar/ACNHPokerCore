using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public class DataGridViewProgressColumn : DataGridViewImageColumn
    {
        public DataGridViewProgressColumn()
        {
            CellTemplate = new DataGridViewProgressCell();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public sealed override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set { base.CellTemplate = value; }
        }
    }
    class DataGridViewProgressCell : DataGridViewImageCell
    {
        // Used to make custom cell consistent with a DataGridViewImageCell
        static readonly Image emptyImage;
        public int setValue { get; set; } = 0;
        public string remark { get; set; } = "";

        public int mode = 0;
        static DataGridViewProgressCell()
        {
            emptyImage = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }
        public DataGridViewProgressCell()
        {
            ValueType = typeof(int);
        }

        public sealed override Type ValueType
        {
            get { return base.ValueType; }
            set { base.ValueType = value; }
        }

        // Method required to make the Progress Cell consistent with the default Image Cell.
        // The default Image Cell assumes an Image as a value, although the value of the Progress Cell is an int.
        protected override object GetFormattedValue(object value,
                            int rowIndex, ref DataGridViewCellStyle cellStyle,
                            TypeConverter valueTypeConverter,
                            TypeConverter formattedValueTypeConverter,
                            DataGridViewDataErrorContexts context)
        {
            return emptyImage;
        }

        protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            float percentage = (setValue / 255.0f);
            Brush foreColorBrush = new SolidBrush(cellStyle.ForeColor);
            Brush BlackBrush = new SolidBrush(Color.Black);
            // Draws the cell grid
            base.Paint(g, clipBounds, cellBounds,
             rowIndex, cellState, value, formattedValue, errorText,
             cellStyle, advancedBorderStyle, (paintParts & ~DataGridViewPaintParts.ContentForeground));
            if (percentage > 0.0)
            {
                int size = Convert.ToInt32((percentage * cellBounds.Width - 4));
                if (size <= 0)
                {
                    size = 1;
                }

                if (mode == 0)
                {
                    if (setValue < 85)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, setValue * 3, 0, 255 - setValue * 3)), cellBounds.X + 2, cellBounds.Y + 2, size * 2, cellBounds.Height - 4);
                        g.DrawString(remark, cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 10);
                    }
                    else if (setValue == 255)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, setValue, setValue, 0)), cellBounds.X + 2, cellBounds.Y + 2, size, cellBounds.Height - 4);
                        g.DrawString(remark, cellStyle.Font, BlackBrush, cellBounds.X + 6, cellBounds.Y + 10);
                    }
                    else
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, setValue, 0)), cellBounds.X + 2, cellBounds.Y + 2, size, cellBounds.Height - 4);
                        g.DrawString(remark, cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 10);
                    }
                }
                else
                {
                    if (setValue < 25)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, setValue * 3, 0, 255 - setValue * 3)), cellBounds.X + 2, cellBounds.Y + 2, size * 10, cellBounds.Height - 4);
                        g.DrawString(remark, cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 10);
                    }
                    else if (setValue == 255)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, setValue, setValue, 0)), cellBounds.X + 2, cellBounds.Y + 2, size, cellBounds.Height - 4);
                        g.DrawString(remark, cellStyle.Font, BlackBrush, cellBounds.X + 6, cellBounds.Y + 10);
                    }
                    else
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, setValue, 0)), cellBounds.X + 2, cellBounds.Y + 2, size, cellBounds.Height - 4);
                        g.DrawString(remark, cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 10);
                    }
                }
                // Draw the progress bar and the text
                //g.DrawString(setValue.ToString(), cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 18);
            }
            else
            {
                g.DrawString(remark, cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 10);
                // draw the text
                /*
                if (this.DataGridView.CurrentRow.Index == rowIndex)
                    g.DrawString(setValue.ToString(), cellStyle.Font, new SolidBrush(cellStyle.SelectionForeColor), cellBounds.X + 6, cellBounds.Y + 2);
                else
                    g.DrawString(setValue.ToString(), cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 2);
                    */
            }
        }
    }

}
