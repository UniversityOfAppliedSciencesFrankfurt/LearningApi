using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaussianAndMeanFilter
{
    public class GaussianFilter : IPipelineModule<double[,,], double[,,]>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public double[,,] Run(double[,,] data, IContext ctx)
        {
            return filter(data);
        }

        /// <summary>
        /// Implementing a Gaussian filter matrix of (3x3) dimention
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private double[,,] filter(double[,,] data)
        {
            double[,,] result = data;

            for (int x = 1; x < data.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < data.GetLength(1) - 1; y++)
                {
                    //Red value first column 
                    var prev11R = data[x - 1, y - 1, 0];
                    var prev12R = data[x, y - 1, 0];
                    var prev13R = data[x + 1, y - 1, 0];

                    //Red value sencond column 
                    var prev21R = data[x - 1, y, 0];
                    var prev22R = data[x, y, 0];
                    var prev23R = data[x + 1, y, 0];

                    //Red value third column
                    var prev31R = data[x - 1, y + 1, 0];
                    var prev32R = data[x, y + 1, 0];
                    var prev33R = data[x + 1, y + 1, 0];

                    //Green value first column 
                    var prev11G = data[x - 1, y - 1, 1];
                    var prev12G = data[x, y - 1, 1];
                    var prev13G = data[x + 1, y - 1, 1];

                    //Green value sencond column 
                    var prev21G = data[x - 1, y, 1];
                    var prev22G = data[x, y, 1];
                    var prev23G = data[x + 1, y, 1];

                    //Green value third column
                    var prev31G = data[x - 1, y + 1, 1];
                    var prev32G = data[x, y + 1, 1];
                    var prev33G = data[x + 1, y + 1, 1];

                    //Blue value first column 
                    var prev11B = data[x - 1, y - 1, 2];
                    var prev12B = data[x, y - 1, 2];
                    var prev13B = data[x + 1, y - 1, 2];

                    //Blue value sencond column 
                    var prev21B = data[x - 1, y, 2];
                    var prev22B = data[x, y, 2];
                    var prev23B = data[x + 1, y, 2];

                    //Blue value third column
                    var prev31B = data[x - 1, y + 1, 2];
                    var prev32B = data[x, y + 1, 2];
                    var prev33B = data[x + 1, y + 1, 2];

                    //Calculating new pixel value
                    double avgR = (prev11R * 1 + prev12R * 2 + prev13R * 1 + prev21R * 2 + prev22R * 4 + prev23R * 2 + prev31R * 1 + prev32R * 2 + prev33R * 1) / 16;
                    double avgG = (prev11G * 1 + prev12G * 2 + prev13G * 1 + prev21G * 2 + prev22G * 4 + prev23G * 2 + prev31G * 1 + prev32G * 2 + prev33G * 1) / 16;
                    double avgB = (prev11B * 1 + prev12B * 2 + prev13B * 1 + prev21B * 2 + prev22B * 4 + prev23B * 2 + prev31B * 1 + prev32B * 2 + prev33B * 1) / 16;

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
