using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    public interface IActivationFunction
    {
        double Derivative(double x);
        double Derivative2(double y);
        double Function(double x);
    }
}
