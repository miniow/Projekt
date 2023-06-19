using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ProgramLogic
{
    public class Neuron2
    {
        public int numberOfInputs;
        public double[] weights;
        public double actualError;
        public double[] weightsBefore;
        public double sum;
        public double output;


        public Neuron2(int inputNo) {
            this.numberOfInputs = inputNo;
            weights = new double[numberOfInputs];
            for (int i = 0; i < numberOfInputs; i++)
            {
                weights[i] = 0;
            }
        }
        public void InitWeigths()
        {
            Random random = new Random();
            for (int i = 0; i<numberOfInputs; i++) 
            {
                weights[i] = random.NextDouble()*2-1;
            }
        }
        public double[] GetWeigths()
        {
            return weights;
        }
        public double CalculateOutput(double[] inputs)
        {
            
            if(inputs.Length != numberOfInputs)
            {
                throw new ArgumentException("incorect number of inputs");
            }
            double localSum = 0;
            for (int i = 0; i<numberOfInputs; i++)
            {
                localSum+= weights[i] * inputs[i];
            }
            return localSum;
        }
        public double TeachOneIteration(double[] x, double d)
        {
            double s = CalculateOutput(x);
            double y = ActFunction(s);
            double error = d - y;
            if (Math.Abs(error)<double.Epsilon)
            { 
            }
            else
            {
                for(int i =0; i<numberOfInputs; i++)
                {
                    weights[i] = weights[i] + x[i]*d;
                }
                
            }
            sum = s;
            output = y;
            return Math.Abs(error);
        }

        public double ActFunction(double num)
        {
            if (num > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
                
        }

    }

    public class Adaline2
    {
        public int numberOfInputs;
        public double[] weights;
        public double learningRate;

        public double actualError;
        public double sum;
        public double output;

        public Adaline2(int num, double ratio=0.1)
        {
            numberOfInputs = num;
            this.weights = new double[num];
            this.learningRate = ratio;
        }

        public void InitWeigths()
        {
            Random random = new Random();
            for (int i = 0; i < numberOfInputs; i++)
            {
                weights[i] = random.NextDouble() * 2 - 1;
            }
        }

        public double CalculateOutput(double[] inputs)
        {
            if (inputs.Length != numberOfInputs)
            {
                throw new ArgumentException("incorect number of inputs");
            }
            double localSum = 0;
            for (int i = 0; i < numberOfInputs; i++)
            {
                localSum += weights[i] * inputs[i];
            }
            output = ActFunction(localSum);
            return output;
        }
        public double TeachOneIteration(double[] x, double d)
        {
            double s = 0;
            for(int i=0; i<numberOfInputs;i++)
            {
                s += weights[i] * x[i];
            }
            for (int i = 0; i < numberOfInputs; i++)
            {
                weights[i] = weights[i] + learningRate * (d - s) * x[i];
            }
            double y = ActFunction(s);
            double error = d - y;
            return Math.Abs(error);
        }
        public double ActFunction(double num)
        {
            if (num > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }
    }
}
