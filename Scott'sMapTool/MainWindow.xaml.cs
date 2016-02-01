﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;

namespace Scott_sMapTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        //data tables for the DataGrid on the UI
        //table is what you see
        ObservableCollection<GridRowObject> table = new ObservableCollection<GridRowObject>();
        //these tables enable us to switch rapidly between tables after they've been populated for the first time
        ObservableCollection<GridRowObject> emptyTable = new ObservableCollection<GridRowObject>();
        ObservableCollection<GridRowObject> lambdaAverageTable = new ObservableCollection<GridRowObject>();
        ObservableCollection<GridRowObject> baseTuneTable = new ObservableCollection<GridRowObject>();
        ObservableCollection<GridRowObject> suggestedTuneTable = new ObservableCollection<GridRowObject>();
        //every time we open a lambda file we'll put it in a dictionary with the key bieng the shortened filename
        Dictionary<string, ObservableCollection<GridRowObject>> openedLambdaDictionary = new Dictionary<string, ObservableCollection<GridRowObject>>();

        //These two correspond to the listboxes shown and are connected to the dictionary below for full filepaths
        ObservableCollection<string> baseTune = new ObservableCollection<string>();
        ObservableCollection<string> lambdaList = new ObservableCollection<string>();
        //Here is said dictionary, mapping the strings in the listboxes to full filepaths
        Dictionary<string, string> filepaths = new Dictionary<string, string>();

        #endregion

        #region Custom Classes
        public class GridRowObject:ICloneable
        {
            public double? A { get; set; }
            public double? B { get; set; }
            public double? C { get; set; }
            public double? D { get; set; }
            public double? E { get; set; }
            public double? F { get; set; }
            public double? G { get; set; }
            public double? H { get; set; }
            public double? I { get; set; }
            public double? J { get; set; }
            public double? K { get; set; }
            public double? L { get; set; }
            public double? M { get; set; }
            public double? N { get; set; }
            public double? O { get; set; }
            public double? P { get; set; }
            public double? Q { get; set; }
            public double? R { get; set; }
            public double? S { get; set; }
            public double? T { get; set; }
            public double? U { get; set; }
            public double? V { get; set; }
            public double? W { get; set; }
            public double? X { get; set; }
            public double? Y { get; set; }
            public double? Z { get; set; }

            public object Clone()
            {
                GridRowObject newRow = (GridRowObject)MemberwiseClone();
                return newRow;
            }
        }
        #endregion

        #region public MainWindow()
        public MainWindow()
        {
            InitializeComponent();
            
            table.Add(new GridRowObject()
            {
                A = null,
                B = 0,
                C = 250,
                D = 500,
                E = 750,
                F = 1000,
                G = 1250,
                H = 1500,
                I = 1750,
                J = 2000,
                K = 2250,
                L = 2500,
                M = 2750,
                N = 3000,
                O = 3250,
                P = 3500,
                Q = 3750,
                R = 4000,
                S = 4250,
                T = 4500,
                U = 4750,
                V = 5000,
                W = 5250,
                X = 5500,
                Y = 5750,
                Z = 6000
            });
            for (int i = 0; i < 26; i++)
            {
                table.Add(new GridRowObject()
                {
                    A = null,
                    B = null,
                    C = null,
                    D = null,
                    E = null,
                    F = null,
                    G = null,
                    H = null,
                    I = null,
                    J = null,
                    K = null,
                    L = null,
                    M = null,
                    N = null,
                    O = null,
                    P = null,
                    Q = null,
                    R = null,
                    S = null,
                    T = null,
                    U = null,
                    V = null,
                    W = null,
                    X = null,
                    Y = null,
                    Z = null
                });
            }
            double rowHeader = 100.0;
            int index = 1;
            while (rowHeader >= 0)
            {
                table[index].A = rowHeader;
                rowHeader -= 4.0;
                index++;
            }
            dataGrid1.ItemsSource = table;
            //store the empty table (this will make my life easier)
            emptyTable = new ObservableCollection<GridRowObject>(NewCopy(table));
            lambdaAverageTable = new ObservableCollection<GridRowObject>(NewCopy(emptyTable));
            baseTuneTable = new ObservableCollection<GridRowObject>(NewCopy(emptyTable));
            suggestedTuneTable = new ObservableCollection<GridRowObject>(NewCopy(emptyTable));

            //now we set some listbox properties that I'm not sure how to set in the properties window
            TuneListBox.AllowDrop = true;
            //TuneListBox.DataContext = baseTune;
            TuneListBox.ItemsSource = baseTune;
            LambdaListBox.AllowDrop = true;
            //LambdaListBox.DataContext = lambdaList;
            LambdaListBox.ItemsSource = lambdaList;
        }
        #endregion

        #region events

            #region drag drop functionality
        /* THIS SECTION HANDLES DRAGGING AND DROPPING OF FILES INTO THE LIST BOXES */
        
        private void TuneListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
        }

        private void TuneListBox_Drop(object sender, DragEventArgs e)
        {
            // get the list of filenames from the drop event as an array of strings
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            //enforce filetypes
            if (files[0].Split('.').Last() != "ped")
            {
                //show an error message and don't add it to the list or the dictionary
                MessageBox.Show("This box only accepts files with a .ped extension.\n\nThese are the files Performance Electronics saves when you save tunes in their tuning program");
                return;
            }

            // for the base tune box we only want to have 1 tune, so we'll just replace it
            if (baseTune.Count > 0)
                baseTune.Clear();

            //take only the first one even if they dragged more than one
            //Let's also strip off everything but the name
            string filename = files[0].Split('\\').Last();
            baseTune.Add(filename);
            filepaths.Add(filename, files[0]);

            ProcessPED();

            /* this is an example of how to use the dictionary we just populated. Pretty handy, huh? Just look it up

            string message = string.Format("the filepath for {0} is {1}", baseTune[0], filepaths[baseTune[0]]);

            */
        }

        private void LambdaListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
        }

        private void LambdaListBox_Drop(object sender, DragEventArgs e)
        {
            // get the list of filenames from the drop event as an array of strings
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            //take only the first one even if they dragged more than one
            //Let's also strip off everything but the name
            bool messageShown = false;
            foreach (string file in files)
            {
                bool add = true;
                //enforce filetypes
                if(file.Split('.').Last() != "csv" )
                {
                    //show an error message and don't add it to the list or the dictionary
                    //show an error message only once for a list of bad filetypes
                    if (messageShown == false)
                    {
                        MessageBox.Show("This box only accepts files with a .csv extension.\n\nThese should be data aquisition files taken from performance electronics \n\nWe'll still add all of the .csv files you attempted to add (you can drag multiple).");
                        messageShown = true;
                    }
                    add = false;
                }

                //add it to the list and the dictionary
                if (add)
                {
                    string filename = file.Split('\\').Last();
                    try
                    {
                        filepaths.Add(filename, file);
                        lambdaList.Add(filename);
                        ProcessCSV(filename);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(string.Format("You've hit an exception. Don't worry, this probably just means you tried to add a file that is already in the list. We won't add that one. If that is the case then the following message will say you already added that key. Here is the message: \n\n\"{0}\"",ex.Message));
                    }
                }
            }
        }
            #endregion

            #region Button Clicks

                #region File
        //This function essentially just reinitializes the window and its data
        //linked to File>New
        private void New_Click(object sender, RoutedEventArgs e)
        {
            ReInitializeObjects();
        }

        //Generic close event linked to File>Exit
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
                #endregion

                #region Show
        private void Show_BaseMap_Click(object sender, RoutedEventArgs e)
        {
            table = new ObservableCollection<GridRowObject>(NewCopy(baseTuneTable));
            dataGrid1.ItemsSource = table;
        }

        private void Show_SuggestedMap_Click(object sender, RoutedEventArgs e)
        {
            table = new ObservableCollection<GridRowObject>(NewCopy(suggestedTuneTable));
            dataGrid1.ItemsSource = table;
        }

        private void Show_LambdaAverages_Click(object sender, RoutedEventArgs e)
        {
            lambdaAverageTable = new ObservableCollection<GridRowObject>(NewCopy(openedLambdaDictionary[lambdaList.First()]));
            table = new ObservableCollection<GridRowObject>(NewCopy(lambdaAverageTable));
            dataGrid1.ItemsSource = table;
        }
                #endregion

            #endregion

        #endregion

        #region Private
        //this function makes it so we can copy by value and not by reference. We don't want the same object, just the same 
        private ObservableCollection<GridRowObject> NewCopy(ObservableCollection<GridRowObject> oldObservable)
        {
            ObservableCollection<GridRowObject> newObservable = new ObservableCollection<GridRowObject>();
            foreach(GridRowObject row in oldObservable)
            {
                newObservable.Add((GridRowObject)row.Clone());
            }
            return newObservable;
        }

        private void ReInitializeObjects()
        {
            //clear the table out except for the headers
            table = new ObservableCollection<GridRowObject>(NewCopy(emptyTable));
            lambdaAverageTable = new ObservableCollection<GridRowObject>(NewCopy(emptyTable)); 
            baseTuneTable = new ObservableCollection<GridRowObject>(NewCopy(emptyTable)); 
            suggestedTuneTable = new ObservableCollection<GridRowObject>(NewCopy(emptyTable));
            dataGrid1.ItemsSource = table;

            //clear out the lists
            if (baseTune.Count > 0)
                baseTune.Clear();
            if (lambdaList.Count > 0)
                lambdaList.Clear();
            if (filepaths.Count > 0)
                filepaths.Clear();
        }

        private void ProcessCSV(string name)
        {
            // keep in mind that this file is a single column of time data, not a map! 
            // we can make him stronger...
            // we have the technology...
            
            StreamReader reader = null;
            try
            {
                //look up the full filepath that we captured from the drag-drop events
                reader = new StreamReader(File.OpenRead(filepaths[name]));
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            //the first line contains header information.
            //we can use this info to find the columns we want to process rather than hard-coding
            string line = reader.ReadLine();
            List<string> columnHeaders = line.Split(',').ToList();  // a collection of all the headers
            int RPMcolumnIndex = columnHeaders.IndexOf("RPM");      // RPM 0-based column index in the .csv file
            int TPScolumnIndex = columnHeaders.IndexOf("TPS (%)");  // TPS 0-based column index in the .csv file
            int MAPcolumnIndex = columnHeaders.IndexOf("MAP (psi)");  // TPS 0-based column index in the .csv file
            int LambdaMeasuredcolumnIndex = columnHeaders.IndexOf("Lambda Measured");  // TPS 0-based column index in the .csv file
            //create lists for each variable
            List<double> rpm = new List<double>();
            List<double> tps = new List<double>();
            List<double> map = new List<double>();
            List<double> lambda = new List<double>();
            double[,,] cells = new double[25, 26, 2];

            //now that we know column indicies we can use them to read the rest of the file
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                string[] values = line.Split(',');
                rpm.Add(Convert.ToDouble(values[RPMcolumnIndex]));
                tps.Add(Convert.ToDouble(values[TPScolumnIndex]));
                map.Add(Convert.ToDouble(values[MAPcolumnIndex]));
                lambda.Add(Convert.ToDouble(values[LambdaMeasuredcolumnIndex]));
            }
            reader.Close();

            //Generate map of unaveraged data
            for (int i = 0; i < lambda.Count; i++)
            {
                //casting as an int truncates, which is what we want I'm pretty sure.
                int rpmIndex = (int)(rpm[i] / 250);
                int tpsIndex = 25 - (int)(tps[i] / 4);
                int mapIndex = (int)((map[i] - 2.5) / 0.5);

                cells[rpmIndex, tpsIndex, 0] += 1.0;
                cells[rpmIndex, tpsIndex, 1] += lambda[i];
            }

            //from here we can generate a map of data and add it to the master list of lambda files that have been processed
            ObservableCollection<GridRowObject> newMeasuredLambdaMap = new ObservableCollection<GridRowObject>(NewCopy(emptyTable));
            for (int tpsIndex = 0; tpsIndex < 26; tpsIndex++)
            {
                for (int rpmIndex = 0; rpmIndex < 25; rpmIndex++)
                {
                    //the reason this loop is organized all wonky is that I reorganized it like 20 different times
                    //it turned into a frankenstein monster taking part of all of them
                    bool shouldBeNulled = false;
                    if (cells[rpmIndex, tpsIndex, 1] == 0)
                        shouldBeNulled = true;
                    switch (rpmIndex)
                    {
                        case 0:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].B = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].B += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 1:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].C = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].C += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 2:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].D = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].D += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 3:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].E = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].E += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 4:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].F = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].F += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 5:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].G = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].G += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 6:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].H = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].H += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 7:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].I = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].I += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 8:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].J = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].J += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 9:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].K = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].K += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 10:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].L = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].L += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 11:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].M = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].M += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 12:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].N = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].N += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 13:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].O = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].O += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 14:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].P = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].P += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 15:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].Q = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].Q += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 16:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].R = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].R += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 17:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].S = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].S += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 18:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].T = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].T += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 19:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].U = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].U += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 20:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].V = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].V += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 21:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].W = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].W += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 22:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].X = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].X += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 23:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].Y = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].Y += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        case 24:
                            if (!shouldBeNulled)
                                newMeasuredLambdaMap[tpsIndex+1].Z = 0.0;
                            newMeasuredLambdaMap[tpsIndex+1].Z += Math.Round(cells[rpmIndex, tpsIndex, 1] / cells[rpmIndex, tpsIndex, 0],2);
                            break;
                        default:
                            MessageBox.Show("What the what? Tell scott to take a look at this. This shouldn't happen...");
                            break;
                    }
                }
            }

            //now just add that lambda to the dictionary, we'll average them when the user asks to see the averages for now
            openedLambdaDictionary.Add(name, newMeasuredLambdaMap);
        }

        private void ProcessPED()
        {
            //populate bytes array with data
            byte[] fileBytes = null;
            string baseTuneFilePath = filepaths[baseTune[0]];
            try
            {
                fileBytes = File.ReadAllBytes(baseTuneFilePath);
            }
            catch (IOException ex)
            {
                MessageBox.Show(string.Format("Something went wrong reading in that base tune file, here is the exception message: \n\n\"{0}\"",ex.Message));
                return;
            }

            //grab the fuel table out of the binary file
            int j = 0, k = 25;
            double[,] fuelTable = new double[25, 26];
            //From what I've seen so far, this is ALWAYS where the fuel table is in their binary .ped file 
            //(makes sense because it's probably just a serializable object that gets written to the .ped)
            for (int i = 185; i < 1485; i++)
            {
                if (i % 2 == 1) //skip all of the empty bytes (every other one)
                {
                    if (k == -1) //increment down each column (j) one by one (rows = k)
                    {
                        k = 25;
                        j++;
                    }
                    //convert from their binary format to a double
                    fuelTable[j, k] = ((uint)fileBytes[i] + 1) * 0.015625;
                    k--;
                }
            }

            baseTuneTable = new ObservableCollection<GridRowObject>(NewCopy(emptyTable));
            for (int y = 1; y <= 26; y++)
            {
                baseTuneTable[27-y].B = Math.Round(fuelTable[0, y - 1],2);
                baseTuneTable[27-y].C = Math.Round(fuelTable[1, y - 1], 2);
                baseTuneTable[27-y].D = Math.Round(fuelTable[2, y - 1], 2);
                baseTuneTable[27-y].E = Math.Round(fuelTable[3, y - 1], 2);
                baseTuneTable[27-y].F = Math.Round(fuelTable[4, y - 1], 2);
                baseTuneTable[27-y].G = Math.Round(fuelTable[5, y - 1], 2);
                baseTuneTable[27-y].H = Math.Round(fuelTable[6, y - 1], 2);
                baseTuneTable[27-y].I = Math.Round(fuelTable[7, y - 1], 2);
                baseTuneTable[27-y].J = Math.Round(fuelTable[8, y - 1], 2);
                baseTuneTable[27-y].K = Math.Round(fuelTable[9, y - 1], 2);
                baseTuneTable[27-y].L = Math.Round(fuelTable[10, y - 1], 2);
                baseTuneTable[27-y].M = Math.Round(fuelTable[11, y - 1], 2);
                baseTuneTable[27-y].N = Math.Round(fuelTable[12, y - 1], 2);
                baseTuneTable[27-y].O = Math.Round(fuelTable[13, y - 1], 2);
                baseTuneTable[27-y].P = Math.Round(fuelTable[14, y - 1], 2);
                baseTuneTable[27-y].Q = Math.Round(fuelTable[15, y - 1], 2);
                baseTuneTable[27-y].R = Math.Round(fuelTable[16, y - 1], 2);
                baseTuneTable[27-y].S = Math.Round(fuelTable[17, y - 1], 2);
                baseTuneTable[27-y].T = Math.Round(fuelTable[18, y - 1], 2);
                baseTuneTable[27-y].U = Math.Round(fuelTable[19, y - 1], 2);
                baseTuneTable[27-y].V = Math.Round(fuelTable[20, y - 1], 2);
                baseTuneTable[27-y].W = Math.Round(fuelTable[21, y - 1], 2);
                baseTuneTable[27-y].X = Math.Round(fuelTable[22, y - 1], 2);
                baseTuneTable[27-y].Y = Math.Round(fuelTable[23, y - 1], 2);
                baseTuneTable[27-y].Z = Math.Round(fuelTable[24, y - 1], 2);
            }
            
        }


        #endregion
    }
}
