using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.Math
{
    public interface IRandomNumberGenerator<T>
    {
        T[] Generate(int samples);

        T[] Generate(int samples, T[] result);

        T Generate();
    }

}
