using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DigitalCircuit
{
    public partial class subForm : Form
    {
        public subForm()
        {
            InitializeComponent();
        }

        private void subForm_Load(object sender, EventArgs e)
        {
            string path = "help.txt";
            try  
              {
            StreamReader textIn = new StreamReader(
                                        new FileStream(
                                            path,FileMode.Open,FileAccess.Read));
                while (textIn.Peek() != -1)
                {
                    string line = textIn.ReadLine();
                    listBox1.Items.Add(line);
                }
                textIn.Close();
            }
        
        catch (Exception ex) 
        {
            // Let the user know what went wrong.
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(ex.Message);
        }
    }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
           
        }
    }

