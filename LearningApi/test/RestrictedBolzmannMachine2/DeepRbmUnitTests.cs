using LearningFoundation;
using System;
using System.Linq;
using Xunit;
using NeuralNet.RestrictedBolzmannMachine2;
using System.IO;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using LearningFoundation.Arrays;
using System.Threading;
using System.Collections.Generic;

namespace test.RestrictedBolzmannMachine2
{
    /// <summary>
    /// Tests for DeepRbm Algorithm.
    /// </summary>
    public class DeepRbmUnitTests
    {
       
        /// <summary>
        /// RBM is not supervised algorithm. This is why we do not have a label.
        /// </summary>
        /// <returns></returns>
        private DataDescriptor getDescriptorForRbm_sample1()
        {
            DataDescriptor des = new DataDescriptor();
            des.Features = new LearningFoundation.DataMappers.Column[6];

            // Label not used.
            des.LabelIndex = -1;

            des.Features = new Column[6];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[2] = new Column { Id = 3, Name = "col3", Index = 2, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[3] = new Column { Id = 4, Name = "col4", Index = 3, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[4] = new Column { Id = 5, Name = "col5", Index = 4, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[5] = new Column { Id = 6, Name = "col6", Index = 5, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };

            return des;
        }
 

        /// <summary>
        /// RBM is not supervised algorithm. This is why we do not have a label.
        /// </summary>
        /// <returns></returns>
        private DataDescriptor getDescriptorForRbmTwoClassesClassifier(int dims)
        {
            DataDescriptor des = new DataDescriptor();
            des.Features = new LearningFoundation.DataMappers.Column[dims];

            // Label not used.
            des.LabelIndex = -1;
            des.Features = new Column[dims];
            for (int i = 0; i < dims; i++)
            {
                des.Features[i] = new Column { Id = i, Name = $"col{i}", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            }

            return des;
        }

      

        /// <summary>
        /// TODO...
        /// </summary>
        [Theory]
        //[InlineData(1, 4096, new int[] { 4096, 250, 10 })]       
        [InlineData(1, 0.2, new int[] { 4096, 20, 10 })]
        public void DigitRecognitionDeepTest(int iterations, double learningRate, int[] layers)
        {
            Debug.WriteLine($"{iterations}-{String.Join("", layers)}");

            LearningApi api = new LearningApi(RbmHandwrittenDigitUnitTests.GetDescriptorForDigits());

            // Initialize data provider
            // TODO: Describe Digit Dataset.
            api.UseCsvDataProvider(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\DigitDataset.csv"), ',', false, 0);
            api.UseDefaultDataMapper();

            api.UseDeepRbm(learningRate, iterations, layers);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            RbmDeepScore score = api.Run() as RbmDeepScore;
            watch.Stop();

            var testData = RbmHandwrittenDigitUnitTests.ReadData(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\DigitTest.csv"));

            var result = api.Algorithm.Predict(testData, api.Context) as RbmDeepResult;
            var accList = new double[result.Results.Count];
            var predictions = new double[result.Results.Count][];
            var predictedHiddenNodes = new double[result.Results.Count][];
            var Time = watch.ElapsedMilliseconds / 1000;

            int i = 0;
            foreach (var item in result.Results)
            {
                predictions[i] = item.First().VisibleNodesPredictions;
                predictedHiddenNodes[i] = item.Last().HiddenNodesPredictions;
                accList[i] = testData[i].GetHammingDistance(predictions[i]);
                i++;
            }

            RbmHandwrittenDigitUnitTests.WriteDeepResult(iterations, layers, accList, Time/60.0 ,predictedHiddenNodes );
            /// write predicted hidden nodes.......
            RbmHandwrittenDigitUnitTests.WriteOutputMatrix(iterations, layers, predictions, testData);
        }



        /// <summary>
        /// This test provides data, which contains two patterns.
        /// First pattern is concentrated on left and second pattern is concentrated on right.
        /// Sample data is stored in 'rbm_twoclass_sample.csv'.
        /// Data looks like:
        /// 011111000000
        /// 000000001110
        /// It is concentrated on left or on right.
        /// </summary>
        [Fact]
        public void Rbm_ClassifierTest()
        {
            var dataPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\rbm_twoclass_sample.csv");

            LearningApi api = new LearningApi(this.getDescriptorForRbmTwoClassesClassifier(10));

            // Initialize data provider
            api.UseCsvDataProvider(dataPath, ';', false, 1);
            api.UseDefaultDataMapper();
            api.UseDeepRbm(0.2, 1000, new int[] { 10, 2 });
             
            RbmResult score = api.Run() as RbmResult;

            double[][] testData = new double[5][];

            //
            // This test data contains two patterns. One is grouped at left and one at almost right.
            testData[0] = new double[] { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
            testData[1] = new double[] { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
            testData[2] = new double[] { 0, 0, 0, 0, 0, 1, 1, 1, 0, 0 };
            testData[3] = new double[] { 0, 0, 0, 0, 0, 1, 0, 1, 0, 0 };

            // This will be classified as third class.
            testData[4] = new double[] { 1, 1, 1, 0, 0, 1, 1, 1, 0, 0 };

            RbmDeepResult result = api.Algorithm.Predict(testData, api.Context) as RbmDeepResult;

            //
            // 2 * BIT1 + BIT2 of [0] and [1] should be same.
            // We don't know how RBM will classiffy data. We only expect that
            // same or similar pattern of data will be assigned to the same class.
            // Note, we have here two classes (two hiddne nodes).
            // First and second data sample are of the same class. 
            // Third and fourth are also of same class. See data.

            // Here we check first classs.
            Assert.True(result.Results[0].ToArray()[0].HiddenNodesPredictions[0] == result.Results[1].ToArray()[0].HiddenNodesPredictions[0] &&
                result.Results[0].ToArray()[0].HiddenNodesPredictions[1] == result.Results[1].ToArray()[0].HiddenNodesPredictions[1]);

            // Here is test for second class.
            Assert.True(result.Results[2].ToArray()[0].HiddenNodesPredictions[0] == result.Results[3].ToArray()[0].HiddenNodesPredictions[0] &&
                result.Results[2].ToArray()[0].HiddenNodesPredictions[1] == result.Results[3].ToArray()[0].HiddenNodesPredictions[1]);

        }


        [Fact]
        public void Rbm_ClassifierDeepTest()
        {
            var dataPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\rbm_twoclass_sample.csv");

            LearningApi api = new LearningApi(this.getDescriptorForRbmTwoClassesClassifier(10));

            // Initialize data provider
            api.UseCsvDataProvider(dataPath, ';', false, 1);
            api.UseDefaultDataMapper();
            api.UseDeepRbm(0.2, 1000, new int[] { 10, 8, 5, 2 });

            RbmResult score = api.Run() as RbmResult;

            double[][] testData = new double[5][];

            //
            // This test data contains two patterns. One is grouped at left and one at almost right.
            testData[0] = new double[] { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
            testData[1] = new double[] { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
            testData[2] = new double[] { 0, 0, 0, 0, 0, 1, 1, 1, 0, 0 };
            testData[3] = new double[] { 0, 0, 0, 0, 0, 1, 0, 1, 0, 0 };

            // This will be classified as third class.
            testData[4] = new double[] { 1, 1, 1, 0, 0, 1, 1, 1, 0, 0 };

            RbmDeepResult result = api.Algorithm.Predict(testData, api.Context) as RbmDeepResult;

            //
            // 2 * BIT1 + BIT2 of [0] and [1] should be same.
            // We don't know how RBM will classiffy data. We only expect that
            // same or similar pattern of data will be assigned to the same class.
            // Note, we have here two classes (two hiddne nodes).
            // First and second data sample are of the same class. 
            // Third and fourth are also of same class. See data.

            // Here we check first classs.
            Assert.True(result.Results[0].ToArray()[2].HiddenNodesPredictions[0] == result.Results[1].ToArray()[2].HiddenNodesPredictions[0] &&
                result.Results[0].ToArray()[2].HiddenNodesPredictions[1] == result.Results[1].ToArray()[2].HiddenNodesPredictions[1]);

            // Here is test for second class.
            Assert.True(result.Results[2].ToArray()[2].HiddenNodesPredictions[0] == result.Results[3].ToArray()[2].HiddenNodesPredictions[0] &&
                result.Results[2].ToArray()[2].HiddenNodesPredictions[1] == result.Results[3].ToArray()[2].HiddenNodesPredictions[1]);

        }


        /// <summary>
        ///
        /// </summary>
        [Fact]
        public void Rbm_ClassifierTest2()
        {
            var dataPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\rbm_sample2.csv");

            LearningApi api = new LearningApi(this.getDescriptorForRbmTwoClassesClassifier(21));

            // Initialize data provider
            api.UseCsvDataProvider(dataPath, ',', false, 1);
            api.UseDefaultDataMapper();
            api.UseDeepRbm(0.2, 10000, new int[] { 21, 9, 2 });

            RbmResult score = api.Run() as RbmResult;

            var expectedResults = new Dictionary<int, List<double[]>>();

            // All test data, which belong to the sam class.
            List<double[]> testListClass1 = new List<double[]>();
            List<double[]> testListClass2 = new List<double[]>();

            //
            // This test data contains two patterns. One is grouped at left and one at almost right.
            // testListClass1 contains class 1
            // testListClass2 contains class 2
            testListClass1.Add(new double[] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            testListClass1.Add(new double[] { 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            testListClass1.Add(new double[] { 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

            testListClass2.Add(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 });
            testListClass2.Add(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0 });
            testListClass2.Add(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 });

            expectedResults.Add(1, testListClass1);
            expectedResults.Add(2, testListClass2);

            validateClassificationResult(api, expectedResults);
        }


        /// <summary>
        /// Validates prediction result of Deep RBM network.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="expectedResults"></param>
        private static void validateClassificationResult(LearningApi api, Dictionary<int, List<double[]>> expectedResults)
        {
            // Every class has a result.
            List<double> distinctResults = new List<double>();

            //
            // Traverse all test sample vectors, which are expected to be classified as
            // of same class.
            foreach (var keyPair in expectedResults)
            {

                RbmDeepResult result = api.Algorithm.Predict(keyPair.Value.ToArray(), api.Context) as RbmDeepResult;

                // We take encoding of first predicted vector of the current class
                // and calculate binary result from it. We don't know encoding
                // if this class, because on every run of unit test encding might be different.
                // Whatever encoding is for this class we take it as reference value.
                var classResult = result.Results[0].Last().HiddenNodesPredictions.ToBinary();

                // Here we make sure that all prediction of the same class are same as refernece value,
                // means same.
                for (var i = 0; i < result.Results.Count; i++)
                {
                    Assert.True(result.Results[i].Last().HiddenNodesPredictions.ToBinary() == classResult);
                }

                distinctResults.Add(classResult);
            }

            //
            // All classes must have difference result.(encoding)
            for (int i = 0; i < distinctResults.Count -1; i++)
            {
                for (int j = i+1; j < distinctResults.Count; j++)
                {
                    Assert.True(distinctResults[i] != distinctResults[j]);
                }
            }
        }
    }




}
