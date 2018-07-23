using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.Arrays
{
    public static class BinaryExt
    {
        public static double ToBinary(this double[] array)
        {
            double res = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != 0 && array[i] != 1)
                    throw new ArgumentException("All elements in array must be either 1 or 0!");

                if (array[i] == 1)
                    res += Math.Pow(2, i);
            }

            return res;
        }
    }
}
