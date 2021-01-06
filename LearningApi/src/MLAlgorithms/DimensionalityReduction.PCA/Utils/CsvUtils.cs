using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;
using LearningFoundation.DataProviders;
using LearningFoundation.DimensionalityReduction.PCA;
using LearningFoundation.DimensionalityReduction.PCA.Utils;

namespace LearningFoundation.DimensionalityReduction.PCA.Utils
{
    /// <summary>
    /// CsvUtils is using for reading CSV data
    /// </summary>
    public class CsvUtils
    {
        /// <summary>
        /// Read data stored as csv format into a double[][] array
        /// </summary>
        /// <param name="path">full path of the csv file</param>
        /// <param name="delimiter">char represent the delimiter</param>
        /// <returns>double[][] array represent data in csv file</returns>
        public static double[][] ReadCSVToDouble2DArray(string path, char delimiter)
        {
            CsvDataProvider csvReader = new CsvDataProvider(path, delimiter, false);
            object[][] rawData = csvReader.Run(null, null);
            double[][] data = DataParserUtils.ParseObject2DArrayToDouble2DArray(rawData);
            return data;
        }

        /// <summary>
        /// Write data in a double[][] array into a csv file
        /// </summary>
        /// <param name="path">full path of the destination file</param>
        /// <param name="delimiter">char to delimiter</param>
        /// <param name="inputData">input data as double[][] array</param>
        public static void WriteCSVFromDouble2DArray(string path, char delimiter, double[][] inputData){
            int rows = inputData.GetLength(0);
            List<string> lines = new List<string>();

            for (int i = 0; i < rows; i++) {
                int cols = inputData[i].GetLength(0);
                string line = "";
                for (int j = 0; j < cols; j++) {
                    if (j != 0) {
                        line += Char.ToString(delimiter);
                    }
                    line += inputData[i][j].ToString("0.####");
                }
                lines.Add(line);
            }

            string[] linesArray = lines.ToArray();
            System.IO.File.WriteAllLines(path, linesArray);
        }
    }
}