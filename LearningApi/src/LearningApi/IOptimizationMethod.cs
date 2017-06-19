using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation
{ 
    public interface IOptimizationMethod<TInput, TOutput>
    {
        int NumberOfVariables { get; set; }

        TInput Solution { get; set; }

        TOutput Value { get; }

        bool Minimize();

        bool Maximize();

    }
   
}
