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
        public double[] weights;
        public double actualMeanError =0;
        public Neuron(int number)
        {
            this.numberOfInputs = number;
            weights = new double[numberOfInputs];
            for (int i = 0; i < numberOfInputs; i++)
            {
                weights[i] = 0;
            }
            InitWeights();
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

            double sum = weights[0];
            for (int i = 1; i < numberOfInputs; i++)
            {
                sum += weights[i+1] * inputs[i];
            }
            return sum;
        }
    }
    public class Perceptron : Neuron
    {
        public Perceptron(int number) : base(number) { }
        public double actualError = 0;
        public void Train(double[] learninigExapmles, double totalError, int maxIter)
        {
            int iter = 0;
            double meanError;
            do
            {
                double sumError = 0;
                for (int i = 0; i < learninigExapmles.Length - 1; i++)
                {

                    double y, d, error;
                    d = learninigExapmles[learninigExapmles.Length - 1];
                    y = ActFunc(CalculateOutput(learninigExapmles));
                    error = d - y;

                    if (y == d)
                    {
                        continue;
                    }
                    else
                    {
                        for (int j = 0; j < numberOfInputs; j++)
                        {
                            //  weights[j] = weights[j] + learninigExapmles[i][j] * d;
                        }
                    }
                    sumError += Math.Abs(error);
                }
                meanError = sumError / learninigExapmles.Length;
                iter++;
            } while (meanError > totalError || iter >= maxIter);
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
        double n;
        public Adaline(int number, double num2) : base(number) { this.n = num2;  }

        public void Train(double[][] learninigExapmles, double[] decisionArg, double totalError)
        {
           
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
