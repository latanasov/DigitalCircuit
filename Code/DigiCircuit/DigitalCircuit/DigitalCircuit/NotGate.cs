using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DigitalCircuit
{
    [Serializable]
    class NotGate : Gate
    {
        private int Input1;

        //Constructor
        /// <summary>
        /// //Constructor of the NotGate Class
        /// </summary>
        /// <param name="loca">Coordinates of the selected grid cell</param>
        public NotGate(Point loca)
            : base(loca)
        {
            Input1 = -1;
        }

        //Copy Constructor
        /// <summary>
        /// Copy Constructor of NotGate Class
        /// </summary>
        /// <param name="G">The Original Gate to be copied</param>
        public NotGate(Gate G)
            : base(G)
        {
            Input1 = ((NotGate)G).Input1;

        }
        /// <summary>
        /// This function checks first if there is sufficient input(1 input) for the gate,
        /// and if not null, assigns the value of the connection to them then calculate the value of the gate
        /// </summary>
        /// <returns>The calculated value of the notGate, which is the opposite value of the input</returns>
        public override int calcValue()
        {
            if (Connected.Count >= 1)
            {
                if (Connected[0] != null)
                {
                    Input1 = Connected[0].getValue();
                }
                else
                {
                    Input1 = -1;
                }
               
                switch (Input1)
                {
                    case 1: Value =  0;
                        break;
                    case 0: Value = 1;
                        break;
                    default: Value =  -1;
                        break;
                }
            }
            else
            {
                Value = -1;
            }
            return Value;
            
        }

        /// <summary>
        /// This functions draw the NotGate in the picturebox pic in the coordinates of the object
        /// </summary>
        /// <param name="pic">the picturebox in which the gate is going to be drawn</param>
        public override void drawGate(ref PictureBox pic)
        {
            Graphics g = pic.CreateGraphics();
            Point Coord = new Point((col - 1) * 50 + 1, (row - 1) * 50 + 1);
            g.DrawImageUnscaled(Image.FromFile("../../../../../NotGate.jpg"), Coord);
        }

        /// <summary>
        /// This functions resets the value of the input of a gate to -1
        /// </summary>
        /// <param name="id">To determine which Input to be reseted</param>
        public override void resetInputValue(int id)
        {
            Input1 = -1;
        }
    }
}
