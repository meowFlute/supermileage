using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;

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

        //this is the scaling factor for the averaged lambda data that we'll use to tune the engine
        public double fuelScalingFactor = 1.0;

        #endregion

        #region Custom Classes

        public class GridRowObject:ICloneable
        {
            public double? A { get; set; } = null;
            public SolidColorBrush AColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? B { get; set; } = null;
            public SolidColorBrush BColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? C { get; set; } = null;
            public SolidColorBrush CColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? D { get; set; } = null;
            public SolidColorBrush DColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? E { get; set; } = null;
            public SolidColorBrush EColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? F { get; set; } = null;
            public SolidColorBrush FColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? G { get; set; } = null;
            public SolidColorBrush GColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? H { get; set; } = null;
            public SolidColorBrush HColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? I { get; set; } = null;
            public SolidColorBrush IColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? J { get; set; } = null;
            public SolidColorBrush JColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? K { get; set; } = null;
            public SolidColorBrush KColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? L { get; set; } = null;
            public SolidColorBrush LColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? M { get; set; } = null;
            public SolidColorBrush MColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? N { get; set; } = null;
            public SolidColorBrush NColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? O { get; set; } = null;
            public SolidColorBrush OColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? P { get; set; } = null;
            public SolidColorBrush PColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? Q { get; set; } = null;
            public SolidColorBrush QColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? R { get; set; } = null;
            public SolidColorBrush RColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? S { get; set; } = null;
            public SolidColorBrush SColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? T { get; set; } = null;
            public SolidColorBrush TColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? U { get; set; } = null;
            public SolidColorBrush UColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? V { get; set; } = null;
            public SolidColorBrush VColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? W { get; set; } = null;
            public SolidColorBrush WColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? X { get; set; } = null;
            public SolidColorBrush XColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? Y { get; set; } = null;
            public SolidColorBrush YColor { get; set; } = new SolidColorBrush(Colors.Black);

            public double? Z { get; set; } = null;
            public SolidColorBrush ZColor { get; set; } = new SolidColorBrush(Colors.Black);

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
            if (lambdaList.Count > 0)   // if we updated the PED file and the lambda list has stuff in it 
                ProcessSuggestedMAP();  // then we update the suggested map too since we can
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

            // after they are all in there, we set the average lambda data again. We do this everytime the list changes
            averageLambdaTables();
            if (baseTune.Count > 0)   // if we updated the PED file and the lambda list has stuff in it 
                ProcessSuggestedMAP();  // then we update the suggested map too since we can
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

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Multiselect = true;
            
            dlg.Filter = "ECU Recorded Data (.csv)|*.csv|ECU MAP files (.ped)|*.ped";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                foreach (string filename in dlg.FileNames)
                {
                    if (filename.Split('.').Last() == "csv")
                    {
                        string shortName = filename.Split('\\').Last();
                        try
                        {
                            filepaths.Add(shortName, filename);
                            lambdaList.Add(shortName);
                            ProcessCSV(shortName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("You've hit an exception. Don't worry, this probably just means you tried to add a file that is already in the list. We won't add that one. If that is the case then the following message will say you already added that key. Here is the message: \n\n\"{0}\"", ex.Message));
                        }
                    }
                    else if (filename.Split('.').Last() == "ped")
                    {
                        // for the base tune box we only want to have 1 tune, so we'll just replace it
                        if (baseTune.Count > 0)
                        {
                            filepaths.Remove(baseTune[0]);
                            baseTune.Clear();
                        }

                        //take only the first one even if they dragged more than one
                        //Let's also strip off everything but the name
                        string shortName = filename.Split('\\').Last();
                        baseTune.Add(shortName);
                        filepaths.Add(shortName, filename);

                        ProcessPED();
                        if (lambdaList.Count > 0)   // if we updated the PED file and the lambda list has stuff in it 
                            ProcessSuggestedMAP();  // then we update the suggested map too since we can
                        return;
                    }
                    else
                        MessageBox.Show("You can only open .csv and .ped files\n-Love\n\nScott");
                }

                if (dlg.FileNames[0].Split('.').Last() == "csv")
                {
                    averageLambdaTables();
                    if (baseTune.Count > 0)
                        ProcessSuggestedMAP();
                }
            }
        }

        private void BaseTuneAdd_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Multiselect = true;

            dlg.Filter = "ECU MAP files (.ped)|*.ped";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                foreach (string filename in dlg.FileNames)
                {
                    if (filename.Split('.').Last() == "csv")
                    {
                        string shortName = filename.Split('\\').Last();
                        try
                        {
                            filepaths.Add(shortName, filename);
                            lambdaList.Add(shortName);
                            ProcessCSV(shortName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("You've hit an exception. Don't worry, this probably just means you tried to add a file that is already in the list. We won't add that one. If that is the case then the following message will say you already added that key. Here is the message: \n\n\"{0}\"", ex.Message));
                        }
                    }
                    else if (filename.Split('.').Last() == "ped")
                    {
                        // for the base tune box we only want to have 1 tune, so we'll just replace it
                        if (baseTune.Count > 0)
                        {
                            filepaths.Remove(baseTune[0]);
                            baseTune.Clear();
                        }

                        //take only the first one even if they dragged more than one
                        //Let's also strip off everything but the name
                        string shortName = filename.Split('\\').Last();
                        baseTune.Add(shortName);
                        filepaths.Add(shortName, filename);

                        ProcessPED();
                        if (lambdaList.Count > 0)   // if we updated the PED file and the lambda list has stuff in it 
                            ProcessSuggestedMAP();  // then we update the suggested map too since we can
                        return; //this way we only add one to our filepaths dictionary
                    }
                    else
                        MessageBox.Show("You can only open .csv and .ped files\n-Love\n\nScott");
                }

                if (dlg.FileNames[0].Split('.').Last() == "csv")
                {
                    averageLambdaTables();
                    if (baseTune.Count > 0)
                        ProcessSuggestedMAP();
                }
            }
        }

        private void BaseTuneRemove_Click(object sender, RoutedEventArgs e)
        {
            if(baseTune.Count > 0)
            {
                string[] itemsToRemove = new string[TuneListBox.SelectedItems.Count];
                TuneListBox.SelectedItems.CopyTo(itemsToRemove, 0);

                foreach(string item in itemsToRemove)
                {
                    filepaths.Remove(item);
                    baseTune.Remove(item);
                    baseTuneTable = new ObservableCollection<GridRowObject>(NewCopy(emptyTable));
                    suggestedTuneTable = new ObservableCollection<GridRowObject>(NewCopy(emptyTable));
                }
            }
        }

        private void LambdRemove_Click(object sender, RoutedEventArgs e)
        {
            if(lambdaList.Count > 0)
            {
                string[] itemsToRemove = new string[LambdaListBox.SelectedItems.Count];
                LambdaListBox.SelectedItems.CopyTo(itemsToRemove, 0);

                foreach(string item in itemsToRemove)
                {
                    filepaths.Remove(item);
                    openedLambdaDictionary.Remove(item);
                    lambdaList.Remove(item);
                    averageLambdaTables();
                    if (baseTune.Count > 0)
                        ProcessSuggestedMAP();
                }
            }
        }

        private void LambdaAdd_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Multiselect = true;

            dlg.Filter = "ECU Recorded Data (.csv)|*.csv";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                foreach (string filename in dlg.FileNames)
                {
                    if (filename.Split('.').Last() == "csv")
                    {
                        string shortName = filename.Split('\\').Last();
                        try
                        {
                            filepaths.Add(shortName, filename);
                            lambdaList.Add(shortName);
                            ProcessCSV(shortName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("You've hit an exception. Don't worry, this probably just means you tried to add a file that is already in the list. We won't add that one. If that is the case then the following message will say you already added that key. Here is the message: \n\n\"{0}\"", ex.Message));
                        }
                    }
                    else if (filename.Split('.').Last() == "ped")
                    {
                        // for the base tune box we only want to have 1 tune, so we'll just replace it
                        if (baseTune.Count > 0)
                            baseTune.Clear();

                        //take only the first one even if they dragged more than one
                        //Let's also strip off everything but the name
                        string shortName = filename.Split('\\').Last();
                        baseTune.Add(shortName);
                        filepaths.Add(shortName, filename);

                        ProcessPED();
                        if (lambdaList.Count > 0)   // if we updated the PED file and the lambda list has stuff in it 
                            ProcessSuggestedMAP();  // then we update the suggested map too since we can
                    }
                    else
                        MessageBox.Show("You can only open .csv and .ped files\n-Love\n\nScott");
                }

                if (dlg.FileNames[0].Split('.').Last() == "csv")
                {
                    averageLambdaTables();
                    if (baseTune.Count > 0)
                        ProcessSuggestedMAP();
                }
            }
        }

        private void Re_Process_Map_Click(object sender, RoutedEventArgs e)
        {
            double testTextBox;
            if (lambdaList.Count > 0 && baseTune.Count > 0 && double.TryParse(desiredLambda.Text, out testTextBox))
            {
                ProcessSuggestedMAP();
                table = new ObservableCollection<GridRowObject>(NewCopy(suggestedTuneTable));
                dataGrid1.ItemsSource = table;
            }
            else
                MessageBox.Show("You must have a valid base tune, a list of data aquisition files, and a desired lambda");
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
            table = new ObservableCollection<GridRowObject>(NewCopy(lambdaAverageTable));
            dataGrid1.ItemsSource = table;
        }
        #endregion

        #endregion

        #endregion

        #region Private
        //this function does the leg work for averaging the tables we have loaded
        private void averageLambdaTables()
        {
            lambdaAverageTable = new ObservableCollection<GridRowObject>(NewCopy(emptyTable));
            foreach(KeyValuePair<string, ObservableCollection<GridRowObject>> pair in openedLambdaDictionary.ToList())
            {
                //pair.Value is the observable collection of the open file
                for(int tpsIndex = 1; tpsIndex < 27; tpsIndex++)
                {
                    // a null + double addition results in null, so we don't want to add a null and we need to check for it

                    // field B
                    if (pair.Value[tpsIndex].B != null && lambdaAverageTable[tpsIndex].B == null)
                        lambdaAverageTable[tpsIndex].B = 0.0;
                    if (pair.Value[tpsIndex].B != null)
                        lambdaAverageTable[tpsIndex].B += pair.Value[tpsIndex].B / openedLambdaDictionary.Count;

                    // field C
                    if (pair.Value[tpsIndex].C != null && lambdaAverageTable[tpsIndex].C == null)
                        lambdaAverageTable[tpsIndex].C = 0.0;
                    if (pair.Value[tpsIndex].C != null)
                        lambdaAverageTable[tpsIndex].C += pair.Value[tpsIndex].C / openedLambdaDictionary.Count;

                    // field D
                    if (pair.Value[tpsIndex].D != null && lambdaAverageTable[tpsIndex].D == null)
                        lambdaAverageTable[tpsIndex].D = 0.0;
                    if (pair.Value[tpsIndex].D != null)
                        lambdaAverageTable[tpsIndex].D += pair.Value[tpsIndex].D / openedLambdaDictionary.Count;

                    // field E
                    if (pair.Value[tpsIndex].E != null && lambdaAverageTable[tpsIndex].E == null) 
                        lambdaAverageTable[tpsIndex].E = 0.0;
                    if (pair.Value[tpsIndex].E != null) 
                        lambdaAverageTable[tpsIndex].E += pair.Value[tpsIndex].E / openedLambdaDictionary.Count;

                    // field F
                    if (pair.Value[tpsIndex].F != null && lambdaAverageTable[tpsIndex].F == null)
                        lambdaAverageTable[tpsIndex].F = 0.0;
                    if (pair.Value[tpsIndex].F != null)
                        lambdaAverageTable[tpsIndex].F += pair.Value[tpsIndex].F / openedLambdaDictionary.Count;

                    // field G
                    if (pair.Value[tpsIndex].G != null && lambdaAverageTable[tpsIndex].G == null)
                        lambdaAverageTable[tpsIndex].G = 0.0;
                    if (pair.Value[tpsIndex].G != null)
                        lambdaAverageTable[tpsIndex].G += pair.Value[tpsIndex].G / openedLambdaDictionary.Count;

                    // field H
                    if (pair.Value[tpsIndex].H != null && lambdaAverageTable[tpsIndex].H == null)
                        lambdaAverageTable[tpsIndex].H = 0.0;
                    if (pair.Value[tpsIndex].H != null)
                        lambdaAverageTable[tpsIndex].H += pair.Value[tpsIndex].H / openedLambdaDictionary.Count;

                    // field I
                    if (pair.Value[tpsIndex].I != null && lambdaAverageTable[tpsIndex].I == null)
                        lambdaAverageTable[tpsIndex].I = 0.0;
                    if (pair.Value[tpsIndex].I != null)
                        lambdaAverageTable[tpsIndex].I += pair.Value[tpsIndex].I / openedLambdaDictionary.Count;

                    // field J
                    if (pair.Value[tpsIndex].J != null && lambdaAverageTable[tpsIndex].J == null)
                        lambdaAverageTable[tpsIndex].J = 0.0;
                    if (pair.Value[tpsIndex].J != null)
                        lambdaAverageTable[tpsIndex].J += pair.Value[tpsIndex].J / openedLambdaDictionary.Count;

                    // field K
                    if (pair.Value[tpsIndex].K != null && lambdaAverageTable[tpsIndex].K == null)
                        lambdaAverageTable[tpsIndex].K = 0.0;
                    if (pair.Value[tpsIndex].K != null)
                        lambdaAverageTable[tpsIndex].K += pair.Value[tpsIndex].K / openedLambdaDictionary.Count;

                    // field L
                    if (pair.Value[tpsIndex].L != null && lambdaAverageTable[tpsIndex].L == null)
                        lambdaAverageTable[tpsIndex].L = 0.0;
                    if (pair.Value[tpsIndex].L != null)
                        lambdaAverageTable[tpsIndex].L += pair.Value[tpsIndex].L / openedLambdaDictionary.Count;

                    // field M
                    if (pair.Value[tpsIndex].M != null && lambdaAverageTable[tpsIndex].M == null)
                        lambdaAverageTable[tpsIndex].M = 0.0;
                    if (pair.Value[tpsIndex].M != null)
                        lambdaAverageTable[tpsIndex].M += pair.Value[tpsIndex].M / openedLambdaDictionary.Count;

                    // field N
                    if (pair.Value[tpsIndex].N != null && lambdaAverageTable[tpsIndex].N == null)
                        lambdaAverageTable[tpsIndex].N = 0.0;
                    if (pair.Value[tpsIndex].N != null)
                        lambdaAverageTable[tpsIndex].N += pair.Value[tpsIndex].N / openedLambdaDictionary.Count;

                    // field O
                    if (pair.Value[tpsIndex].O != null && lambdaAverageTable[tpsIndex].O == null)
                        lambdaAverageTable[tpsIndex].O = 0.0;
                    if (pair.Value[tpsIndex].O != null)
                        lambdaAverageTable[tpsIndex].O += pair.Value[tpsIndex].O / openedLambdaDictionary.Count;

                    // field P
                    if (pair.Value[tpsIndex].P != null && lambdaAverageTable[tpsIndex].P == null)
                        lambdaAverageTable[tpsIndex].P = 0.0;
                    if (pair.Value[tpsIndex].P != null)
                        lambdaAverageTable[tpsIndex].P += pair.Value[tpsIndex].P / openedLambdaDictionary.Count;

                    // field Q
                    if (pair.Value[tpsIndex].Q != null && lambdaAverageTable[tpsIndex].Q == null)
                        lambdaAverageTable[tpsIndex].Q = 0.0;
                    if (pair.Value[tpsIndex].Q != null)
                        lambdaAverageTable[tpsIndex].Q += pair.Value[tpsIndex].Q / openedLambdaDictionary.Count;

                    // field R
                    if (pair.Value[tpsIndex].R != null && lambdaAverageTable[tpsIndex].R == null)
                        lambdaAverageTable[tpsIndex].R = 0.0;
                    if (pair.Value[tpsIndex].R != null)
                        lambdaAverageTable[tpsIndex].R += pair.Value[tpsIndex].R / openedLambdaDictionary.Count;

                    // field S
                    if (pair.Value[tpsIndex].S != null && lambdaAverageTable[tpsIndex].S == null)
                        lambdaAverageTable[tpsIndex].S = 0.0;
                    if (pair.Value[tpsIndex].S != null)
                        lambdaAverageTable[tpsIndex].S += pair.Value[tpsIndex].S / openedLambdaDictionary.Count;

                    // field T
                    if (pair.Value[tpsIndex].T != null && lambdaAverageTable[tpsIndex].T == null)
                        lambdaAverageTable[tpsIndex].T = 0.0;
                    if (pair.Value[tpsIndex].T != null)
                        lambdaAverageTable[tpsIndex].T += pair.Value[tpsIndex].T / openedLambdaDictionary.Count;

                    // field U
                    if (pair.Value[tpsIndex].U != null && lambdaAverageTable[tpsIndex].U == null)
                        lambdaAverageTable[tpsIndex].U = 0.0;
                    if (pair.Value[tpsIndex].U != null)
                        lambdaAverageTable[tpsIndex].U += pair.Value[tpsIndex].U / openedLambdaDictionary.Count;

                    // field V
                    if (pair.Value[tpsIndex].V != null && lambdaAverageTable[tpsIndex].V == null)
                        lambdaAverageTable[tpsIndex].V = 0.0;
                    if (pair.Value[tpsIndex].V != null)
                        lambdaAverageTable[tpsIndex].V += pair.Value[tpsIndex].V / openedLambdaDictionary.Count;

                    // field W
                    if (pair.Value[tpsIndex].W != null && lambdaAverageTable[tpsIndex].W == null)
                        lambdaAverageTable[tpsIndex].W = 0.0;
                    if (pair.Value[tpsIndex].W != null)
                        lambdaAverageTable[tpsIndex].W += pair.Value[tpsIndex].W / openedLambdaDictionary.Count;

                    // field X
                    if (pair.Value[tpsIndex].X != null && lambdaAverageTable[tpsIndex].X == null)
                        lambdaAverageTable[tpsIndex].X = 0.0;
                    if (pair.Value[tpsIndex].X != null)
                        lambdaAverageTable[tpsIndex].X += pair.Value[tpsIndex].X / openedLambdaDictionary.Count;

                    // field Y
                    if (pair.Value[tpsIndex].Y != null && lambdaAverageTable[tpsIndex].Y == null)
                        lambdaAverageTable[tpsIndex].Y = 0.0;
                    if (pair.Value[tpsIndex].Y != null)
                        lambdaAverageTable[tpsIndex].Y += pair.Value[tpsIndex].Y / openedLambdaDictionary.Count;

                    // field Z
                    if (pair.Value[tpsIndex].Z != null && lambdaAverageTable[tpsIndex].Z == null)
                        lambdaAverageTable[tpsIndex].Z = 0.0;
                    if (pair.Value[tpsIndex].Z != null)
                        lambdaAverageTable[tpsIndex].Z += pair.Value[tpsIndex].Z / openedLambdaDictionary.Count;
                }
            }

            for (int y = 1; y <= 26; y++)
            {
                // if the value is null, return a double? with a null value, otherwise return the same value but rounded to 3 decimal places
                lambdaAverageTable[27 - y].B = !lambdaAverageTable[27 - y].B.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].B, 3);
                lambdaAverageTable[27 - y].C = !lambdaAverageTable[27 - y].C.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].C, 3);
                lambdaAverageTable[27 - y].D = !lambdaAverageTable[27 - y].D.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].D, 3);
                lambdaAverageTable[27 - y].E = !lambdaAverageTable[27 - y].E.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].E, 3);
                lambdaAverageTable[27 - y].F = !lambdaAverageTable[27 - y].F.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].F, 3);
                lambdaAverageTable[27 - y].G = !lambdaAverageTable[27 - y].G.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].G, 3);
                lambdaAverageTable[27 - y].H = !lambdaAverageTable[27 - y].H.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].H, 3);
                lambdaAverageTable[27 - y].I = !lambdaAverageTable[27 - y].I.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].I, 3);
                lambdaAverageTable[27 - y].J = !lambdaAverageTable[27 - y].J.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].J, 3);
                lambdaAverageTable[27 - y].K = !lambdaAverageTable[27 - y].K.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].K, 3);
                lambdaAverageTable[27 - y].L = !lambdaAverageTable[27 - y].L.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].L, 3);
                lambdaAverageTable[27 - y].M = !lambdaAverageTable[27 - y].M.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].M, 3);
                lambdaAverageTable[27 - y].N = !lambdaAverageTable[27 - y].N.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].N, 3);
                lambdaAverageTable[27 - y].O = !lambdaAverageTable[27 - y].O.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].O, 3);
                lambdaAverageTable[27 - y].P = !lambdaAverageTable[27 - y].P.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].P, 3);
                lambdaAverageTable[27 - y].Q = !lambdaAverageTable[27 - y].Q.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].Q, 3);
                lambdaAverageTable[27 - y].R = !lambdaAverageTable[27 - y].R.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].R, 3);
                lambdaAverageTable[27 - y].S = !lambdaAverageTable[27 - y].S.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].S, 3);
                lambdaAverageTable[27 - y].T = !lambdaAverageTable[27 - y].T.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].T, 3);
                lambdaAverageTable[27 - y].U = !lambdaAverageTable[27 - y].U.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].U, 3);
                lambdaAverageTable[27 - y].V = !lambdaAverageTable[27 - y].V.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].V, 3);
                lambdaAverageTable[27 - y].W = !lambdaAverageTable[27 - y].W.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].W, 3);
                lambdaAverageTable[27 - y].X = !lambdaAverageTable[27 - y].X.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].X, 3);
                lambdaAverageTable[27 - y].Y = !lambdaAverageTable[27 - y].Y.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].Y, 3);
                lambdaAverageTable[27 - y].Z = !lambdaAverageTable[27 - y].Z.HasValue ? (double?)null : Math.Round((double)lambdaAverageTable[27 - y].Z, 3);
            }
        }

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
            openedLambdaDictionary = new Dictionary<string, ObservableCollection<GridRowObject>>();
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

        private void ProcessSuggestedMAP()
        {
            //start with a clean slate
            suggestedTuneTable = new ObservableCollection<GridRowObject>(NewCopy(emptyTable));
            double desired_lambda;
            if (double.TryParse(desiredLambda.Text, out desired_lambda))
            {
                for (int y = 1; y <= 26; y++)
                {
                    // if the value is null, return a double? with a null value, otherwise return the same value but rounded to 3 decimal places
                    suggestedTuneTable[27 - y].B = !lambdaAverageTable[27 - y].B.HasValue ? baseTuneTable[27 - y].B : Math.Round((double)(lambdaAverageTable[27 - y].B * baseTuneTable[27 - y].B / desired_lambda), 2);
                    suggestedTuneTable[27 - y].C = !lambdaAverageTable[27 - y].C.HasValue ? baseTuneTable[27 - y].C : Math.Round((double)(lambdaAverageTable[27 - y].C * baseTuneTable[27 - y].C / desired_lambda), 2);
                    suggestedTuneTable[27 - y].D = !lambdaAverageTable[27 - y].D.HasValue ? baseTuneTable[27 - y].D : Math.Round((double)(lambdaAverageTable[27 - y].D * baseTuneTable[27 - y].D / desired_lambda), 2);
                    suggestedTuneTable[27 - y].E = !lambdaAverageTable[27 - y].E.HasValue ? baseTuneTable[27 - y].E : Math.Round((double)(lambdaAverageTable[27 - y].E * baseTuneTable[27 - y].E / desired_lambda), 2);
                    suggestedTuneTable[27 - y].F = !lambdaAverageTable[27 - y].F.HasValue ? baseTuneTable[27 - y].F : Math.Round((double)(lambdaAverageTable[27 - y].F * baseTuneTable[27 - y].F / desired_lambda), 2);
                    suggestedTuneTable[27 - y].G = !lambdaAverageTable[27 - y].G.HasValue ? baseTuneTable[27 - y].G : Math.Round((double)(lambdaAverageTable[27 - y].G * baseTuneTable[27 - y].G / desired_lambda), 2);
                    suggestedTuneTable[27 - y].H = !lambdaAverageTable[27 - y].H.HasValue ? baseTuneTable[27 - y].H : Math.Round((double)(lambdaAverageTable[27 - y].H * baseTuneTable[27 - y].H / desired_lambda), 2);
                    suggestedTuneTable[27 - y].I = !lambdaAverageTable[27 - y].I.HasValue ? baseTuneTable[27 - y].I : Math.Round((double)(lambdaAverageTable[27 - y].I * baseTuneTable[27 - y].I / desired_lambda), 2);
                    suggestedTuneTable[27 - y].J = !lambdaAverageTable[27 - y].J.HasValue ? baseTuneTable[27 - y].J : Math.Round((double)(lambdaAverageTable[27 - y].J * baseTuneTable[27 - y].J / desired_lambda), 2);
                    suggestedTuneTable[27 - y].K = !lambdaAverageTable[27 - y].K.HasValue ? baseTuneTable[27 - y].K : Math.Round((double)(lambdaAverageTable[27 - y].K * baseTuneTable[27 - y].K / desired_lambda), 2);
                    suggestedTuneTable[27 - y].L = !lambdaAverageTable[27 - y].L.HasValue ? baseTuneTable[27 - y].L : Math.Round((double)(lambdaAverageTable[27 - y].L * baseTuneTable[27 - y].L / desired_lambda), 2);
                    suggestedTuneTable[27 - y].M = !lambdaAverageTable[27 - y].M.HasValue ? baseTuneTable[27 - y].M : Math.Round((double)(lambdaAverageTable[27 - y].M * baseTuneTable[27 - y].M / desired_lambda), 2);
                    suggestedTuneTable[27 - y].N = !lambdaAverageTable[27 - y].N.HasValue ? baseTuneTable[27 - y].N : Math.Round((double)(lambdaAverageTable[27 - y].N * baseTuneTable[27 - y].N / desired_lambda), 2);
                    suggestedTuneTable[27 - y].O = !lambdaAverageTable[27 - y].O.HasValue ? baseTuneTable[27 - y].O : Math.Round((double)(lambdaAverageTable[27 - y].O * baseTuneTable[27 - y].O / desired_lambda), 2);
                    suggestedTuneTable[27 - y].P = !lambdaAverageTable[27 - y].P.HasValue ? baseTuneTable[27 - y].P : Math.Round((double)(lambdaAverageTable[27 - y].P * baseTuneTable[27 - y].P / desired_lambda), 2);
                    suggestedTuneTable[27 - y].Q = !lambdaAverageTable[27 - y].Q.HasValue ? baseTuneTable[27 - y].Q : Math.Round((double)(lambdaAverageTable[27 - y].Q * baseTuneTable[27 - y].Q / desired_lambda), 2);
                    suggestedTuneTable[27 - y].R = !lambdaAverageTable[27 - y].R.HasValue ? baseTuneTable[27 - y].R : Math.Round((double)(lambdaAverageTable[27 - y].R * baseTuneTable[27 - y].R / desired_lambda), 2);
                    suggestedTuneTable[27 - y].S = !lambdaAverageTable[27 - y].S.HasValue ? baseTuneTable[27 - y].S : Math.Round((double)(lambdaAverageTable[27 - y].S * baseTuneTable[27 - y].S / desired_lambda), 2);
                    suggestedTuneTable[27 - y].T = !lambdaAverageTable[27 - y].T.HasValue ? baseTuneTable[27 - y].T : Math.Round((double)(lambdaAverageTable[27 - y].T * baseTuneTable[27 - y].T / desired_lambda), 2);
                    suggestedTuneTable[27 - y].U = !lambdaAverageTable[27 - y].U.HasValue ? baseTuneTable[27 - y].U : Math.Round((double)(lambdaAverageTable[27 - y].U * baseTuneTable[27 - y].U / desired_lambda), 2);
                    suggestedTuneTable[27 - y].V = !lambdaAverageTable[27 - y].V.HasValue ? baseTuneTable[27 - y].V : Math.Round((double)(lambdaAverageTable[27 - y].V * baseTuneTable[27 - y].V / desired_lambda), 2);
                    suggestedTuneTable[27 - y].W = !lambdaAverageTable[27 - y].W.HasValue ? baseTuneTable[27 - y].W : Math.Round((double)(lambdaAverageTable[27 - y].W * baseTuneTable[27 - y].W / desired_lambda), 2);
                    suggestedTuneTable[27 - y].X = !lambdaAverageTable[27 - y].X.HasValue ? baseTuneTable[27 - y].X : Math.Round((double)(lambdaAverageTable[27 - y].X * baseTuneTable[27 - y].X / desired_lambda), 2);
                    suggestedTuneTable[27 - y].Y = !lambdaAverageTable[27 - y].Y.HasValue ? baseTuneTable[27 - y].Y : Math.Round((double)(lambdaAverageTable[27 - y].Y * baseTuneTable[27 - y].Y / desired_lambda), 2);
                    suggestedTuneTable[27 - y].Z = !lambdaAverageTable[27 - y].Z.HasValue ? baseTuneTable[27 - y].Z : Math.Round((double)(lambdaAverageTable[27 - y].Z * baseTuneTable[27 - y].Z / desired_lambda), 2);

                    suggestedTuneTable[27 - y].BColor = lambdaAverageTable[27 - y].B.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].CColor = lambdaAverageTable[27 - y].C.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].DColor = lambdaAverageTable[27 - y].D.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].EColor = lambdaAverageTable[27 - y].E.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].FColor = lambdaAverageTable[27 - y].F.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].GColor = lambdaAverageTable[27 - y].G.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].HColor = lambdaAverageTable[27 - y].H.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].IColor = lambdaAverageTable[27 - y].I.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].JColor = lambdaAverageTable[27 - y].J.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].KColor = lambdaAverageTable[27 - y].K.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].LColor = lambdaAverageTable[27 - y].L.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].MColor = lambdaAverageTable[27 - y].M.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].NColor = lambdaAverageTable[27 - y].N.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].OColor = lambdaAverageTable[27 - y].O.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].PColor = lambdaAverageTable[27 - y].P.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].QColor = lambdaAverageTable[27 - y].Q.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].RColor = lambdaAverageTable[27 - y].R.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].SColor = lambdaAverageTable[27 - y].S.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].TColor = lambdaAverageTable[27 - y].T.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].UColor = lambdaAverageTable[27 - y].U.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].VColor = lambdaAverageTable[27 - y].V.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].WColor = lambdaAverageTable[27 - y].W.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].XColor = lambdaAverageTable[27 - y].X.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].YColor = lambdaAverageTable[27 - y].Y.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    suggestedTuneTable[27 - y].ZColor = lambdaAverageTable[27 - y].Z.HasValue ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                MessageBox.Show("The deisred lambda box MUST have a number inside of it");
            }
            
        }

        #endregion
    }
}
