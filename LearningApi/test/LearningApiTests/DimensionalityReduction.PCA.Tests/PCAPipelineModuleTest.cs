using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;
using LearningFoundation.DataProviders;
using LearningFoundation.DimensionalityReduction.PCA;
using LearningFoundation.DimensionalityReduction.PCA.Utils;
using System.IO;
using DimensionalityReduction.PCA.Utils;
using DimensionalityReduction.PCA.Tests.Const;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DimensionalityReduction.PCA.Tests
{
    /// <summary>
    /// Test the functionality of PCAPipelineModule
    /// </summary>
   [TestClass]
    public class PCAPipelineModuleTest
    {
        private static double[] TestCaseWithExpectedLosses = new double[] { 0.01, 0.03, 0.05, 0.1 };

        /// <summary>
        /// Test to use the PCAPipelineModule with a bitmap image
        /// </summary>
        [TestMethod]
        public void Test_PCAPipelineWithBitmapImage()
        {
            foreach (string filePath in Directory.GetFiles(TestConstants.INPUT_DATA_DIR))
            {
                string fileName = Path.GetFileName(filePath);
                if (fileName.EndsWith(".bmp"))
                {
                    double[][] data = ImageUtils.getImageAsDoubleArray(filePath);
                    Assert.IsNotNull(data);

                    foreach (double expectedLoss in TestCaseWithExpectedLosses)
                    {
                        PCAPipelineModule pca = new PCAPipelineModule(maximumLoss: expectedLoss);
                        double[][] result = pca.Run(data, null);
                        Assert.IsNotNull(result);

                        double[][] estimatedData = pca.EstimatedData;
                        Assert.IsNotNull(estimatedData);

                        string percentSuffix = Convert.ToString(Convert.ToInt32(100.0 * expectedLoss));
                        string outputFilePath = Path.Combine(TestConstants.OUTPUT_RESULT_DIR, fileName.Replace(".bmp","_") + percentSuffix + "percent_loss.bmp");
                        ImageUtils.saveDoubleArrayToImage(outputFilePath, estimatedData);
                    }
                }
            }
        }

        /// <summary>
        /// Test to use the PCAPipelineModule with a data stored in csv
        /// </summary>
        [TestMethod]
        public void Test_PCAPipelineWithCSVImage()
        {
            foreach (string filePath in Directory.GetFiles(TestConstants.INPUT_DATA_DIR))
            {
                string fileName = Path.GetFileName(filePath);
                if (fileName.EndsWith(".pgm.csv"))
                {
                    double[][] data = CsvUtils.ReadCSVToDouble2DArray(filePath, ',');
                    Assert.IsNotNull(data);

                    foreach (double expectedLoss in TestCaseWithExpectedLosses)
                    {
                        PCAPipelineModule pca = new PCAPipelineModule(maximumLoss: expectedLoss);
                        double[][] result = pca.Run(data, null);
                        Assert.IsNotNull(result);

                        double[][] estimatedData = pca.EstimatedData;
                        Assert.IsNotNull(estimatedData);

                        string percentSuffix = Convert.ToString(Convert.ToInt32(100.0 * expectedLoss));
                        string outputFilePath = Path.Combine(TestConstants.OUTPUT_RESULT_DIR, fileName.Replace(".pgm.csv", "_") + percentSuffix + "percent_loss.csv");
                        CsvUtils.WriteCSVFromDouble2DArray(outputFilePath, ',', estimatedData);
                    }
                }
            }
        }
    }
}
