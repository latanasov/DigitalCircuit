using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Drawing;

namespace DigitalCircuit
{
    [Serializable]
    class Circuit
    {
        private string name;
        private List<Gate> gateList;
        private List<Connection> ConnList;
        private List<Circuit> undoList = new List<Circuit>(10);
        private List<Circuit> redoList = new List<Circuit>(10);

        /// <summary>
        /// This is the constructor of the circuit, it gives the name of the circuit according to the parameter, 
        /// and creates the list of gates and connections
        /// </summary>
        /// <param name="givenName">
        /// The name of the current circuit
        /// </param>
        public Circuit(string givenName)
        {
            this.name = givenName;
            gateList = new List<Gate>();
            ConnList = new List<Connection>();
        }

        //Copy Constructor
        public Circuit(List<Gate> gate, List<Connection> conn)
        {
            gateList = new List<Gate>();
            ConnList = new List<Connection>();
            this.name = "foundo";
            for (int i = 0; i < gate.Count(); i++)
            {
                Gate G;
                if (gate[i] is SourceGate) { G = new SourceGate(gate[i]); }
                else if (gate[i] is SinkGate) { G = new SinkGate(gate[i]); }
                else if (gate[i] is AndGate) { G = new AndGate(gate[i]); }
                else if (gate[i] is OrGate) { G = new OrGate(gate[i]); }
                else { G = new NotGate(gate[i]); }
                gateList.Add(G);
            }
            for (int i = 0; i < conn.Count(); i++)
            {
                ConnList.Add(new Connection(conn[i], gateList));
            }

        }

        /// <summary>
        /// Saves the connection and gate list to a binary file.
        /// </summary>
        /// <param name="fileName">The file's name</param>
        public void SaveCircuit(string fileName)
        {
            FileStream fs = null;
            BinaryFormatter bf = null;

            try
            {
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                bf = new BinaryFormatter();
                bf.Serialize(fs, this.gateList);
                bf.Serialize(fs, this.ConnList);
                MessageBox.Show("Circuit Saved");

            }
            catch (SerializationException)
            { MessageBox.Show("something wrong with Serialization"); }
            catch (IOException)
            { MessageBox.Show("something wrong with IO"); }
            finally
            {
                if (fs != null) fs.Close();
            }

            

        }

