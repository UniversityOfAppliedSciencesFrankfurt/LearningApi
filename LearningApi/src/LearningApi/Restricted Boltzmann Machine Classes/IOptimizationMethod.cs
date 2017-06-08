using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation
{
    public interface IOptimizationMethod : IOptimizationMethod<double[], double>
    {
    }

    public interface IOptimizationMethod<TCode> : IOptimizationMethod, IOptimizationMethod<double[], double, TCode>
        where TCode : struct
    {
    }

    public interface IOptimizationMethod<TInput, TOutput>
    {
        int NumberOfVariables { get; set; }

        TInput Solution { get; set; }

        TOutput Value { get; }

        bool Minimize();

        bool Maximize();

    }

    public interface IOptimizationMethod<TInput, TOutput, TCode> : IOptimizationMethod<TInput, TOutput>
        where TCode : struct
    {
        TCode Status { get; }
    }
}
