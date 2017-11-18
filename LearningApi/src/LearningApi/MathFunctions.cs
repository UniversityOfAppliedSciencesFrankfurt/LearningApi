using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation
{
    /// <summary>
    /// Collection of useful math functions.
    /// </summary>
    public class MathFunctions
    {
        /// <summary>
        /// Random generator used in static session context.
        /// </summary>
        private static Random m_Rnd = new Random();

        /// <summary>
        /// Calculates outter product of two vectors.
        /// vector1: x11,x1,x13,..,x1N
        /// vector1: x11,x1,x13,..,x1N
        /// z[N,M]=V1*V2
        /// </summary>
        /// <param name="vector1">x1,x2,x3,..,x1N</param>
        /// <param name="vector2">y1,y2,x3,..,y1M</param>
        /// <returns>
        /// row1 = x1 * y1, x1 * y2,.., x1 * yM 
        /// row2 = x2 * y1, x2 * y2,.., x2 * yM 
        /// rowN = xN * y1, xN * y2,.., xN * yM 
        /// </returns>
        public static double[][] OuterProd(double[] vector1, double[] vector2)
        {
            int rows = vector1.Length;
            int cols = vector2.Length;
            double[][] result = new double[rows][];

            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result[i][j] = vector1[i] * vector2[j];

            return result;
        }
        

        /// <summary>
        /// It shuffless elements of array randomly.
        /// </summary>
        /// <param name="elements">Elements of an array.</param>
        /// <param name="rnd">Random gfenerator can be provided from outside.</param>
        public static void Shuffle(int[] elements, Random rnd = null)
        {
            if (rnd == null)
                rnd = m_Rnd;

            for (int i = 0; i < elements.Length; ++i)
            {
                int ri = m_Rnd.Next(i, elements.Length);
                int tmp = elements[i];
                elements[i] = elements[ri];
                elements[ri] = tmp;
            }
        }
    }
}
