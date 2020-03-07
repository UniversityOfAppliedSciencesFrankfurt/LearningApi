// Copyright (c) daenet GmbH / Frankfurt University of Applied Sciences. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;

namespace LearningFoundation.EuclideanColorFilter
{
    /// <summary>
    /// 
    /// </summary>
    public static class CalcDistance
    {
        /// <summary>
        /// Calculate the Distance between two Points in 3D with the formula: distance = Sqrt((x2 - x1)² + (y2 - y1)² + (z2 - z1)²)    
        /// </summary>
        /// <param name="pixelColor"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static float ComputeEuclideanDistance(Color pixelColor, Color center)
        {
            float r2 = (float) Math.Pow(pixelColor.R - center.R, 2);
            float g2 = (float) Math.Pow(pixelColor.G - center.G, 2);
            float b2 = (float) Math.Pow(pixelColor.B - center.B, 2);

            return (float) Math.Sqrt(r2 + g2 + b2);
        }
    }
}
