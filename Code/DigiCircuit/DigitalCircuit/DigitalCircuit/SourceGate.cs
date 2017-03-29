using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DigitalCircuit
{
    [Serializable]
    class SourceGate : Gate
    {

        //Constructor
        /// <summary>
        /// //Constructor of the SourceGate Class
        /// </summary>
        /// <param name="loca">Coordinates of the selected grid cell</param>
        public SourceGate(Point loca)
            : base(loca)
        {
            Value = 0;
        }

        //Copy Constructor
        /// <summary>
        /// Copy Constructor of SourceGate Class
        /// </summary>
        /// <param name="G">The Original Gate to be copied</param>
        public SourceGate(Gate G)
            : base(G)
        {
            Value = ((SourceGate)G).Value;
        }
        /// <summary>
        /// This function changes the value of the sourceGate, if it is 1 it becomes 0 and vice-versa
        /// then it redraws the gate immediately
        /// </summary>
        /// <param name="pic">the picturebox in which the it will be drawn</param>
        /// <returns></returns>
        public int changeValue(ref PictureBox pic)
        {
            if (Value == 0)
            {
                Value = 1;
                drawGate(ref pic);
                return 1;
            }
            else
            {
                Value = 0;
                drawGate(ref pic);
                return 0;
            }
        }
        /// <summary>
        /// This functions draw the sourceGate in the picturebox pic in the coordinates of the object
        /// which image appears depends on the value of the source gate
        /// </summary>
        /// <param name="pic">the picturebox in which the gate is going to be drawn</param>
        public override void drawGate(ref PictureBox pic)
        {
            Graphics g = pic.CreateGraphics();
            Point Coord = new Point((col - 1) * 50 + 1, (row - 1) * 50 + 1);
            if (Value == 1)
            {
                g.DrawImageUnscaled(Image.FromFile("../../../../../sourceOne.jpg"), Coord);
            }
            else
            {
                g.DrawImageUnscaled(Image.FromFile("../../../../../sourceZero.jpg"), Coord);
            }

        }
    }
}
