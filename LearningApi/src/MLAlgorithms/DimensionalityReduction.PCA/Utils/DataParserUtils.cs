using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using LearningFoundation;
using LearningFoundation.DataProviders;
using LearningFoundation.DimensionalityReduction.PCA;
using LearningFoundation.DimensionalityReduction.PCA.Utils;

namespace LearningFoundation.DimensionalityReduction.PCA.Utils
{
    /// <summary>
    /// Some time data that returned from CsvDataProvider or others pipeline is inside an object of type 'object'
    /// or in an object[][] array. 
    /// This class have method that help to convert object -> double[][] , or object[][] -> double[][]
    /// </summary>
    public class DataParserUtils
    {
        /// <summary>
        /// Parse and exstract an object of type 'object' back to type double[][]
        /// </summary>
        /// <param name="input">object of type 'object'</param>
        /// <returns>double[][] array</returns>
        public static double[][] ParseObjectToDouble2DArray(object input)
        {
            List<double[]> result = new List<double[]>();
            IEnumerable inputEnumerable = input as IEnumerable;
            foreach (double[] inputItem in inputEnumerable)
            {
                result.Add(inputItem);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Parse and exstract an array of type 'object[][]' back to type double[][]
        /// </summary>
        /// <param name="input">object of type 'object[][]'</param>
        /// <returns>double[][] array</returns>
        public static double[][] ParseObject2DArrayToDouble2DArray(object[][] input) {
            double[][] res = new double[input.GetLength(0)][];
            for (int i = 0; i < input.GetLength(0); i++) {
                res[i] = new double[input[i].GetLength(0)];
                for (int j = 0; j < input[i].GetLength(0); j++) {
                    res[i][j] = Double.Parse(Convert.ToString(input[i][j]));
                }
            }
            return res;
        }
    }
}