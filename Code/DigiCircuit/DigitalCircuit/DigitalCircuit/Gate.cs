using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DigitalCircuit
{
    [Serializable]
    abstract class Gate
    {

        protected int Value;
        protected int row = 0;
        protected int col = 0;
        protected List<Connection> Connected;

        /// <summary>
        /// The costructor of the gate object, it translates the coordinates of the gate into row and column number 
        /// and stores it in the corresponding variable
        /// </summary>
        /// <param name="loca">
        /// Coordinates of the gate
        /// </param>
        public Gate(Point loca)
        {
            row = loca.Y / 50 + 1;
            col = loca.X / 50 + 1;
            Connected = new List<Connection>();
            Value = -1;
            
        }


        //Copy Constructor
        /// <summary>
        /// Copy Constructor of the Gate Class
        /// </summary>
        /// <param name="g">The gate object to be copied</param>      
        public Gate(Gate g)
        {
            Connected = new List<Connection>();
            this.row = g.getRow();
            this.col = g.getColumn();
            this.Value = g.calcValue();

        }
        /// <summary>
        /// Draws the gate in the picturebox pic
        /// </summary>
        /// <param name="pic">The picturebox in which the gate will be drawn on</param>
        public virtual void drawGate(ref PictureBox pic)
        {

        }

        /// <summary>
        /// Calculates the value of the gate and stores it in the value, then return the value
        /// </summary>
        /// <returns>The value of the gate</returns>
        public virtual int calcValue()
        {
            return Value;
        }

        /// <summary>
        /// This function connects the connection C as an input to the calling gate, which slot selected depends on which one is empty
        /// </summary>
        /// <param name="C">The connection to be connected</param>
        /// <returns>true if connection is succesful, false otherwhise</returns>
        public bool connectInput(Connection C)
        {
            if (Connected.Count() == 0)
            {
                Connected.Add(C);
                return true;
            }
            else if (Connected.Count() == 1)
            {
                if (Connected[0] == null)
                {
                    Connected[0] = C;
                    return true;
                }
                else
                {
                    if (Connected[0].getFirstGate().Equals(this))
                    {
                        Connected.Insert(0, C);
                        return true;
                    }
                    else
                    {
                        if (!(this is NotGate || this is SinkGate))
                        {
                            Connected.Add(C);
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Too Much Inputs!, not allowed", "Too many inputs", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (Connected[0] == null)
                {
                    Connected[0] = C;
                    return true;
                }
                else if (Connected[1] == null)
                {
                    Connected[1] = C;
                    return true;
                }
                else
                {
                    if (Connected[1].getFirstGate().Equals(this))
                    {
                        Connected.Insert(1, C);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Too Much Inputs!, not allowed", "Too many inputs", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
        }
        /// <summary>
        /// This function connects the connection C as an output to the calling gate
        /// </summary>
        /// <param name="C">The connection to be connected</param>
        /// <returns>true if connection is succesful, false otherwhise</returns>
        public bool connectOutput(Connection C)
        {           
                if (Connected.Count() == 0)
                {  
                    if (this is AndGate || this is OrGate)
                    {
                        Connected.Add(null);
                        Connected.Add(null);
                        Connected.Add(C);
                        return true;
                    }
                    else if (this is NotGate)
                    {
                        Connected.Add(null);
                        Connected.Add(C);
                        return true;
                    }
                    else if (this is SourceGate)
                    {
                        Connected.Add(C);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                   
                }
                else if (Connected.Count() == 1)
                {
                    if (Connected[0] == null)
                    {
                        Connected[0] = C;
                        return true;
                    }
                    else
                    {
                        if (Connected[0].getFirstGate().Equals(this))
                        {
                            return false;
                        }
                        else
                        {
                            if (this is AndGate || this is OrGate)
                            {
                                Connected.Add(null);
                                Connected.Add(C);
                                return true;
                            }
                            else if (this is NotGate)
                            {
                                Connected.Add(C);
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                }
                else if (Connected.Count() == 2)
                {
                   
                        if (Connected[1] == null)
                        {
                            Connected[1] = C;
                            return true;
                        }
                        else
                        {
                            if (Connected[1].getFirstGate().Equals(this))
                            {
                                return false;
                            }
                            else
                            {
                                Connected.Add(C);
                                return true;
                            }
                        }


                }
                else if (Connected.Count() == 3)
                {

                    if (Connected[2] == null)
                    {
                        Connected[2] = C;
                        return true;
                    }
                    else
                    {
                        if (Connected[2].getFirstGate().Equals(this))
                        {
                            return false;
                        }
                        else
                        {
                            Connected.Add(C);
                            return true;
                        }
                    }

                }
                else
                {
                    return false;
                }
               
                                  
        }

        /// <summary>
        /// This function removes the connection C from the calling gate
        /// </summary>
        /// <param name="C">Connection to be removed</param>
        public void removeConnect(Connection C)
        {
            int index = -1;
            foreach (Connection co in Connected)
            {
                if (co != null)
                {
                    if (co.Equals(C))
                        index = Connected.IndexOf(co);
                }
            }
           
            if (index != -1)
            {
                Connected[index]= null;
            }
        }
        /// <summary>
        /// This function returns the row number of the calling gate
        /// </summary>
        /// <returns>int row</returns>
        public int getRow()
        {
            return row;
        }
        /// <summary>
        /// This function returns the column number of the calling gate
        /// </summary>
        /// <returns>int column</returns>
        public int getColumn()
        {
            return col;
        }
        /// <summary>
        /// This function returns the list of connections connected to the calling gate
        /// </summary>
        /// <returns>
        /// List Connection Connected
        /// </returns>
        public List<Connection> getListOfConnections()
        {
            return Connected;
        }

        /// <summary>
        /// This function will change the value of one of the input of the calling gate depends on the Connection which is invalid
        /// </summary>
        /// <param name="C">The connection which is not valid anymore, a result of a not completed gate value calculation</param>
        /// <returns>The connection after the invalid connection which will be modified</returns>
        public Connection inputConnectionInvalid(Connection C)
        {
            int count = 0;
            List<Connection> temp = this.getListOfConnections();
            Connection temp2 = null;
            foreach (Connection co in temp)
            {
                if (co.Equals(C))
                {
                    this.resetInputValue(count);
                }
               
                count++;

                if (co.isFirstGate(this))
                {
                    temp2 = co;
                }
            }
            return temp2;
        }
        /// <summary>
        /// This functions resets the value of the input of a gate to -1
        /// </summary>
        /// <param name="id">To determine which Input to be reseted</param>
        public virtual void resetInputValue(int id)
        {
           
        }
    }
}
