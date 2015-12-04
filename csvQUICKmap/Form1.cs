using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csvQUICKmap
{
    public partial class Form1 : Form
    {
        //variables 
        private BindingList<string> filenames = new BindingList<string>();
        private BindingList<string> pedFile = new BindingList<string>();

        //some form settings
        public Form1()
        {
            InitializeComponent();
            //intialize the drop ability of the listbox1 for .csv filename display
            listBox1.AllowDrop = true;
            listBox1.DragEnter += new DragEventHandler(listBox1_DragEnter);
            listBox1.DragDrop += new DragEventHandler(listBox1_DragDrop);
            listBox1.DataSource = filenames;

            //intialize the drop ability of the listbox2 for .ped filename display
            listBox2.AllowDrop = true;
            listBox2.DragEnter += new DragEventHandler(listBox2_DragEnter);
            listBox2.DragDrop += new DragEventHandler(listBox2_DragDrop);
            listBox2.DataSource = pedFile;
        }

        //events
        void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                filenames.Add(file);
            }
            listBox1.Update();
        }

        void listBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void listBox2_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (pedFile.Count < 1)
                    pedFile.Add(file);
                else
                    MessageBox.Show("Only one .ped file can be edited at a time");
            }
            listBox2.Update();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            //create lists for each variable
            List<double> rpm = new List<double>();
            List<double> tps = new List<double>();
            List<double> map = new List<double>();
            List<double> lambda = new List<double>();

            //creates array to store cells of final .csv matrix
            double[,,] cells = new double[25,26,2];

            //populate lists with data
            foreach (string filename in filenames)
            {
                StreamReader reader = null;
                try
                {
                    reader = new StreamReader(File.OpenRead(filename));
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                string line = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    string[] values = line.Split(',');
                    rpm.Add(Convert.ToDouble(values[1]));
                    tps.Add(Convert.ToDouble(values[4]));
                    map.Add(Convert.ToDouble(values[6]));
                    lambda.Add(Convert.ToDouble(values[10]));
                }
                reader.Close();
                
            }

            //Generate map of data
            for(int i = 0; i < lambda.Count; i++)
            {
                //casting as an int truncates, which is what we want I'm pretty sure.
                int rpmIndex = (int)(rpm[i]/250);
                int tpsIndex = (int)(tps[i]/4);
                int mapIndex = (int)((map[i] - 2.5)/0.5);
                if (Y_Value_DropDown.Text == "TPS   (%)") //throttle position being used 
                {
                    cells[rpmIndex, tpsIndex, 0] += 1;
                    cells[rpmIndex, tpsIndex, 1] += lambda[i];
                }
                else  // MAP Sensor being used
                {
                    cells[rpmIndex, mapIndex, 0] += 1;
                    cells[rpmIndex, mapIndex, 1] += lambda[i];
                }
            }

            //generate unique filepath that (current local time)
            string outputFilePath = @"../../../Outputs/output.csv";
            StringBuilder csv = new StringBuilder();

            for(int i = 26; i >= 0; i--)
            {
                for(int j = 0; j < 25; j++)
                {
                    if (i == 26)
                    {
                        if(j == 0)
                        {
                            csv.Append(" ,");
                        }
                        if(j != 24)
                        {
                            csv.Append(string.Format("{0}, ", (j * 250).ToString()));
                        }
                        else if(j == 24)
                        {
                            csv.Append(string.Format("{0}{1} ", (j * 250).ToString(), Environment.NewLine));
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            //add in the tps value column
                            if(Y_Value_DropDown.Text == "TPS   (%)")
                            {
                                csv.Append(string.Format("{0}, ", (i*4).ToString()));
                            }
                            //add in the map value column
                            else
                            {
                                csv.Append(string.Format("{0}, ", (2.5 + (i*0.5)).ToString()));
                            }
                        }
                        if(j != 24)
                        {
                            if (cells[j, i, 0] != 0)
                            {
                                csv.Append(string.Format("{0}, ", (cells[j, i, 1] / cells[j, i, 0]).ToString()));
                            }
                            else
                            {
                                csv.Append("-1, ");
                            }
                        }
                        else if (j == 24)
                        {
                            if (cells[j, i, 0] != 0)
                            {
                                csv.Append(string.Format("{0}{1}", (cells[j, i, 1] / cells[j, i, 0]).ToString(), Environment.NewLine));
                            }
                            else
                            {
                                csv.Append(string.Format("{0}{1}", "-1", Environment.NewLine));
                            }
                        }
                    }
                }
            }

            try
            {
                File.WriteAllText(outputFilePath.ToString(), csv.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            MessageBox.Show(string.Format("File written to {0}",outputFilePath));
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (pedFile.Count > 0)
            {
                pedFile.Remove(listBox2.SelectedItem.ToString());
                listBox2.Update();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (filenames.Count > 0)
            {
                filenames.Remove(listBox1.SelectedItem.ToString());
                listBox1.Update();
            }
        }
    }

}
