using System;
using System.Drawing;
using LearningFoundation;

// Copyright (c) daenet GmbH / Frankfurt University of Applied Sciences. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace EuclideanColorFilter
{
    /// <summary>
    ///Main class in which the algorithm is implemented
    /// </summary>
    public class EuclideanFilterModule : IPipelineModule<double[,,], double[,,]>
    {
        /// <summary>
        /// Radius property which is necessary for filtering
        /// </summary>
        public float Radius; //{ get; }

        /// <summary>
        /// Center property which is necessary for filtering
        /// </summary>
        public Color Center; //{ set; } 

        /// <summary>
        /// Constructor with two arguments
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public EuclideanFilterModule(Color center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Calculate the distance between each Pixel and Center-RGB-Value. If the Distance is within the Radius we just give back the orginal pixel.
        /// If the Distance is bigger than the Radius we set the pixel to black. After looping through all pixel we get back the filtered Image.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public double[,,] Run(double[,,] data, IContext ctx)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }

            int width = data.GetLength(0);
            int height = data.GetLength(1);
            int numColors = data.GetLength(2);

            double[,,] result = new double[width, height, numColors];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color color = GetAndSetPixels.GetPixel(data, i, j);
                    float distance = CalcDistance.ComputeEuclideanDistance(color, Center);

                    if (distance <= Radius)
                    {
                        GetAndSetPixels.SetPixel(result, i, j, color);
                    }
                    else
                    {
                        GetAndSetPixels.SetPixel(result, i, j, Color.Black);
                    }

                }
            }

            return result;
        }
    }
}
