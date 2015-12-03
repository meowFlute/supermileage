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
        private double[,,] cells = new double[25,26,2];

        //some form settings
        public Form1()
        {
            InitializeComponent();
            listBox1.AllowDrop = true;
            listBox1.DragEnter += new DragEventHandler(listBox1_DragEnter);
            listBox1.DragDrop += new DragEventHandler(listBox1_DragDrop);
            listBox1.DataSource = filenames;
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

            //populate lists with data
            foreach (string filename in filenames)
            {
                StreamReader reader = new StreamReader(File.OpenRead(filename));
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
                if (Y_Value_DropDown.Text == "TPS   (%)")
                {
                    cells[rpmIndex, tpsIndex, 0] += 1;
                    cells[rpmIndex, tpsIndex, 1] += lambda[i];
                }
                else
                {
                    cells[rpmIndex, tpsIndex, 0] += 1;
                    cells[rpmIndex, tpsIndex, 1] += lambda[i];
                }
            }

            //generate unique filepath that (current local time)
            string outputFilePath = @"C:/Users\Mobile Wind Tunnel/Desktop/Scott's Tunes/Data Aquisition/convertedFiles/output.csv";
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

            File.WriteAllText(outputFilePath.ToString(), csv.ToString());

            MessageBox.Show(string.Format("File written to {0}",outputFilePath));
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            filenames.Remove(listBox1.SelectedItem.ToString());
            listBox1.Update();
        }
    }

}
