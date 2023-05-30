using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ProgramLogic
{
    public class Neuron
    {
        protected int numberOfInputs;
        protected double[] weights;
        public Neuron(int number)
        {
            this.numberOfInputs = number;
            weights = new double[numberOfInputs];
            for (int i = 0; i < numberOfInputs; i++)
            {
                weights[i] = 0;
            }
        }
        public void InitWeights()
        {
            Random random = new Random();
            for (int i = 0; i < numberOfInputs; i++)
            {
                weights[i] = random.NextDouble() * 2 - 1;
            }
        }
        public double[] GetWeights()
        {
            return weights;
        }
        public double CalculateOutput(double[] inputs)
        {
            if (inputs.Length != numberOfInputs)
            {
                throw new ArgumentException("incorect number of inputs");
            }

            double sum = 0;
            for (int i = 1; i < numberOfInputs; i++)
            {
                sum += weights[i] * inputs[i] + weights[0];
            }
            return sum;
        }
    }
    public class Perceptron : Neuron
    {
        public Perceptron(int number) : base(number) { }

        public void Train(double[][] learninigExapmles, double[] decisionArg, double totalError)
        {
            InitWeights();
            double meanError;
            do
            {
                double sumError = 0;
                for (int i = 0; i < learninigExapmles.Length; i++)
                {

                    double y, d, error;
                    d = decisionArg[i];
                    y = ActFunc(CalculateOutput(learninigExapmles[i]));
                    error = d - y;
                    Console.WriteLine(i.ToString());
                    Console.WriteLine(string.Join(", ", weights));

                    if (y == d)
                    {
                        continue;
                    }
                    else
                    {
                        for (int j = 0; j < numberOfInputs; j++)
                        {
                            weights[j] = weights[j] + learninigExapmles[i][j] * d;
                        }
                    }
                    sumError += Math.Abs(error);
                }
                meanError = sumError / learninigExapmles.Length;
            } while (meanError > totalError);
            ;
        }
        private double ActFunc(double num)
        {
            if (num > 0)
                return 1;
            else
                return -1;
        }

    }
    public class Adaline : Neuron
    {
        public Adaline(int number) : base(number) { }

        public void Train(double[][] learninigExapmles, double[] decisionArg, double totalError)
        {
            InitWeights();
            Console.WriteLine("podaj wspołczynnik uczenia (0,1)");
            double n = Convert.ToDouble(Console.ReadLine);
            double meanError = 0;
            do
            {
                double sumError = 0;
                for (int i = 0; i < learninigExapmles.Length; i++)
                {
                    double y, s, d, error;
                    d = decisionArg[i];
                    s = CalculateOutput(learninigExapmles[i]);
                    y = ActFunc(s);
                    error = d - y;
                    for (int j = 0; j < numberOfInputs; j++)
                    {
                        weights[j] = weights[j] + n * (d - s) * learninigExapmles[i][j];
                    }
                    sumError += Math.Abs(error);
                }
                meanError = sumError / learninigExapmles.Length;
            } while (meanError < totalError);
        }
        private double ActFunc(double num)
        {
            if (num > 0)
                return 1;
            else
                return 0;
        }
    }
}
