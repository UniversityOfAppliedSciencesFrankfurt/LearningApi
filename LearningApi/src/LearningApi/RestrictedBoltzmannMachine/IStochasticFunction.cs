using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LearningFoundation
{
    public interface IStochasticFunction : IActivationFunction
    {
        double Generate(double x);

        double Generate2(double y);
    }
}
