using Microsoft.Win32;
using Projekt.ProgramLogic;
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
        public int lastIndex = 0;
        public double learningRate;
        public int maxIter;
        public double totalError;
        private const double rectWidth = 80;
        private const double rectHeight = 40;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void loadData_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count > 0)
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
            creat_Neuron(dataTable.Columns.Count);


        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void creat_Neuron(int n)
        {
            for (int i = 0; i < n; i++)
            {
                Rectangle rectangle = new Rectangle
                {
                    Width = rectWidth,
                    Height = rectHeight,
                    Fill = Brushes.LightBlue,
                    Stroke = Brushes.Black,
                    Margin = new Thickness(5)
                };
                Canvas.SetTop(rectangle, 20);
                Canvas.SetLeft(rectangle, i * (rectWidth + 20));
                canvasNeuron.Children.Add(rectangle);
                TextBlock textBlock = new TextBlock();
                textBlock.Name = ("x" + i.ToString());
                textBlock.Text = ("x" + i.ToString());
                textBlock.FontSize = 12;
                textBlock.Foreground = Brushes.Black;
                Canvas.SetTop(textBlock, 20 + rectangle.Height / 2);
                Canvas.SetLeft(textBlock, i * (rectWidth + 20) + rectangle.Width / 2);
                canvasNeuron.Children.Add(textBlock);
            }

            // Tworzenie trójkątów
            for (int i = 0; i < n; i++)
            {

                Polygon triangle = new Polygon
                {
                    Fill = Brushes.LightGreen,
                    Name = ("w" + i.ToString()),
                    Points = new PointCollection
                    {
                        new Point(0, 0),
                        new Point(rectWidth, 0),
                        new Point(rectWidth / 2, rectHeight)
                    },
                    Stroke = Brushes.Black,
                    Margin = new Thickness(5)
                };
                Canvas.SetTop(triangle, 80);
                Canvas.SetLeft(triangle, i * (rectWidth + 20));
                canvasNeuron.Children.Add(triangle);

                TextBlock textBlock = new TextBlock();
                textBlock.Name = ("w" + i.ToString());
                textBlock.Text = ("w" + i.ToString());
                textBlock.FontSize = 12;
                textBlock.Foreground = Brushes.Black;
                Canvas.SetTop(textBlock, 80 + rectHeight / 2);
                Canvas.SetLeft(textBlock, i * (rectWidth + 20) + rectWidth / 2);
                canvasNeuron.Children.Add(textBlock);
            }
            // Tworzenie koła
            Ellipse circle = new Ellipse
            {
                Name = "sumator",
                Width = rectWidth,
                Height = rectHeight,
                Fill = Brushes.LightSalmon,
                Stroke = Brushes.Black
            };
            Canvas.SetTop(circle, (30 + rectHeight * 2 + 50));
            Canvas.SetLeft(circle, (n - 1) * rectWidth / 2 + 20);
            canvasNeuron.Children.Add(circle);
            TextBlock textBlocksum = new TextBlock();
            textBlocksum.Name = "sum";
            textBlocksum.Text = "sum";
            textBlocksum.FontSize = 12;
            textBlocksum.Foreground = Brushes.Black;
            Canvas.SetTop(textBlocksum, (30 + rectHeight * 2 + 50) + rectHeight / 2);
            Canvas.SetLeft(textBlocksum, ((n - 1) * rectWidth / 2) + rectWidth / 2);
            canvasNeuron.Children.Add((TextBlock)textBlocksum);

            // Tworzenie kwadratu funkcji wyjścia
            Rectangle outputSquare = new Rectangle
            {
                Name = "output",
                Width = rectWidth,
                Height = rectHeight,
                Fill = Brushes.Orange,
                Stroke = Brushes.Black,
            };
            Canvas.SetTop(outputSquare, ((30 + rectHeight * 4 + 50)));
            Canvas.SetLeft(outputSquare, (n - 1) * rectWidth / 2 + 20);
            canvasNeuron.Children.Add(outputSquare);
            TextBlock textBlockout = new TextBlock();
            textBlockout.Name = "out";
            textBlockout.Text = "out";
            textBlockout.FontSize = 12;
            textBlockout.Foreground = Brushes.Black;
            Canvas.SetTop(textBlockout, rectHeight * 4 + 60 + rectHeight / 2);
            Canvas.SetLeft(textBlockout, ((n - 1) * rectWidth / 2) + rectWidth / 2);

            canvasNeuron.Children.Add(textBlockout);
        }
        private void saveData_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbtPerceptron_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbtAdaline_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void sLR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
