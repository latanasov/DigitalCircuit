using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DigitalCircuit
{
    [Serializable]
    class Connection
    {
        private Gate first;
        private Gate second;

        /// <summary>
        /// This is the constructor of the connection object,
        /// which having two parameters: first and second gate object.
        /// </summary>
        /// <param name="firs"></param>
        /// <param name="secon"></param>
        public Connection(Gate firs, Gate secon)
        {
            first = firs;
            second = secon;
        }

        //Copy Constructor
        /// <summary>
        /// Copy Constructor of Connection Class
        /// </summary>
        /// <param name="C">The Original Connection to be copied</param>
        /// <param name="Ga">The List of Gate of the copy circuit</param>
        public Connection(Connection C, List<Gate> Ga)
        {
            foreach (Gate Gat in Ga)
            {
                //Checks which gate is which, and assign it to the correct attribute of connection
                if (C.getFirstGate().getRow() == Gat.getRow() && C.getFirstGate().getColumn() == Gat.getColumn())
                {
                    first = Gat;
                    first.connectOutput(this);
                }

                else if (C.getSecondGate().getRow() == Gat.getRow() && C.getSecondGate().getColumn() == Gat.getColumn())
                {
                    second = Gat;
                    second.connectInput(this);
                }
            }
        }

        /// <summary>
        /// This function return the value of the connection, which is depended on the first gate
        /// </summary>
        /// <returns></returns>
        public int getValue()
        {
            return first.calcValue();
        }
        
        /// <summary>
        /// this function draw the connection and determine its color (value)
        /// </summary>
        /// <param name="pic">The pictureBox where the connection will be drawn</param>
        public void drawConnection(ref PictureBox pic)
        {
            int col1 = first.getColumn(); 
            int row1 = first.getRow();
            int col2 = second.getColumn();
            int row2 = second.getRow();
            Pen P;
            if (getValue() == 0)
            {
                P = new Pen(Color.Blue, 3);
            }
            else if (getValue() == 1)
            {
                P = new Pen(Color.Green, 3);
            }
            else
            {
                P = new Pen(Color.Red, 3);
            }
            Graphics g = pic.CreateGraphics();
            Point p1 = new Point((col1 - 1) * 50 + 50, (row1 - 1) * 50 + 25);
            Point p2 = new Point(0,0);

            //the following code define where the line end
            if (second is NotGate || second is SinkGate)
            {
                p2 = new Point((col2 - 1) * 50, (row2 - 1) * 50 + 25);
            }
            else
            {
                List<Connection> temp = second.getListOfConnections();
                if (temp.Count() == 1 || (temp.Count() > 1 && temp[0] != null && temp[1] == null) || (temp.Count() > 1 && temp[0] != null && temp[1] != null && this.Equals(temp[0]))) 
                {
                    p2 = new Point((col2 - 1) * 50, (row2 - 1) * 50);
                }
                else if ((temp.Count() > 1 && temp[0] != null && temp[1] != null )||(temp.Count() > 1 && temp[1].Equals(this)))
                {
                    p2 = new Point((col2 - 1) * 50, (row2 - 1) * 50 + 50);
                }
            }
            g.DrawLine(P, p1, p2);
            
        }

        /// <summary>
        /// this function is checking whether the parameter gate G is the first gate of the connection
        /// </summary>
        /// <param name="G">The Gate to be checked</param>
        /// <returns>true if it's first gate, false otherwise</returns>
        public bool isFirstGate(Gate G)
        {
            if(first.Equals(G))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// This function return the second gate of the connection
        /// </summary>
        /// <returns>The second gate of the connection</returns>
        public Gate getSecondGate()
        {
            return second;
        }

        /// <summary>
        /// this function return the first gate of the connection
        /// </summary>
        /// <returns>The first gate of the connection</returns>
        public Gate getFirstGate()
        {
            return first;
        }
    }
}
