using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetworks.Core.ActivationFunctions
{
    public class BooleanActivationFunction : IActivationFunction
    {
        public double Function(double x)
        {
            return (x >= 0) ? 1 : 0;
        }
    }    
}
