using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Scott_sMapTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class GridRowObject
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
        }

        public MainWindow()
        {
            InitializeComponent();

            //generate empty grid for data table
            ObservableCollection<GridRowObject> table = new ObservableCollection<GridRowObject>();
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
            this.dataGrid1.ItemsSource = table;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
