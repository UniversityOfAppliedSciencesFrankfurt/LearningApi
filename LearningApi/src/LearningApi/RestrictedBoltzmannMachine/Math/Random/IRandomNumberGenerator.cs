using System;
using System.Collections.Generic;


namespace LearningFoundation.MathFunction
{
    public interface IRandomNumberGenerator<T>
    {
        T[] Generate(int samples);

        T[] Generate(int samples, T[] result);

        T Generate();
    }

}
