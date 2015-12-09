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
using Microsoft.Office.Interop.Excel;

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
                int rpmIndex = 0;
                if (textBox1.Text != null)
                    rpmIndex = (int)(rpm[i] / Convert.ToUInt32(textBox1.Text));
                else
                    MessageBox.Show("put a number into the RPM step size box");
                    return;
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
#if DEBUG   //debug output
            string outputFilePath = @"../../../Outputs/Measured Lambda Table.csv";
#else       //release output
            //TODO put a different output for the release build here
            string outputFilePath = @"C:/Users/Mobile Wind Tunnel/Desktop/Scott's Tunes/Data Aquisition/convertedFiles/Measured Lambda Table.csv";
            
#endif

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
                            csv.Append(string.Format("{0}, ", (j * Convert.ToUInt32(textBox1.Text)).ToString()));
                        }
                        else if(j == 24)
                        {
                            csv.Append(string.Format("{0}{1} ", (j * Convert.ToUInt32(textBox1.Text)).ToString(), Environment.NewLine));
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

        private void button3_Click(object sender, EventArgs e)
        {
            //populate bytes array with data
            byte[] fileBytes = null;
            foreach (string filename in pedFile)
            {
                try
                {
                    fileBytes = File.ReadAllBytes(filename);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }

            //grab the fuel table out of the binary file
            int j = 0, k = 25;
            double[,] fuelTable = new double[25, 26];
            for (int i = 185; i < 1485; i++)
            {
                if (i % 2 == 1) //skip all of the empty bytes (every other one)
                {
                    if (k == -1) //increment down each column (j) one by one (rows = k)
                    {
                        k = 25;
                        j++;
                    }
                    fuelTable[j, k] = ((uint)fileBytes[i] + 1) * 0.015625;
                    k--;
                }
            }

            //create lists for each variable
            List<double> rpm = new List<double>();
            List<double> tps = new List<double>();
            List<double> map = new List<double>();
            List<double> lambda = new List<double>();

            //creates array to store cells of final .csv matrix
            double[, ,] cells = new double[25, 26, 2];

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
            for (int i = 0; i < lambda.Count; i++)
            {
                //casting as an int truncates, which is what we want I'm pretty sure.
                int rpmIndex = (int)(rpm[i] / 250);
                int tpsIndex = (int)(tps[i] / 4);
                int mapIndex = (int)((map[i] - 2.5) / 0.5);
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

            //average the collected values into a lambda table
            double[,] lambdaMeasuredTable = new double[25, 26];
            for (int i = 0; i < 25; i++)
            {
                for (j = 0; j < 26; j++)
                {
                    if (cells[i, j, 0] != 0)
                        lambdaMeasuredTable[i, j] = cells[i, j, 1] / cells[i, j, 0];
                    else
                        lambdaMeasuredTable[i, j] = 1;
                }
            }
            
            //adjust the fuel table to try and achieve 1.05
            for (int i = 0; i < 25; i++)
            {
                for (j = 0; j < 26; j++)
                {
                    fuelTable[i, j] *= lambdaMeasuredTable[i, j];
                }
            }

#if DEBUG   //debug output
            string outputFilePath = @"../../../Outputs/Suggested Fuel Table.csv";
#else       //release output
            string outputFilePath = @"C:/Users/Mobile Wind Tunnel/Desktop/Scott's Tunes/Data Aquisition/convertedFiles/Suggested Fuel Table.csv";
#endif

            StringBuilder csv = new StringBuilder();

            for (int i = 26; i >= 0; i--)
            {
                for (j = 0; j < 25; j++)
                {
                    if (i == 26)
                    {
                        if (j == 0)
                        {
                            csv.Append(" ,");
                        }
                        if (j != 24)
                        {
                            csv.Append(string.Format("{0}, ", (j * 250).ToString()));
                        }
                        else if (j == 24)
                        {
                            csv.Append(string.Format("{0}{1} ", (j * 250).ToString(), Environment.NewLine));
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            //add in the tps value column
                            if (Y_Value_DropDown.Text == "TPS   (%)")
                            {
                                csv.Append(string.Format("{0}, ", (i * 4).ToString()));
                            }
                            //add in the map value column
                            else
                            {
                                csv.Append(string.Format("{0}, ", (2.5 + (i * 0.5)).ToString()));
                            }
                        }
                        if (j != 24)
                        {
                            if (cells[j, i, 0] != 0)
                            {
                                csv.Append(string.Format("{0}, ", (fuelTable[j,i]).ToString()));
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
                                csv.Append(string.Format("{0}{1}", (fuelTable[j,i]).ToString(), Environment.NewLine));
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


            MessageBox.Show(string.Format("File written to {0}", outputFilePath));
        }

        //fire up excel
        private void button4_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = true;
#if DEBUG
            //this would be the path on my pace lab computer where I actually work on this
#else
            //just plan on running release on the supermileage laptop
            excel.Workbooks.Open(@"C:/Users/Mobile Wind Tunnel/Desktop/Scott's Tunes/Data Aquisition/convertedFiles/Suggested Fuel Table.csv");
            excel.Workbooks.Open(@"C:/Users/Mobile Wind Tunnel/Desktop/Scott's Tunes/Data Aquisition/convertedFiles/Measured Lambda Table.csv");
#endif
        }
    }

}
