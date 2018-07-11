using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;



namespace LearningFoundation.Helpers
{
    public static class FunctionGenerator
    {
        /// <summary>
        /// Creates the function.
        /// </summary>
        /// <param name="points">Number of points on the reference axis X.</param>
        /// <param name="numOfDims">Number of dimensions</param>
        /// <param name="delta">Delta interval on reference axis X. 
        /// Example: 0, delta, 2*delta,..,(points-1)*delta</param>
        /// <param name="func">Function to be used to create y from x. </param>
        /// <returns>Array of coordinates of generated function for every dimension.
        /// If 3 dimensions are used, then 
        /// ret[dim1] = {x1,x2,..xN},
        /// ret[dim2] = {y1,y2,..,yN},
        /// ret[dim3] = {z1,z2,..,zN},
        /// where N = points</returns>
        /// First dimension X contains reference points.
        public static List<double[]> CreateFunction(int points, int numOfDims, double delta, Func<double, int, double> func = null)
        {
            List<double[]> rows = new List<double[]>();

            double[] dimXRow = new double[points];
            for (int i = 0; i < points; i++)
            {
                dimXRow[i] = i * delta;
            }

            rows.Add(dimXRow);

            for (int y = 0; y < numOfDims - 1; y++)
            {
                double[] dimCoord = new double[points];

                for (int i = 0; i < points; i++)
                {
                    if (func == null)
                        dimCoord[i] = Math.Sin(i * delta);
                    else
                        dimCoord[i] = func(i * delta, points);
                }

                rows.Add(dimCoord);
            }

            return rows;
        }

        /// <summary>
        /// Creates function, which is similar to the reference function.
        /// </summary>
        /// <param name="referenceFuncData">the reference function</param>
        /// <param name="MinNoiseRangePercentage">minimum percentage of noise range compared to each attribute range</param>
        /// <param name="MaxNoiseRangePercentage">maximum percentage of noise range compared to each attribute range</param>
        /// <returns></returns>
        public static double[][] CreateSimilarFromReferenceFunc(double[][] referenceFuncData,
            int MinNoiseRangePercentage, int MaxNoiseRangePercentage)
        {
            // calculte random noise limits for every dimension instead of the first (referenc one).
            double[][] randomNoiseLimit = noiseLimit(referenceFuncData, MinNoiseRangePercentage, MaxNoiseRangePercentage);

            // initialize for the similar function
            double[][] similarFunction = new double[referenceFuncData.Length][];
            for (int n = 0; n < similarFunction.Length; n++)
            {
                similarFunction[n] = new double[referenceFuncData[0].Length];
            }

            similarFunction[0] = referenceFuncData[0];

            Random rnd = new Random((int)DateTime.Now.Ticks * DateTime.Now.Millisecond);

            for (int dim = 1; dim < referenceFuncData.Length; dim++)
            {
                // add noise on each point of the function
                for (int j = 0; j < referenceFuncData[0].Length; j++)
                {
                    double rndNoise = rnd.NextDouble();
                    var distorsion = rndNoise * (randomNoiseLimit[1][dim] - randomNoiseLimit[0][dim]) + randomNoiseLimit[0][dim];

                    // randomize whether the noise is positive or negative
                    bool negativeNoise = rnd.NextDouble() < 0.5;
                    if (negativeNoise)
                    {
                        distorsion = distorsion * -1;
                    }

                    // add random noise (between -NL & +NL of the dimension) to the coordinates                 
                    similarFunction[dim][j] = referenceFuncData[dim][j] + distorsion;
                   // Debug.WriteLine($"{referenceFuncData[dim][j]} - {similarFunction[dim][j]} - {rndNoice}% - {distorsion}");
                }
            }

            return similarFunction;
        }

        /// <summary>
        /// Calculates the limits of the noise based on the noise range provided percentge.
        /// </summary>
        /// <param name="referenceFuncData">Reference function data</param>
        /// <param name="MinNoiseRangePercentage">minimum percentage of noise range compared to each attribute range</param>
        /// <param name="MaxNoiseRangePercentage">maximum percentage of noise range compared to each attribute range</param>
        /// <returns>the noise limits (first row: min and second row: max) of each attribute</returns>
        private static double[][] noiseLimit(double[][] referenceFuncData, int MinNoiseRangePercentage, int MaxNoiseRangePercentage)
        {
            // initialize the minimum, maximum and noise limit vectors
            double[] min = new double[referenceFuncData.Length];
            double[] max = new double[referenceFuncData.Length];
            double[][] nl = new double[2][];
            nl[0] = new double[referenceFuncData.Length];
            nl[1] = new double[referenceFuncData.Length];

            //
            // Get the minimum, maximum and noise limit of each dimension
            for (int i = 0; i < referenceFuncData.Length; i++)
            {
                for (int dimIndx = 0; dimIndx < referenceFuncData[0].Length; dimIndx++)
                {
                    if (dimIndx == 0)
                    {
                        min[i] = referenceFuncData[i][dimIndx];
                        max[i] = referenceFuncData[i][dimIndx];
                    }
                    else
                    {
                        if (referenceFuncData[i][dimIndx] < min[i])
                        {
                            min[i] = referenceFuncData[i][dimIndx];
                        }
                        else if (referenceFuncData[i][dimIndx] > max[i])
                        {
                            max[i] = referenceFuncData[i][dimIndx];
                        }
                    }
                }

                // calculate the minimum noise limit of the current dimension
                nl[0][i] = (max[i] - min[i]) * MinNoiseRangePercentage / 200;
                // calculate the maximum noise limit of the current dimension
                nl[0][i] = (max[i] - min[i]) * MaxNoiseRangePercentage / 200;
            }

            return nl;
        }
    }
}
