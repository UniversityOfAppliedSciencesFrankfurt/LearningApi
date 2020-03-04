using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;

namespace GaussianAndMeanFilter
{
    public class MeanFilter : IPipelineModule<double[,,], double[,,]>
    {
        /// <summary>
        /// Inherited from IPipeLine module
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public double[,,] Run(double[,,] data, IContext ctx)
        {
            return filter(data);
        }

        /// <summary>
        /// Implementing a Mean filter matrix of (3x3) dimention
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private double[,,] filter(double[,,] data)
        {
            double[,,] result = data;

            for (int x = 1; x < data.GetLength(0)-1; x++)
            {
                for (int y = 1; y < data.GetLength(1)-1; y++)
                {
                    //Reading Red value first column of the 3x3 matrix
                    var prev11R = data[x - 1, y - 1, 0];
                    var prev12R = data[x , y - 1, 0];
                    var prev13R = data[x + 1, y - 1, 0];

                    //Reading Red value sencond column of the 3x3 matrix
                    var prev21R = data[x - 1, y, 0];
                    var prev22R = data[x , y, 0];
                    var prev23R = data[x + 1, y, 0];

                    //Reading Red value third column of the 3x3 matrix
                    var prev31R = data[x - 1, y+1, 0];
                    var prev32R = data[x, y+1, 0];
                    var prev33R = data[x + 1, y+1, 0];

                    //Reading Green value first column of the 3x3 matrix
                    var prev11G = data[x - 1, y - 1, 1];
                    var prev12G = data[x, y - 1, 1];
                    var prev13G = data[x + 1, y - 1, 1];

                    //Reading Green value sencond column of the 3x3 matrix
                    var prev21G = data[x - 1, y, 1];
                    var prev22G = data[x, y, 1];
                    var prev23G = data[x + 1, y, 1];

                    //Reading Green value third column of the 3x3 matrix
                    var prev31G = data[x - 1, y + 1, 1];
                    var prev32G = data[x, y + 1, 1];
                    var prev33G = data[x + 1, y + 1, 1];

                    //Reading Blue value first column of the 3x3 matrix
                    var prev11B = data[x - 1, y - 1, 2];
                    var prev12B = data[x, y - 1, 2];
                    var prev13B = data[x + 1, y - 1, 2];

                    //Reading Blue value sencond column of the 3x3 matrix
                    var prev21B = data[x - 1, y, 2];
                    var prev22B = data[x, y, 2];
                    var prev23B = data[x + 1, y, 2];

                    //Reading Blue value third column of the 3x3 matrix
                    var prev31B = data[x - 1, y + 1, 2];
                    var prev32B = data[x, y + 1, 2];
                    var prev33B = data[x + 1, y + 1, 2];

                    //Calculating the mean value
                    double avgR = (prev11R + prev12R + prev13R + prev21R + prev22R + prev23R + prev31R + prev32R + prev33R) / 9;
                    double avgG = (prev11G + prev12G + prev13G + prev21G + prev22G + prev23G + prev31G + prev32G + prev33G) / 9;
                    double avgB = (prev11B + prev12B + prev13B + prev21B + prev22B + prev23B + prev31B + prev32B + prev33B) / 9;

                    //Replacing the original pixel value by the calculated value
                    result[x, y, 0] = avgR;
                    result[x, y, 1] = avgG;
                    result[x, y, 2] = avgB;
                }
            }

            return result;
        }
    }
}
