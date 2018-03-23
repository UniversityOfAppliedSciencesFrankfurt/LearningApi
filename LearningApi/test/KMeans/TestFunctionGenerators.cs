using LearningFoundation;
using LearningFoundation.DataMappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using LearningFoundation.Clustering.KMeans;

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
            int NRP = 10;

            // generate the similar functions
            generateSimilarFunctions("TestFile01", NumSimFunc, NRP);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// generateSimilarFunctions is a function that generates and saves similar functions to the given main one by adding noise to it
        /// </summary>
        /// <param name="functionName">path to main function</param>
        /// <param name="numFunctions">number of functions to generate</param>
        /// <param name="noiseRangePercentage">percentage of noise range compared to each attribute range</param>
        private static void generateSimilarFunctions(string functionName, int numFunctions, int noiseRangePercentage)
        {
            string path = Path.Combine(rootFolder, functionName);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
                    
            string functionFileName = $"{functionName}.csv";

            Helpers.CheckOrCreateDefaultFunction(path, functionFileName, 100, 2);

            // load original function
            double[][] mFun = Helpers.LoadFunctionData(Path.Combine(path, functionFileName));

            // initialize for the similar function
            double[][] mFun2 = new double[mFun.Length][];
            for (int i = 0; i < mFun2.Length; i++)
            {
                mFun2[i] = new double[mFun[0].Length];
            }

            // complete file path and name
            string fName = path + "\\" + cPathPrefix + noiseRangePercentage + "\\" + Path.GetFileNameWithoutExtension(functionName) + " SimilarFunctions NRP" + noiseRangePercentage + ".csv";
           
            // complete file path and name for the normalized version
            string fNameNormalized = path + "\\" +cPathPrefix + noiseRangePercentage + "\\" + Path.GetFileNameWithoutExtension(functionName) + " SimilarFunctions Normalized NRP" + noiseRangePercentage + ".csv";
          
            // save original function in both new file
            Helpers.Write2CSVFile(mFun, fName);
            Helpers.Write2CSVFile(normalizeData(mFun), fNameNormalized);

            // seed for random numbers
            int seed = 0;
            // calculte random noise limits
            double[] randomNoiseLimit = noiseLimit(mFun, noiseRangePercentage);

            for (int i = 0; i < numFunctions; i++)
            {
                mFun2[0] = mFun[0];
                //for all other dimensions
                for (int d = 1; d < mFun.Length; d++)
                {
                    // add noise on each point of the function
                    for (int j = 0; j < mFun[0].Length; j++)
                    {
                        // add random noise (between -NL & +NL of the dimension) to the coordinates                 
                        mFun2[d][j] = mFun[d][j] + getRandomNumber(randomNoiseLimit[d] * -1, randomNoiseLimit[d], seed);
                        seed++;
                    }
                }
                // append the newly generated similar function
                Helpers.Write2CSVFile(mFun2, fName, true);
                Helpers.Write2CSVFile(normalizeData(mFun2), fNameNormalized, true);
            }
        }

        /// <summary>
        /// noiseLimit is a function that calculates the limits of the noise based on the noise range percentge provided by the user
        /// </summary>
        /// <param name="baseFunction">main function</param>
        /// <param name="noiseRangePercentage">percentage of noise range compared to each attribute range</param>
        /// <returns>the noise limit of each attribute</returns>
        private static double[] noiseLimit (double[][] baseFunction, int noiseRangePercentage)
        {
            // initialize the minimum, maximum and noise limit vectors
            double[] min = new double[baseFunction.Length];
            double[] max = new double[baseFunction.Length];
            double[] nl = new double[baseFunction.Length];
            // get the minimum, maximum and noise limit of each dimension
            for (int i = 0; i < baseFunction.Length; i++)
            {
                for (int j = 0; j < baseFunction[0].Length; j++)
                {
                    if (j == 0)
                    {
                        min[i] = baseFunction[i][j];
                        max[i] = baseFunction[i][j];
                    }
                    else
                    {
                        if (baseFunction[i][j] < min[i])
                        {
                            min[i] = baseFunction[i][j];
                        }
                        else if(baseFunction[i][j] > max[i])
                        {
                            max[i] = baseFunction[i][j];
                        }
                    }
                }
                // calculate noise limit of the current dimension
                nl[i] = (max[i] - min[i])*noiseRangePercentage/200;
            }

            return nl;
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
        private static double[][] normalizeData(double[][] Data)
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