        /// <summary>
        /// Reads the data from the binary file and loads the values
        /// into the connection and gate list.
        /// </summary>
        /// <param name="fileName">Name of the file + dir</param>
        public void LoadCircuit(string fileName)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();

                
                this.gateList = (List<Gate>)bf.Deserialize(fs);
                this.ConnList = (List<Connection>)bf.Deserialize(fs);
                MessageBox.Show("Circuit loaded");
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

        }
        /// <summary>
        /// This function checks, for whether the Gate gate overlaps another gate in the circuit by
        /// comparing the row and column of the gate with every gate in the circuit
        /// </summary>
        /// <param name="gate">
        /// The newly created gate to be placed in a cell in the circuit
        /// </param>
        /// <returns>
        /// It returns true when the gate doesn't overlap, or false if a gate exist in the same cell
        /// </returns>
        public bool addGate(Gate gate)
        {
            foreach (Gate ga in gateList)
            {
                if ((ga.getRow() == gate.getRow()) && (ga.getColumn() == gate.getColumn()))
                {
                    MessageBox.Show("This cell has been occupied, please select another cell to place the gate", "Overlapping gate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            setUndoList();
            this.redoList.Clear();
            gateList.Add(gate);
            return true;

        }
        /// <summary>
        /// This function deletes the selected gate along with all of its connection from the circuit
        /// It deletes all connection of that gate first before finally deleting the gate itself
        /// </summary>
        /// <param name="gate">The gate to be deleted</param>
        /// <returns>True if the gate is deleted</returns>
        public bool deleteGate(Gate gate)
        {
            setUndoList();
            this.redoList.Clear();
            List<Connection> temp = gate.getListOfConnections();
            int count = temp.Count();
            for(int i =0;i < count;i++)
            {
                if (temp[i] != null)
                {
                    if (temp[i].isFirstGate(gate))
                    {
                        Gate temp2 = temp[i].getSecondGate();
                        Connection Cx = temp[i];
                        do
                        {
                            Cx = temp2.inputConnectionInvalid(Cx);
                            if (Cx != null)
                                temp2 = Cx.getSecondGate();
                            else
                                temp2.calcValue();
                        }
                        while (Cx != null);
                       
                    }
                    deleteConnection(temp[i]);
                }
                
            }
            
            gateList.Remove(gate);
            return true;
        }
        /// <summary>
        /// This function checks:
        /// - 2 existed gate in 2 mouse click position
        /// - existed connection
        /// - connection rules
        /// and lastly adding the connection
        /// </summary>
        /// <param name="conn">
        /// conn will be the new created connection</param>
        /// <returns>
        /// the function will return true of a connection is created</returns>
        public bool addConnection(Connection conn)
        {
            setUndoList();
            this.redoList.Clear();
            foreach (Connection con in ConnList)
            {
                if ((conn.getFirstGate() == con.getFirstGate())&&(conn.getSecondGate() == con.getSecondGate()))
                {
                    MessageBox.Show("These gate have been connected", "Overlapping connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            if ((conn.getFirstGate() is SourceGate) && (conn.getSecondGate() is SourceGate))
            {
                MessageBox.Show("The connected gates are both source gate", "Invalid Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else if ((conn.getFirstGate() is SinkGate) && (conn.getSecondGate() is SinkGate))
            {
                MessageBox.Show("The connected gates are both sink gate", "Invalid Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                if(conn !=null)
                if (conn.getFirstGate().connectOutput(conn))
                {
                    if (conn.getSecondGate().connectInput(conn))
                    {
                       
                        ConnList.Add(conn);
                        return true;
                    }
                    else
                    {
                        conn.getFirstGate().removeConnect(conn);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Only one output per gate is allowed", "Invalid Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                
            }
            return false;
        }

        /// <summary>
        /// This function simply remove the connection from the connection list of the whole circuit,
        /// as well as remove it in the connection list of the first and second gate object.
        /// </summary>
        /// <param name="conn">
        /// conn will be removed</param>
        /// <returns></returns>
        public bool deleteConnection(Connection conn)
        {
            this.redoList.Clear();
            foreach (Connection con in ConnList)
            {
                if (con.Equals(conn))
                {
                    con.getFirstGate().removeConnect(conn);
                    con.getSecondGate().removeConnect(conn);
                }
            }
            ConnList.Remove(conn);
            return true;
        }
        /// <summary>
        /// This function gets a gate from the list which is located in the same row and column as specified in parameter
        /// </summary>
        /// <param name="row">selected row number</param>
        /// <param name="col">selected column number</param>
        /// <returns>returns the gate if there is a gate in the same position, otherwise null</returns>
        public Gate getGateFromList(int row, int col)
        {
            foreach (Gate G in gateList)
            {
                if (G.getColumn() == col && G.getRow() == row)
                {
                    return G;
                }
            }
            return null;
        }
        /// <summary>
        /// This function redraws the whole circuit in the picturebox pic
        /// </summary>
        /// <param name="pic">the pictureBox where the circuit is going to be drawn</param>
        public void reDraw(ref PictureBox pic)
        {
            foreach (Gate G in gateList)
            {
                G.drawGate(ref pic);
            }

            foreach (Connection C in ConnList)
            {
                C.drawConnection(ref pic);
            }
        }
        /// <summary>
        /// This function gets the connection from the list of connection of circuit which is closest to the location of the click
        /// Within a certain limit
        /// </summary>
        /// <param name="x">The coordinates of the click</param>
        /// <returns>The connection closest to the click, or if distance is too far returns null</returns>
        public Connection getConnectionFromList(Point x)
        {
            Connection temp = null;
            double loDis = 0;
            int count = 1;
            int x1=0,y1=0,x2=0,y2=0;
            foreach (Connection co in ConnList)
            {
                 x1 = (co.getFirstGate().getColumn()-1) * 50 +25;
                 y1 = (co.getFirstGate().getRow()-1)  * 50+25;
                 x2 = (co.getSecondGate().getColumn()-1)  * 50 + 25;
                 y2 = (co.getSecondGate().getRow()-1) * 50 + 25;
                double m = Convert.ToDouble(y2 - y1) / (x2 - x1);
                
                double b = (m * (-x1)) + y1;
                if (m == 0)
                {
                    b = y1;
                }
                double test = x.Y - m * x.X - b;
                if (test<0)
                {
                    test = -test;
                }
                double dist = (test) / Math.Sqrt((m*m)+1);
                if(loDis > dist)
                {
                    loDis = dist;
                    temp = co;
                }
                else if (count == 1)
                {
                    loDis = dist;
                    temp = co;
                    count++;
                }
            }

            if (loDis < 50)
            {   
                return temp;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets the list of connection
        /// </summary>
        /// <returns>List fo connections</returns>
        public List<Connection> getConList()
        {
            return this.ConnList;
        }

        public List<Gate> getGateList()
        {
            return this.gateList;
        }

        /**Sets current circuit into undo history list**/
        public void setUndoList()
        {
            if (undoList.Count == 10) { undoList.RemoveAt(0); }
            Circuit temp = new Circuit(this.gateList, this.ConnList);
            undoList.Add(temp);
 
        }

        /**Sets current circuit into redo history list**/
        private void setRedoList()
        {
            if (redoList.Count == 10) {redoList.RemoveAt(0); }
            Circuit temp = new Circuit(this.gateList, this.ConnList);
            redoList.Add(temp);

        }

        /**Undo function ,able to undo the last 10 actions**/

        public void undoCircuit()
        {

            if (undoList.Count <= 0) { }

            else
            {           
                setRedoList();
                this.gateList = undoList[undoList.Count() - 1].getGateList();
                this.ConnList = undoList[undoList.Count() - 1].getConList();
                this.undoList.RemoveAt(undoList.Count() - 1);
            }
                
        }

          /**Redo function ,able to redo the last 10 actions**/
        public void redoCircuit()
        {
            if (redoList.Count <= 0) { }
            
            else
            {
               
                setUndoList();  
                this.gateList = redoList[redoList.Count() - 1].getGateList();
                this.ConnList = redoList[redoList.Count() - 1].getConList();
                this.redoList.RemoveAt(redoList.Count() - 1);
            }


        }

        
     

    }
}
