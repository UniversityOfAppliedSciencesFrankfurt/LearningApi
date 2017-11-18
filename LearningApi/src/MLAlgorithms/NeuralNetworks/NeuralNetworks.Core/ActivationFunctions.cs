using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetworks.Core
{
    public class ActivationFunctions 
    {
        public static double Boolean(double val)
        {
            return (val >= 0) ? 1 : 0;
        }

        public static double Sigmoid(double val)
        {
            return 1 / (1 + Math.Exp(-val));
        }

      
    }
}
