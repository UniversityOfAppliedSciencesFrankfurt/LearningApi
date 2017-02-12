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
    }
}
