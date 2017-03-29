using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DigitalCircuit
{
    public partial class DigiCircuit : Form
    {
        public DigiCircuit()
        {
            InitializeComponent();
        }

        Graphics grid;
        Gate chosen = null, first = null, second = null;
        Circuit circ = new Circuit("Digital");
        Pen gridLine = new Pen(Color.Black);
        Pen highlightLine = new Pen(Color.Red, 2);
        int widthOfOneCell = 50, heightOfOneCell = 50;
        int rowNum = 0, colNum = 0;
        private Rectangle highlight = Rectangle.Empty;
        Button dynamicButton = null;
        Connection chosenConn = null;

        // these stuff serve for the connection.
        int counter = 0;

        enum drawCode
        {
            Source, Sink, And, Or, Not, Connect
        };

        drawCode? code = null;

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void CreateDynamicButton(Point P)
        {
            // Create a Button object 
            dynamicButton = new Button();

            // Set Button properties
            dynamicButton.Height = 30;
            dynamicButton.Width = 100;
            dynamicButton.BackColor = Color.Red;
            dynamicButton.ForeColor = Color.Blue;
            dynamicButton.Location = P;
            dynamicButton.Text = "Change";
            dynamicButton.Name = "DynamicButton";
            dynamicButton.Font = new Font("Georgia", 16);

            // Add a Button Click Event handler
            dynamicButton.Click += new EventHandler(DynamicButton_Click);

            // Add Button to the Form. Placement of the Button
            // will be based on the Location and Size of button
            gridBox.Controls.Add(dynamicButton);
        }

        /// <summary>
        /// Button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DynamicButton_Click(object sender, EventArgs e)
        {
            circ.setUndoList();
            ((SourceGate)chosen).changeValue(ref gridBox);
            circ.reDraw(ref gridBox);

        }

       
        /// <summary>
        /// Load a circuit when clicking on the folder icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                circ.LoadCircuit(openFileDialog1.FileName);
            }
            circ.reDraw(ref gridBox);
        }
        /// <summary>
        /// Makes a new form for the help file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DigiCircuit_Load(object sender, EventArgs e)
        {
            subForm myNewForm = new subForm();
        }

        
        /// <summary>
        /// Paint the gridBox with squares and/or highlighter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridBox_Paint(object sender, PaintEventArgs e)
        {
            grid = e.Graphics;
            int currentX = 0, currentY = 0;

            for (int i = 0; i < 20; i++)
            {
                for (int ai = 0; ai < 20; ai++)
                {
                    grid.DrawRectangle(gridLine, currentX, currentY, widthOfOneCell, heightOfOneCell);
                    //gr.FillRectangle(myBrush, currentX+1, currentY+1, widthOfOneCell-1, heightOfOneCell-1);
                    currentX += widthOfOneCell;
                }
                currentX = 0;
                currentY += heightOfOneCell;
            }

            if (highlight != Rectangle.Empty)
            {
                grid.DrawRectangle(highlightLine, highlight);
            }

        }

        private void gridBox_MouseClick(object sender, MouseEventArgs e)
        {
            //Hides the dynamic button when clicking on an empty cell.
            if (dynamicButton != null)
            {
                dynamicButton.Visible = false;
            }
            //First, it checks which button is clicked
            //Then depends on the button clicked, in creates and draw the corresponding objects gate or connection
            if (code == drawCode.Source)
            {
                Gate SG1 = new SourceGate(e.Location);
                if (circ.addGate(SG1) == true)
                {
                    SG1.drawGate(ref gridBox);
                    highlight = Rectangle.Empty;
                }
                code = null;
            }
            else if (code == drawCode.Sink)
            {
                Gate SG1 = new SinkGate(e.Location);
                if (circ.addGate(SG1) == true)
                {
                    SG1.drawGate(ref gridBox);
                    highlight = Rectangle.Empty;
                }
                code = null;
            }
            else if (code == drawCode.And)
            {
                Gate AG1 = new AndGate(e.Location);
                if (circ.addGate(AG1) == true)
                {
                    AG1.drawGate(ref gridBox);
                    highlight = Rectangle.Empty;
                }
                code = null;
            }
            else if (code == drawCode.Or)
            {
                Gate OG1 = new OrGate(e.Location);
                if (circ.addGate(OG1) == true)
                {
                    OG1.drawGate(ref gridBox);
                    highlight = Rectangle.Empty;
                }
                code = null;
            }
            else if (code == drawCode.Not)
            {
                Gate NG1 = new NotGate(e.Location);
                if (circ.addGate(NG1) == true)
                {
                    NG1.drawGate(ref gridBox);
                    highlight = Rectangle.Empty;
                }
                code = null;
            }
            else if (code == drawCode.Connect)
            {
                rowNum = e.Location.Y / 50 + 1;
                colNum = e.Location.X / 50 + 1;
                chosen = circ.getGateFromList(rowNum, colNum);

                if (chosen != null)
                {
                    if (counter == 0)
                    {
                        // if the first gate is sink, gives error message
                        if (chosen is SinkGate)
                        {
                            MessageBox.Show("The sink gate cannot be the first gate", "Invalid Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            first = second = null;
                            code = null;
                        }
                        else
                        {
                            first = chosen;
                            counter = 1;
                        }
                    }
                    else
                    {
                        // if the second gate is source, gives error message
                        if (chosen is SourceGate)
                        {
                            MessageBox.Show("Source gate cannot be the second gate", "Invalid Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            first = second = null;
                            code = null;
                            counter = 0;
                        }
                        // if the both selected gate are the same, gives error message
                        else if (first == chosen)
                        {
                            MessageBox.Show("You are trying to connect the same gate", "Invalid Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            first = second = null;
                            code = null;
                            counter = 0;
                        }
                        else
                        {

                            second = chosen;
                            counter = 0;
                            Connection c = new Connection(first, second);
                            if (circ.addConnection(c))
                            {
                                c.drawConnection(ref gridBox);
                                first = second = null;
                                circ.reDraw(ref gridBox);
                            }
                        }
                    }
                }
                else
                {
                    code = null;
                    first = second = null;
                }
            }
            else if (code == null)
            {
                rowNum = e.Location.Y / 50 + 1;
                colNum = e.Location.X / 50 + 1;
                chosen = circ.getGateFromList(rowNum, colNum);
                if (chosen != null)
                {
                    chosenConn = null;
                    Point x = new Point(((colNum - 1) * 50 - 25), ((rowNum - 1) * 50) + 50);
                    highlight = new Rectangle(((colNum - 1) * 50) - 5, ((rowNum - 1) * 50) - 5, 60, 60);
                    gridBox.Refresh();
                    circ.reDraw(ref gridBox);

                    if (chosen is SourceGate)
                    {
                        CreateDynamicButton(x);
                    }

                }
                else
                {
                    chosenConn = circ.getConnectionFromList(e.Location);
                    chosen = null;
                    if (chosenConn != null)
                    {
                        int x1 = (chosenConn.getFirstGate().getColumn()) * 50;
                        int x2 = (chosenConn.getSecondGate().getColumn() - 1) * 50;
                        int y1 = (chosenConn.getFirstGate().getRow() - 1) * 50;
                        int y2 = (chosenConn.getSecondGate().getRow()-1) * 50;
                        if (y2 > y1)
                            highlight = new Rectangle(x1-5, y1+15, (x2 - x1)+10, (y2 - y1)+5);
                        else if(y2<y1)
                            highlight = new Rectangle(x1-5, y2+15, (x2 - x1)+10, (y1 - y2)+10);
                        else
                            highlight = new Rectangle(x1-5, y1, (x2 - x1)+10, 50);
                    }
                    else
                    {
                        highlight = new Rectangle(((colNum - 1) * 50) - 5, ((rowNum - 1) * 50) - 5, 60, 60);
                        gridBox.Refresh();
                        circ.reDraw(ref gridBox);
                    }

                }
            }
            gridBox.Refresh();
            circ.reDraw(ref gridBox);

        }


        /// <summary>
        /// Shows the Help in a subform.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subForm myNewForm = new subForm();

            myNewForm.Show();
        }

        /// <summary>
        /// btnAnd Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnd_Click(object sender, EventArgs e)
        {
            code = drawCode.And;
            first = second = null;
        }

        /// <summary>
        /// btnOr Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOr_Click(object sender, EventArgs e)
        {
            code = drawCode.Or;
            first = second = null;
        }

        /// <summary>
        /// btnNot Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNot_Click(object sender, EventArgs e)
        {
            code = drawCode.Not;
            first = second = null;
        }

        /// <summary>
        /// btnSource Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSource_Click(object sender, EventArgs e)
        {
            code = drawCode.Source;
            first = second = null;
        }

        /// <summary>
        /// btnSink Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSink_Click(object sender, EventArgs e)
        {
            code = drawCode.Sink;
            first = second = null;
        }

        /// <summary>
        /// btnConnect Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            code = drawCode.Connect;
            first = second = null;
        }

        /// <summary>
        /// btnDel Click event handler, deletes the connection/gate depends on which one is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {

            if (dynamicButton != null)
            {
                dynamicButton.Visible = false;
            }

            if (chosen != null)
            {
                circ.deleteGate(chosen);
                rowNum = colNum = 0;
                highlight = Rectangle.Empty;
                gridBox.Refresh();
                circ.reDraw(ref gridBox);
                chosen = null;
                chosenConn = null;
            }
            if (chosenConn != null)
            {
                circ.setUndoList();
                circ.deleteConnection(chosenConn);
                highlight = Rectangle.Empty;
                rowNum = colNum = 0;
                gridBox.Refresh();
                circ.reDraw(ref gridBox);
                chosenConn = null;
                chosen = null;
            }

        }
        /// <summary>
        /// Save file from menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = saveFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {

                circ.SaveCircuit(saveFileDialog1.FileName);

            }

        }
        /// <summary>
        /// Load file from menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                circ.LoadCircuit(openFileDialog1.FileName);
            }
            circ.reDraw(ref gridBox);
        }
        /// <summary>
        /// Save a circuit using the icon save.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult dr = saveFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {

                circ.SaveCircuit(saveFileDialog1.FileName);

            }

        }

        /// <summary>
        /// Delete menu Click event handler, deletes the connection/gate depends on which one is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dynamicButton != null)
            {
                dynamicButton.Visible = false;
            }
            if (chosen != null)
            {
                circ.deleteGate(chosen);
                rowNum = colNum = 0;
                highlight = Rectangle.Empty;
                gridBox.Refresh();
                circ.reDraw(ref gridBox);
                chosen = null;
            }
            if (chosenConn != null)
            {
                circ.setUndoList();
                circ.deleteConnection(chosenConn);
                highlight = Rectangle.Empty;
                rowNum = colNum = 0;
                gridBox.Refresh();
                circ.reDraw(ref gridBox);
                chosenConn = null;
                chosen = null;
            }
        }

        /// <summary>
        /// new menu click event handler, create a new circuit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            circ = new Circuit("Digital");
            gridBox.Refresh();
        }

        /// <summary>
        /// btnUndo Click event handler, undo an activity in the circuit by one step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUndo_Click(object sender, EventArgs e)
        {
            circ.undoCircuit();
            gridBox.Refresh();
            circ.reDraw(ref gridBox);
        }

        /// <summary>
        /// exit menu Click event handler, close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Undo menu Click event handler, undo an activity in the circuit by one step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            circ.undoCircuit();
            gridBox.Refresh();
            circ.reDraw(ref gridBox);
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
             circ.redoCircuit();
            gridBox.Refresh();
            circ.reDraw(ref gridBox);
           
        }
        
        private void gridBox_Click(object sender, EventArgs e)
        {
            
        }
        
    }
}
