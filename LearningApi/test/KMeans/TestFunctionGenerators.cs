using LearningFoundation;
using LearningFoundation.DataMappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LearningFoundation.Clustering.KMeans;
using LearningFoundation.Helpers;

namespace Test
{   
    /// <summary>
    /// UnitTest01 is a class that contains a function to automatically generate similar functions and its test
    /// </summary>
    public class TestFunctionGenerators
    {
        private const string cPathPrefix = "\\NRP";

        //private const string rootFolder = "Functions";
        private static string rootFolder = System.IO.Path.GetFullPath(@"..\..\..\") + "KMeans\\TestFiles\\Functions\\";

        #region Tests

        static TestFunctionGenerators()
        {
            if (Directory.Exists(rootFolder) == false)
            {
                Directory.CreateDirectory(rootFolder);
            }
        }

        /// <summary>
        /// Test_GenerateSimilarFunctions is a test for generateSimilarFunctions
        /// </summary>
        [Fact]
        public void Test_GenerateSimilarFunctions()
        {
            // number of similar functions
            int NumSimFunc = 999;
            // percentage of added noise level (distortion) 
            int MinNoise = 5;
            int MaxNoise = 10;

            // generate the similar functions
            generateSimilarFunctions("COS_SIN X", NumSimFunc, MinNoise, MaxNoise);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// generateSimilarFunctions is a function that generates and saves similar functions to the given main one by adding noise to it
        /// </summary>
        /// <param name="functionName">path to main function</param>
        /// <param name="numFunctions">number of functions to generate</param>
        /// <param name="MinNoiseRangePercentage">minimum percentage of noise range compared to each attribute range</param>
        /// <param name="MaxNoiseRangePercentage">maximum percentage of noise range compared to each attribute range</param>
        private static void generateSimilarFunctions(string functionName, int numFunctions, int MinNoiseRangePercentage, int MaxNoiseRangePercentage)
        {
            string path = Path.Combine(rootFolder, functionName);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
                    
            string functionFileName = $"{functionName}.csv";

            Helpers.CheckOrCreateDefaultFunction(path, functionFileName, 100, 2);

            // load original function
            double[][] referenceFunction = Helpers.LoadFunctionData(Path.Combine(path, functionFileName));
            
            // complete file path and name
            string fName = path + "\\" + cPathPrefix + MinNoiseRangePercentage + "-" + MaxNoiseRangePercentage + "\\" + functionName + " SimilarFunctions NRP" + MinNoiseRangePercentage + "-" + MaxNoiseRangePercentage + ".csv";
            // Path.GetFileNameWithoutExtension(functionName)
            // complete file path and name for the normalized version
            string fNameNormalized = path + "\\" + cPathPrefix + MinNoiseRangePercentage + "-" + MaxNoiseRangePercentage + "\\" + functionName + " SimilarFunctions Normalized NRP" + MinNoiseRangePercentage + "-" + MaxNoiseRangePercentage + ".csv";
          
            // save original function in both new file
            Helpers.Write2CSVFile(referenceFunction, fName);
            Helpers.Write2CSVFile(normalizeData(referenceFunction), fNameNormalized);

            for (int i = 0; i < numFunctions; i++)
            {
                double[][] similarFuncData = FunctionGenerator.CreateSimilarFromReferenceFunc(referenceFunction, MinNoiseRangePercentage, MaxNoiseRangePercentage);

                // append the newly generated similar function
                Helpers.Write2CSVFile(similarFuncData, fName, true);
                Helpers.Write2CSVFile(normalizeData(similarFuncData), fNameNormalized, true);
            }
        }

     

       
        
        /// <summary>
        /// getRandomNumber is a function that generates a random number between a desired minimum and maximum
        /// </summary>
        /// <param name="minimum">lower limit for random number</param>
        /// <param name="maximum">upper limit for random number</param>
        /// <param name="seed">a seed for generating a random number</param>
        /// <returns></returns>
        private static double getRandomNumber(double minimum, double maximum, int seed)
        {
            Random rnd = new Random(seed * DateTime.Now.Millisecond);
            return rnd.NextDouble() * (maximum - minimum) + minimum;
        }

        /// <summary>
        /// normalizeData is a function that normalizes a given data
        /// </summary>
        /// <param name="Data">the data to be normalized</param>
        /// <returns></returns>
        internal static double[][] normalizeData(double[][] Data)
        {
            int RowsNumber = Data.Length;
            int ColumnsNumber = Data[0].Length;
            // initialize the normalized data
            double[][] NormalizedData = new double[RowsNumber][];
            for (int i = 0; i < Data.Length; i++)
            {
                NormalizedData[i] = new double[ColumnsNumber];
            }

            double[] MeanOfProperties = new double[RowsNumber];
            double[] VarianceOfProperties = new double[RowsNumber];
            bool EqualZero = false;

            for (int i = 0; i < RowsNumber; i++)
            {
                for (int j = 0; j < ColumnsNumber; j++)
                {
                    // calculate mean
                    MeanOfProperties[i] += Data[i][j] / ColumnsNumber; ;
                }
            }

            for (int i = 0; i < RowsNumber; i++)
            {
                for (int j = 0; j < ColumnsNumber; j++)
                {
                    // calculate variance
                    VarianceOfProperties[i] += (Math.Pow(Data[i][j] - MeanOfProperties[i], (double)2)) / ColumnsNumber;
                    if (VarianceOfProperties[i] == 0)
                    {
                        EqualZero = true;
                        break;
                    }
                }
            }
            // Normalize
            if (!EqualZero)
            {
                for (int i = 0; i < RowsNumber; i++)
                {
                    for (int j = 0; j < ColumnsNumber; j++)
                    {
                        NormalizedData[i][j] = (Data[i][j] - MeanOfProperties[i]) / Math.Sqrt(VarianceOfProperties[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < RowsNumber; i++)
                {
                    for (int j = 0; j < ColumnsNumber; j++)
                    {
                        NormalizedData[i][j] = Data[i][j] - MeanOfProperties[i];
                    }
                }
            }
            return NormalizedData;
        }

        #endregion
        
    }
}
