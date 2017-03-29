using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DigitalCircuit
{
    [Serializable]
    class OrGate : Gate
    {
        private int Input1;
        private int Input2;

        //Constructor
        /// <summary>
        /// //Constructor of the OrGate Class
        /// </summary>
        /// <param name="loca">Coordinates of the selected grid cell</param>
        public OrGate(Point loca)
            : base(loca)
        {
            Input1 = Input2 = -1;
        }


        //Copy Constructor
        /// <summary>
        /// Copy Constructor of OrGate Class
        /// </summary>
        /// <param name="G">The Original Gate to be copied</param>
        public OrGate(Gate G)
            : base(G)
        {
            Input1 = ((OrGate)G).Input1;
            Input2 = ((OrGate)G).Input2;
        }
        /// <summary>
        /// This function checks first if there is sufficient inputs(2 inputs) for the gate,
        /// and if not null, assigns the value of the connection to them then calculate the value of the gate
        /// </summary>
        /// <returns>The calculated value of the orGate, result of ORing the 2 values of the inputs</returns>
        public override int calcValue()
        {
            if (Connected.Count >= 2)
            {
                if (Connected[0] != null)
                {
                    Input1 = Connected[0].getValue();
                }
                else
                {
                    Input1 = -1;
                }

                if (Connected[1] != null)
                {
                    Input2 = Connected[1].getValue();
                }
                else
                {
                    Input2 = -1;
                }

                if (Input1 != -1 && Input2 != -1)
                {
                    if (Input1 == 1 || Input2 == 1)
                    {
                        Value = 1;
                    }
                    else
                    {
                        Value = 0;
                    }
                }
                else
                {
                    Value = -1;
                }
            }
            else
            {
                Value = -1;
            }
            return Value;
        }

        /// <summary>
        /// This functions draw the OrGate in the picturebox pic in the coordinates of the object
        /// </summary>
        /// <param name="pic">the picturebox in which the gate is going to be drawn</param>
        public override void drawGate(ref PictureBox pic)
        {
            Graphics g = pic.CreateGraphics();
            Point Coord = new Point((col - 1) * 50 + 1, (row - 1) * 50 + 1);
            g.DrawImageUnscaled(Image.FromFile("../../../../../OrGate.jpg"), Coord);
        }

        /// <summary>
        /// This functions resets the value of the input of a gate to -1
        /// </summary>
        /// <param name="id">To determine which Input to be reseted</param>
        public override void resetInputValue(int id)
        {
            if (id == 0)
            {
                Input1 = -1;
            }
            else
            {
                Input2 = -1;
            }
        }
    }
}
