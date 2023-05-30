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
using System.Data;

namespace Projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    


    public partial class MainWindow : Window
    {
        DataTable dataTable = new DataTable();
        public MainWindow()
        {
            InitializeComponent();

        }

        private void loadData_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count>0)
            {
                MessageBoxResult result = MessageBox.Show("Wczytanie nowych danych spowoduje usunięcie obecnych. Czy chcesz kontynuować?", "Ostrzeżenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    dataTable.Rows.Clear();
                    dataTable.Columns.Clear();
                }
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    if (lines.Length > 0)
                    {
                        string firstLine = lines[0];
                        string[] columnNames = firstLine.Split('\t');

                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            dataTable.Columns.Add(columnNames[i], typeof(double));
                        }

                        // Dodawanie danych do siatki
                        for (int i = 1; i < lines.Length; i++)
                        {
                            string[] values = lines[i].Split('\t');
                            dataTable.Rows.Add(values);
                        }
                    }
                    dataGrid.DataContext = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void saveData_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
