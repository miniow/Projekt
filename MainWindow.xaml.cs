using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.IO;
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
using System.ComponentModel;

namespace Projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    


    public partial class MainWindow : Window
    {
        public double[,] trainData;
        public double[,] validData;
        public string[] columnNames;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void loadData_Click(object sender, RoutedEventArgs e)
        {
            if (true)
            {
                MessageBoxResult result = MessageBox.Show("Wczytanie nowych danych spowoduje usunięcie obecnych. Czy chcesz kontynuować?", "Ostrzeżenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {

                }
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] lines = File.ReadAllLines(filePath);
                    if (lines.Length > 0)
                    {
                        string firstLine = lines[0];
                        string[] columnNames = firstLine.Split('\t');

                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            DataGridTextColumn column = new DataGridTextColumn();
                            column.Header = columnNames[i];
                            column.Binding = new System.Windows.Data.Binding("[" + i + "]");
                            dataGrid.Columns.Add(column);
                        }

                        // Dodawanie danych do siatki
                        for (int i = 1; i < lines.Length; i++)
                        {
                            string[] values = lines[i].Split('\t');
                            List<double> rowData = new List<double>();
                            foreach (string value in values)
                            {
                                double parsedValue;
                                if (double.TryParse(value, out parsedValue))
                                {
                                    rowData.Add(parsedValue);
                                }
                            }
                            dataGrid.Items.Add(rowData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas otwierania pliku: " + ex.Message);
                }
            }

        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
