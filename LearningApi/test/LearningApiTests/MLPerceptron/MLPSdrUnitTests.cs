using LearningFoundation;
using MLPerceptron;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using NeuralNet.MLPerceptron;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LearningFoundation.Test.MLPerceptron
{
    /// <summary>
    /// Contains the unit test cases to test supervision built on top of SDR output of HTM algorithm.
    /// </summary>
    [TestClass]
    public class MLPSdrUnitTests
    {
        /// <summary>
        /// MLPSdrUnitTests Default constructor
        /// </summary>
        static MLPSdrUnitTests()
        {

        }

        /// <summary>
        /// Supervised training of unsupervised SDR output.
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="learningrate"></param>
        /// <param name="batchSize"></param>
        /// <param name="hiddenLayerNeurons"></param>
        /// <param name="iterationnumber"></param>
        [DataTestMethod]
        [DataRow(1000, 0.1, 25, new int[] { 6 }, 1)]
        public void UnitTestSdr(int iterations, double learningrate, int batchSize, int[] hiddenLayerNeurons, int iterationnumber)
        {
            LearningApi api = new LearningApi();

            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                List<double[]> rows = new List<double[]>();

                ctx.DataDescriptor = new DataDescriptor();

                var trainingFiles = Directory.GetFiles($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\Sdr");
                int rowCnt = 0;
                foreach (var file in trainingFiles)
                {
                    using (var reader = new StreamReader(file))
                    {
                        string line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            var tokens = line.Split(",");
                            List<string> newTokens = new List<string>();
                            foreach (var token in tokens)
                            {
                                if (token != " ")
                                    newTokens.Add(token);
                            }

                            tokens = newTokens.ToArray();

                            if (rowCnt == 0)
                            {
                                ctx.DataDescriptor.Features = new LearningFoundation.DataMappers.Column[tokens.Length - 1];
                                for (int i = 1; i < tokens.Length; i++)
                                {
                                    ctx.DataDescriptor.Features[i - 1] = new LearningFoundation.DataMappers.Column
                                    {
                                        Id = i,
                                        Index = i,
                                        Type = LearningFoundation.DataMappers.ColumnType.BINARY
                                    };
                                }
                                ctx.DataDescriptor.LabelIndex = -1;
                            }

                            // We have 65 features and digit number in file. to encode digits 0-9. 
                            // Digits can be represented as 9 bits.
                            double[] row = new double[tokens.Length - 1 + 10];
                            for (int i = 0; i < tokens.Length; i++)
                            {
                                row[i] = double.Parse(tokens[i], CultureInfo.InvariantCulture);
                            }

                            //
                            // This code encodes 9 digit classes as last 9 bits of training vector.
                            for (int k = 0; k < 10; k++)
                            {
                                if (double.Parse(tokens[0], CultureInfo.InvariantCulture) == k)
                                    row[tokens.Length - 1 + k] = 1;
                                else
                                    row[tokens.Length - 1 + k] = 0;
                            }

                            rows.Add(row);
                        }
                    }

                    rowCnt++;
                }

                return rows.ToArray();
            });

            //int[] hiddenLayerNeurons = { 6 };
            // Invoke the MLPerecptronAlgorithm with a specific learning rate, number of iterations
            api.UseMLPerceptron(learningrate, iterations, batchSize, iterationnumber, hiddenLayerNeurons);

            MLPerceptronAlgorithmScore score = api.Run() as MLPerceptronAlgorithmScore;

            api.Save("SdrMnistModel");

        }
    }
}






