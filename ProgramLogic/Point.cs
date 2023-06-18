using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ProgramLogic
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Label { get; set; }

        public Point(double x, double y, double label)
        {
            X = x;
            Y = y;
            Label = label;
        }
    }
}
