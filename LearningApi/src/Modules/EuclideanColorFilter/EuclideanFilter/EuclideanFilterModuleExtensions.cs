// Copyright (c) daenet GmbH / Frankfurt University of Applied Sciences. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using LearningFoundation;
using System.Drawing;

namespace LearningFoundation.EuclideanColorFilter
{
    /// <summary>
    ///ExtenstionMethod Class of Euclidean Filter Module Tester
    /// </summary>
    public static class EuclideanFilterModuleExtensions
    {
        /// <summary>
        /// Extension method of Learning Api -> This is needed, to use the "api.Run() as.." - method
        /// </summary>
        /// <param name="api"></param>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static LearningApi UseMyPipelineModule(this LearningApi api, Color color, float radius)
        {
            EuclideanFilterModule module = new EuclideanFilterModule(color, radius);
            api.AddModule(module, $"EuclideanFilterModule-{Guid.NewGuid()}");
            return api;
        }
    }
    public class Distance
    {

        /// <summary>
        /// Apply the Euclidean Color Filter Formula on two random point on pixel of Image. 
        /// Return the distance between 2 points using euclidean distance formula.
        /// </summary>
        public static double EuclideanFormula(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        /// <summary>
        /// Take two any points from Image as pixel point to check whether collected two points of same color or not.
        /// Calculates the similarity between 2 points using Euclidean distance.
        /// Returns a value between 0 and 1 where 0 means they are identical
        /// </summary>
        public static double EuclideanSimilarity(Point p1, Point p2)
        {
            return 1 / (1 + EuclideanFormula(p1, p2));
        }

        /// <summary>
        /// Calculates the distance between match and current color using Euclidean distance.
        /// Returns a value between 0 and 1 where 0 means current color equals to match color.
        /// </summary>
        public static int ColorMatching(Color current, Color match)
        {
            int redDifference;
            int greenDifference;
            int blueDifference;

            redDifference = current.R - match.R;
            greenDifference = current.G - match.G;
            blueDifference = current.B - match.B;

            return redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;

        }

        /// <summary>
        /// Find the nearest color for current color from various map color using Euclidean distance.
        /// Returns a value of index where 0 index means map color is nearest to match color.
        /// </summary>
        /// 
        public static int FindNearestColor(Color[] map, Color current)
        {
            int shortestDistance;
            int index;

            index = -1;
            shortestDistance = int.MaxValue;

            for (int i = 0; i < map.Length; i++)
            {
                Color match;
                int distance;

                match = map[i];
                distance = ColorMatching(current, match);

                if (distance < shortestDistance)
                {
                    index = i;
                    shortestDistance = distance;
                }
            }

            return index;
        }
    }   
}
