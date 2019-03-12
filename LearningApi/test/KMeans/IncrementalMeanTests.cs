using LearningFoundation.Clustering.KMeans;
using LearningFoundation.DataMappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LearningFoundation.Test
{
    [TestClass]
    public class IncrementalMeanTests
    {
        /// <summary>
        /// Tests if incremental mean calculation works properly
        /// </summary>
        [TestMethod]
        public void Test_IncrementalMeanAverage()
        {
            // Test samples.
            double[][] data = new double[10][];

            // Each sample belongs to some cluster.
            int[] clustering = new int[data.Length];

            for (int i = 0; i < 10; i++)
            {
                data[i] = new double[] { i };
                clustering[i] = 0; // We have a single cluster. Every sample belongs to that cluster.
            }

            double[][] means = new double[1][];
            means[0] = new double[] { 0};

            KMeansAlgorithm.UpdateMeans(data, clustering, means);
            // Mean of 1,2,3,4,5,6,7,8,9, 10 is 4.5
            Assert.IsTrue(means[0][0] == 4.5);

            data = new double[5][];

            // Each sample belongs to some cluster.
            clustering = new int[data.Length];

            for (int i = 0; i < 5; i++)
            {
                data[i] = new double[] { i };
                clustering[i] = 0; // We have a single cluster. Every sample belongs to that cluster.
            }

            // Mean of 1,2,3,4,5 is 2
            KMeansAlgorithm.UpdateMeans(data, clustering, means, 0, new double[] { 0 });
            Assert.IsTrue(means[0][0] == 2);

            data = new double[5][];

            // Each sample belongs to some cluster.
            clustering = new int[data.Length];

            for (int i = 0; i < 5; i++)
            {
                data[i] = new double[] { i+5 };
                clustering[i] = 0; // We have a single cluster. Every sample belongs to that cluster.
            }

            KMeansAlgorithm.UpdateMeans(data, clustering, means, 5, new double[] { 2 });
            // M1 = mean of 1,2,3,4,5
            // M2 = mean of 6,7,8,9,10
            // Mean of M1 and M2 together is 4.5
            // (1/q1+q2)[q1*M1+q2*M2]
            // where q1 is number of elements inside of M1 and q2 number of elements inside of M2
            Assert.IsTrue(means[0][0] == 4.5);
        }


        /// <summary>
        /// Tests if incremental mean calculation works properly
        /// </summary>
        [TestMethod]
        public void Test_IncrementalMeanAverageSet()
        {
            for (int numOfSamples = 100; numOfSamples < 150000; numOfSamples += 15000)
            {
                // Test samples.
                double[][] data = new double[numOfSamples][];

                // Each sample belongs to some cluster.
                int[] clustering = new int[data.Length];

                for (int i = 0; i < numOfSamples; i++)
                {
                    data[i] = new double[] { i };
                    clustering[i] = 0; // We have a single cluster. Every sample belongs to that cluster.
                }

                double[][] means = new double[1][];
                means[0] = new double[] { 0 };

                KMeansAlgorithm.UpdateMeans(data, clustering, means);
               
                // Mean of numOfSamples
                var mean = means[0][0];

                data = new double[numOfSamples/2][];

                // Each sample belongs to some cluster.
                clustering = new int[data.Length];

                for (int i = 0; i < numOfSamples / 2; i++)
                {
                    data[i] = new double[] { i };
                    clustering[i] = 0; // We have a single cluster. Every sample belongs to that cluster.
                }

                // Calculate mean of numOfSamples/2
                KMeansAlgorithm.UpdateMeans(data, clustering, means, 0, new double[] { 0 });
                // Mean of numOfSamples/2
                var mean1 = means[0][0];

                data = new double[numOfSamples / 2][];

                // Each sample belongs to some cluster.
                clustering = new int[data.Length];

                for (int i = 0; i < numOfSamples/2; i++)
                {
                    data[i] = new double[] { i + (numOfSamples / 2 )};
                    clustering[i] = 0; // We have a single cluster. Every sample belongs to that cluster.
                }

                KMeansAlgorithm.UpdateMeans(data, clustering, means, numOfSamples/2, new double[] { mean1 });
              
                // Mean for numbers from numOfSamples/2 to numOfSamples
                var mean2 = means[0][0];

                // M1 = mean of numOfSamples/2 (minibatch 1)
                // M2 = mean for numbers from numOfSamples/2 to numOfSamples (minibatch 2)
                // mean is batch for numbers from 1 to numOfSamples
                // (1/q1+q2)[q1*M1+q2*M2]
                // where q1 is number of elements inside of M1 and q2 number of elements inside of M2
                Assert.IsTrue(Math.Round(mean2,2) == Math.Round(mean,2));
            }
        }

        private static DataDescriptor loadDescriptor()
        {
            var des = new DataDescriptor();

            des.Features = new Column[2];
            des.Features[0] = new Column { Id = 1, Name = "Height", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0.0 };
            des.Features[1] = new Column { Id = 2, Name = "Weight", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0.0 };

            return des;
        }


    }
}
