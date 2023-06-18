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
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Reflection.Emit;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
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
        public int maxIter = 0;
        public double totalError =0.1;
        private const double rectWidth = 80;
        private const double rectHeight = 40;
        private List<ProgramLogic.Point>? points;
        public Neuron2? neuron2;
        public PlotModel PlotModel { get; private set; }
        public int epocCount = 0;
        List<double> neuronErrorValues = new List<double>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddPoints()
        {
            points = new List<ProgramLogic.Point>();
            for (int i =0; i<dataTable.Rows.Count; i++)
            {
                
                DataRow row = dataTable.Rows[i];
                double[] data = row.ItemArray.Select(value => Convert.ToDouble(value)).ToArray();


                points.Add(new ProgramLogic.Point(data[0], data[1], data[2]));
            }
            UpdatePlotModel();
            
        }
        private void UpdatePlotModel()
        {
            PlotModel = new PlotModel { Title = "Wizualizacja prostej oraz punktów"};


            var positivePoints = points.Where(p => p.Label == 1);
            var negativePoints = points.Where(p => p.Label == -1);
            var positiveScatterSeries = new ScatterSeries()
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                MarkerFill = OxyColors.Blue
            };

            var negativeScatterSeries = new ScatterSeries()
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                MarkerFill = OxyColors.Red
            };

            foreach (var point in positivePoints)
            {
                positiveScatterSeries.Points.Add(new ScatterPoint(point.X, point.Y));
            }

            foreach (var point in negativePoints)
            {
                negativeScatterSeries.Points.Add(new ScatterPoint(point.X, point.Y));
            }

            PlotModel.Series.Add(positiveScatterSeries);
            PlotModel.Series.Add(negativeScatterSeries);
            plotView.Model = PlotModel;
        }
        private void UpdatePlotLine()
        {
            double[] neuronWeights = neuron2.GetWeigths();
            if (neuronWeights[2] != 0)
            {
                double slope = -(neuronWeights[1] / neuronWeights[2]);
                double intercept = -(neuronWeights[0] / neuronWeights[2]);
                var lineSeries = new LineSeries()
                {
                    Title = "Perceptron",
                    Color = OxyColors.Black,
                    StrokeThickness = 2,
                    DataFieldX = "X",
                    DataFieldY = "Y"
                };
                for (double x = -100; x <= 100; x += 0.1)
                {
                    double y = slope * x - intercept;
                    lineSeries.Points.Add(new DataPoint(x, y));
                }

                PlotModel.Series.Add(lineSeries);

                plotView.Model = PlotModel;

            }
            
            
        }
        private void UpdateErrorPlot(List<double> errorValues)
        {

            var errorPlotModel = new PlotModel {Title = "Wykres błędu uczenia" };
           // var xAxis = new LinearAxis { Title = "Iteracje" };
           // var yAxis = new LinearAxis { Title = "Błąd"  };
            //errorPlotModel.Axes.Add(xAxis);
           // errorPlotModel.Axes.Add(yAxis);

            var lineSeries = new LineSeries { Title = "Błąd", Color = OxyColors.Green, StrokeThickness =2, DataFieldX="iteracje", DataFieldY="błąd"};
            for(int i =0; i<errorValues.Count;i++)
            {
                lineSeries.Points.Add(new DataPoint(i, errorValues[i]));
            }
            errorPlotModel.Series.Add(lineSeries);

            errorPlot.Model = errorPlotModel;
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
                    canvasNeuron.Children.Clear();
                    canvasAdaline.Children.Clear();
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
            neuron2 = new Neuron2(dataTable.Columns.Count);
            neuron2.InitWeigths();
            
            creat_Neuron(dataTable.Columns.Count);
            AddPoints();

        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void creat_Neuron(int n)
        {
            for (int i = 0; i < n; i++)
            {
                Rectangle rectangleNeuron = new Rectangle
                {
                    Width = rectWidth,
                    Height = rectHeight,
                    Fill = Brushes.LightBlue,
                    Stroke = Brushes.Black,
                    Margin = new Thickness(5)
                };
                Rectangle rectangleAdaline = new Rectangle
                {
                    Width = rectWidth,
                    Height = rectHeight,
                    Fill = Brushes.LightBlue,
                    Stroke = Brushes.Black,
                    Margin = new Thickness(5)
                };
                Canvas.SetTop(rectangleNeuron, 20);
                Canvas.SetLeft(rectangleNeuron, i * (rectWidth + 20));
                Canvas.SetTop(rectangleAdaline, 20);
                Canvas.SetLeft(rectangleAdaline, i * (rectWidth + 20));
                canvasNeuron.Children.Add(rectangleNeuron);
                canvasAdaline.Children.Add(rectangleAdaline);
                TextBlock textBlockNeuron = new TextBlock();
                textBlockNeuron.Name = ("Nx" + i.ToString());
                textBlockNeuron.Text = ("Nx" + i.ToString());
                textBlockNeuron.FontSize = 12;
                textBlockNeuron.Foreground = Brushes.Black;
                Canvas.SetTop(textBlockNeuron, 20 + rectangleNeuron.Height / 2);
                Canvas.SetLeft(textBlockNeuron, i * (rectWidth + 20) + rectangleNeuron.Width / 2);
                TextBlock textBlockAdaline = new TextBlock();
                textBlockAdaline.Name = ("Ax" + i.ToString());
                textBlockAdaline.Text = ("Ax" + i.ToString());
                textBlockAdaline.FontSize = 12;
                textBlockAdaline.Foreground = Brushes.Black;
                Canvas.SetTop(textBlockAdaline, 20 + rectangleAdaline.Height / 2);
                Canvas.SetLeft(textBlockAdaline, i * (rectWidth + 20) + rectangleAdaline.Width / 2);
                canvasNeuron.Children.Add(textBlockNeuron);
                canvasAdaline.Children.Add(textBlockAdaline);
            }

            // Tworzenie trójkątów
            for (int i = 0; i < n; i++)
            {

                Polygon triangleNeuron = new Polygon
                {
                    Fill = Brushes.LightGreen,
                    Name = ("w" + i.ToString()),
                    Points = new PointCollection
                    {
                        new System.Windows.Point(0, 0),
                        new System.Windows.Point(rectWidth, 0),
                        new System.Windows.Point(rectWidth / 2, rectHeight)
                    },
                    Stroke = Brushes.Black,
                    Margin = new Thickness(5)
                };
                Polygon triangleAdaline = new Polygon
                {
                    Fill = Brushes.LightGreen,
                    Name = ("w" + i.ToString()),
                    Points = new PointCollection
                    {
                        new System.Windows.Point(0, 0),
                        new System.Windows.Point(rectWidth, 0),
                        new System.Windows.Point(rectWidth / 2, rectHeight)
                    },
                    Stroke = Brushes.Black,
                    Margin = new Thickness(5)
                };
                Canvas.SetTop(triangleNeuron, 80);
                Canvas.SetLeft(triangleNeuron, i * (rectWidth + 20));
                Canvas.SetTop(triangleAdaline, 80);
                Canvas.SetLeft(triangleAdaline, i * (rectWidth + 20));
                canvasNeuron.Children.Add(triangleNeuron);
                canvasAdaline.Children.Add(triangleAdaline);

                TextBlock textBlockNeuron = new TextBlock();
                textBlockNeuron.Name = ("Nw" + i.ToString());
                textBlockNeuron.Text = ("Nw" + i.ToString());
                textBlockNeuron.FontSize = 12;
                textBlockNeuron.Foreground = Brushes.Black;
                Canvas.SetTop(textBlockNeuron, 80 + rectHeight / 2);
                Canvas.SetLeft(textBlockNeuron, i * (rectWidth + 20) + rectWidth / 2);
                TextBlock textBlockAdaline = new TextBlock();
                textBlockAdaline.Name = ("Aw" + i.ToString());
                textBlockAdaline.Text = ("Aw" + i.ToString());
                textBlockAdaline.FontSize = 12;
                textBlockAdaline.Foreground = Brushes.Black;
                Canvas.SetTop(textBlockAdaline, 80 + rectHeight / 2);
                Canvas.SetLeft(textBlockAdaline, i * (rectWidth + 20) + rectWidth / 2);
                canvasNeuron.Children.Add(textBlockNeuron);
                canvasAdaline.Children.Add(textBlockAdaline);
            }
            // Tworzenie koła
            Ellipse circleNeuron = new Ellipse
            {
                Name = "Nsumator",
                Width = rectWidth,
                Height = rectHeight,
                Fill = Brushes.LightSalmon,
                Stroke = Brushes.Black
            };
            Canvas.SetTop(circleNeuron, (30 + rectHeight * 2 + 50));
            Canvas.SetLeft(circleNeuron, (n - 1) * rectWidth / 2 + 20);
            canvasNeuron.Children.Add(circleNeuron);
            Ellipse circleAdaline = new Ellipse
            {
                Name = "Nsumator",
                Width = rectWidth,
                Height = rectHeight,
                Fill = Brushes.LightSalmon,
                Stroke = Brushes.Black
            };
            Canvas.SetTop(circleAdaline, (30 + rectHeight * 2 + 50));
            Canvas.SetLeft(circleAdaline, (n - 1) * rectWidth / 2 + 20) ;
            canvasAdaline.Children.Add(circleAdaline);

            TextBlock textBlocksum = new TextBlock();
            textBlocksum.Name = "Nsum";
            textBlocksum.Text = "Nsum";
            textBlocksum.FontSize = 12;
            textBlocksum.Foreground = Brushes.Black;
            Canvas.SetTop(textBlocksum, (30 + rectHeight * 2 + 50) + rectHeight / 2);
            Canvas.SetLeft(textBlocksum, ((n - 1) * rectWidth / 2) + rectWidth / 2);

            TextBlock textBlocksumAdaline = new TextBlock();
            textBlocksumAdaline.Name = "Asum";
            textBlocksumAdaline.Text = "Asum";
            textBlocksumAdaline.FontSize = 12;
            textBlocksumAdaline.Foreground = Brushes.Black;
            Canvas.SetTop(textBlocksumAdaline, (30 + rectHeight * 2 + 50) + rectHeight / 2);
            Canvas.SetLeft(textBlocksumAdaline, ((n - 1) * rectWidth / 2) + rectWidth / 2);
            canvasNeuron.Children.Add((TextBlock)textBlocksum);
            canvasAdaline.Children.Add((TextBlock)textBlocksumAdaline);

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
            textBlockout.Name = "Nout";
            textBlockout.Text = "Nout";
            textBlockout.FontSize = 12;
            textBlockout.Foreground = Brushes.Black;
            Canvas.SetTop(textBlockout, rectHeight * 4 + 60 + rectHeight / 2);
            Canvas.SetLeft(textBlockout, ((n - 1) * rectWidth / 2) + rectWidth / 2);

            canvasNeuron.Children.Add(textBlockout);

            Rectangle outputSquareAdaline = new Rectangle
            {
                Name = "output",
                Width = rectWidth,
                Height = rectHeight,
                Fill = Brushes.Orange,
                Stroke = Brushes.Black,
            };
            Canvas.SetTop(outputSquareAdaline, ((30 + rectHeight * 4 + 50)));
            Canvas.SetLeft(outputSquareAdaline, (n - 1) * rectWidth / 2 + 20);
            canvasAdaline.Children.Add(outputSquareAdaline);

            TextBlock textBlockoutAdaline = new TextBlock();
            textBlockoutAdaline.Name = "Aout";
            textBlockoutAdaline.Text = "Aout";
            textBlockoutAdaline.FontSize = 12;
            textBlockoutAdaline.Foreground = Brushes.Black;
            Canvas.SetTop(textBlockoutAdaline, rectHeight * 4 + 60 + rectHeight / 2);
            Canvas.SetLeft(textBlockoutAdaline, ((n - 1) * rectWidth / 2) + rectWidth / 2);

            canvasAdaline.Children.Add(textBlockoutAdaline);
        }
        private void saveData_Click(object sender, RoutedEventArgs e)
        {

        }
        private TextBlock FindTextBlockInCanvas(Canvas canvas, string textBlockName)
        {
            foreach (var child in canvas.Children)
            {
                if (child is TextBlock textBlock && textBlock.Name == textBlockName)
                {
                    return textBlock;
                }
                else if (child is Canvas childCanvas)
                {
                    var foundTextBlock = FindTextBlockInCanvas(childCanvas, textBlockName);
                    if (foundTextBlock != null)
                    {
                        return foundTextBlock;
                    }
                }
            }
            return null;
        }
        public async Task UpdateWeightsCanvas(double[] array, double[] neuronWeights)
        {
            

            for (int i=0;i<dataTable.Columns.Count;i++)
            {
                TextBlock founded2 = FindTextBlockInCanvas(canvasNeuron, ("Nx" + i.ToString()));
               if(founded2!=null)
                {
                    founded2.Text = array[i].ToString();
                }
                
            }
            
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                TextBlock founded = FindTextBlockInCanvas(canvasNeuron, ("Nw" + i.ToString()));

                if (founded != null)
                {
                    founded.Text = neuron2.weights[i].ToString();
                }
                
            }
            await Task.Delay(200);
            
        }
        public async Task UpdateOtherCanvas(double sum, double output)
        {
            TextBlock sumN = FindTextBlockInCanvas(canvasNeuron, ("Nsum"));
            if (sumN != null)
            {
                sumN.Text = sum.ToString();
            }
            //await Task.Delay(500);
            TextBlock outN = FindTextBlockInCanvas(canvasNeuron, ("Nout"));
            if (outN != null)
            {
                outN.Text = output.ToString();
            }
            await Task.Delay(200);
        }
        public async Task<double> Training(int iterations)
        {
            if(dataTable.Rows.Count <=1)
            {
                throw new Exception("table is empty");
            }
            if(iterations <=0)
            {
                throw new ArgumentException("maxymalna liczba iteraci musi być większ od 0");
            }
            double meanError = 0;
            for (int i = 0; i < iterations; i++)
            {
                //epocCount++;
                double sumError = 0;
                for (int j = 0;j<dataTable.Rows.Count;j++)
                {
                   
                    DataRow row = dataTable.Rows[j];
                    double[] doubleArray = row.ItemArray
                        .Select(value => Convert.ToDouble(value))
                        .ToArray();
                    double d = doubleArray[dataTable.Columns.Count - 1];
                    Array.Resize(ref doubleArray, doubleArray.Length-1);
                    List<double> list = new List<double>(doubleArray);
                    list.Insert(0, 1);
                    double[] actualWeights = neuron2.weights;
                    
                    doubleArray = list.ToArray();
                    await UpdateWeightsCanvas(doubleArray, actualWeights);
                    sumError += neuron2.TeachOneIteration( doubleArray, d);

                    await UpdateOtherCanvas(neuron2.sum,neuron2.output);
                    // MessageBox.Show("epoka =" + i.ToString() + " datarow = " + j.ToString() + "\n WAGI: " + string.Join(", ", neuron2.weights) + " DANE: " + string.Join(", ", doubleArray) + "\n");
                }
                meanError = sumError / dataTable.Rows.Count;
                neuronErrorValues.Add(meanError);
                
                //MessageBox.Show("meanError:" + meanError.ToString());
                if (meanError < totalError || epocCount >= maxIter )
                {
                    double[] koncoweWartosci = { 0, 0, 0 };
                    UpdateWeightsCanvas(koncoweWartosci, neuron2.GetWeigths());
                    UpdateOtherCanvas(0, 0);
                    MessageBox.Show("goal reached after:" + epocCount.ToString() + "\n Neuron weights: " + string.Join("\n", neuron2.weights) + "\n Mean Error =" + meanError.ToString());
                    UpdateErrorPlot(neuronErrorValues);
                    UpdatePlotLine();

                    return meanError;
                }
                epocCount++;

            }
            return meanError;
        }




        private void sLR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void btStep_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Training(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btAuto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("max iter " + maxIter.ToString());
                Training(maxIter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btSetMaxIter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                maxIter = int.Parse(tbMI.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void sTE_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            totalError = sTE.Value;
            
        }
    }
}
