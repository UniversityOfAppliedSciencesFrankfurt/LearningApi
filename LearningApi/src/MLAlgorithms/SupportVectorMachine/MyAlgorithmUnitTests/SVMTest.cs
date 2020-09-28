using LearningFoundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupportVectorMachineAlgorithm;
using System.Collections.Generic;
using System.IO;
using LearningFoundation.DataProviders;
using System;
using System.Globalization;

namespace SupportVectorMachineAlgorithmUnitTests
{
    /// <summary>
    /// The dataset is IRIS, including 100 data samples, which is divided into 2 parts:
    /// Training of the model and Testing of the model.
    /// The training data has 90 data samples saved in Samples\IrisData\iris.csv file.
    /// The testing data set has 10 data samples and it is loaded directly in the code.
    /// Each data sample has 4 features ( sepal_lenght, sepal_width, petal_width, petal_length ) and 1 label ( setosa or versicolor )
    /// The context is loaded from Samples\IrisData\iris_mapper.json, including information of features 
    /// Expected accuracy : 100% (because there are not so much data samples, only 100 data samples including 4 features in total)
    /// Predicted result: '1' is setosa, '-1' is versicolor
    /// Estimated training time: 1-2 seconds
    /// </summary>
    [TestClass]    
    public class Example
    {
        private const string IrisTrainingData = @"Samples\IrisData\iris.csv";

        /// <summary>
        /// This method is used for testing SVMAlgorithm with Iris data set
        /// </summary>
        [TestMethod]
        public void SmallIrisDataSetTest() 
        {
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), IrisTrainingData);

            LearningApi api = new LearningApi(Helpers.GetDescriptor());
            
            // Convert data in csv file into double[][]
            api.UseCsvDataProvider(path, ',', true, 0);

            // Convert data from string to double according to the context
            api.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
            {
                List<double[]> newData = new List<double[]>();
                foreach (var item in data)
                {
                    List<double> row = new List<double>();
                    for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
                    {
                        if (double.TryParse((string)item[i], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out double converted))
                        {
                            row.Add(converted);
                        }
                        else
                        {
                            throw new System.Exception("Column is not convertable to double.");
                        }
                    }

                    switch (item[ctx.DataDescriptor.LabelIndex])
                    {
                        case "setosa":
                            row.Add(1);
                            break;
                        case "versicolor":
                            row.Add(-1);
                            break;
                    }

                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });

            // Set the regularization parameter and the tolerance value 
            api.UseSVMAlgorithm(0.5);

            // Training
            SVMScore score = api.Run() as SVMScore;

            // Testing data preparation
            double[][] predictingData = new double[10][];

            predictingData[0] = new double[] { 4.6, 3.2, 1.4, 0.2 };
            predictingData[1] = new double[] { 5.3, 3.7, 1.5, 0.2 };
            predictingData[2] = new double[] { 5.0, 3.3, 1.4, 0.2 };
            predictingData[3] = new double[] { 7.0, 3.2, 4.7, 1.4 };
            predictingData[4] = new double[] { 6.4, 3.2, 4.5, 1.5 };
            predictingData[5] = new double[] { 4.4, 2.9, 1.4, 0.2 };
            predictingData[6] = new double[] { 5.7, 3.8, 1.7, 0.3 };
            predictingData[7] = new double[] { 6.7, 3.0, 5.0, 1.7 };
            predictingData[8] = new double[] { 6.3, 2.3, 4.4, 1.3 };
            predictingData[9] = new double[] { 5.7, 2.8, 4.1, 1.3 };

            // Predicting
            SVMResult result = api.Algorithm.Predict(predictingData, api.Context) as SVMResult;

            // Actual value is used to compare with predicted value. 1 for 'setosa', -1 for 'versicolor'
            int[] actualValue = new int[] { 1, 1, 1, -1, -1, 1, 1, -1, -1, -1 };

            // Count the number correct answer of prediction
            double checkAccuracy = 0;

            for (int i = 0; i < predictingData.GetLength(0); i++)
            {
                if (result.PredictedResult[i] == actualValue[i])
                {
                    checkAccuracy++;
                }
                else
                {
                    continue;
                }
            }
            // Find the accuracy in percentage
            double accuracy = checkAccuracy / predictingData.GetLength(0) * 100;

            // Show the accuracy in percent and predicted result in output window
            Console.WriteLine("The accuracy is {0} %", accuracy);
            foreach (var item in result.PredictedResult)
            {
                Console.WriteLine(item);
            }
        }
    }

    /// <summary>
    /// The dataset is IRIS, including 100 data samples, which is divided into 2 parts:
    /// Training of the model and Testing of the model.
    /// The training data has 90 data samples saved in Samples\IrisData\iris.csv file.
    /// The 10 testing data samples is loaded from Samples\IrisData\iris_predict.csv file
    /// Each data sample has 4 features ( sepal_lenght, sepal_width, petal_width, petal_length ) and 1 label ( setosa or versicolor )
    /// The context is loaded from Samples\IrisData\iris_mapper.json, including information of features 
    /// Expected accuracy : 100% (because there are not so much data samples, only 100 data samples including 4 features in total)
    /// Predicted result: '1' is setosa, '-1' is versicolor
    /// Estimated training time: 1-2 seconds
    /// </summary>
    [TestClass]
    public class SmallIrisDataSetTest2
    {
        private const string IrisTrainingData = @"Samples\IrisData\iris.csv";
        private const string IrisTestingData = @"Samples\IrisData\iris_predict.csv";

        /// <summary>
        /// This method is used for testing SVMAlgorithm with Iris data set
        /// </summary>
        [TestMethod]
        public void Test_IrisData()
        {
            var inputPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), IrisTrainingData);

            LearningApi api = new LearningApi(Helpers.GetDescriptor());

            api.UseCsvDataProvider(inputPath, ',', true, 0);

            api.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
            {
                List<double[]> newData = new List<double[]>();
                foreach (var item in data)
                {
                    List<double> row = new List<double>();
                    for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
                    {

                        if (double.TryParse((string)item[i], out double converted))
                        {
                            row.Add(converted);
                        }
                        else
                        {
                            throw new System.Exception("Column is not convertable to double.");
                        }
                    }

                    switch (item[ctx.DataDescriptor.LabelIndex])
                    {
                        case "setosa":
                            row.Add(1);
                            break;
                        case "versicolor":
                            row.Add(-1);
                            break;
                    }

                    newData.Add (row.ToArray());
                }
                return newData.ToArray();
            });

            // Set the regularization parameter and the tolerance value.
            // In this data set, it has 4 features. Therefore the regularization parameter should be low value to reduce the training time.
            // The regularization parameter is set to 0.1, and the tolerance value is set to default value (0.01).
            api.UseSVMAlgorithm(0.1);

            // Training
            SVMScore score = api.Run() as SVMScore;

            // Testing data preparation
            string fileName = IrisTestingData;

            string[] lines = File.ReadAllLines(fileName);
            double[][] predictedArray = new double[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] strArray = lines[i].Split(',');
                double[] doubleArray = Array.ConvertAll(strArray, double.Parse);
                predictedArray[i] = doubleArray;
            }
            // Predicting
            SVMResult result = api.Algorithm.Predict(predictedArray, api.Context) as SVMResult;

            // Actual value is used to compare with predicted value. 1 for 'setosa', -1 for 'versicolor'
            int[] actualValue = new int[] { 1, 1, 1, 1, 1, -1, -1, -1, -1, -1 };
            
            // Count the number correct answer of prediction
            double checkAccuracy = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (result.PredictedResult[i] == actualValue[i])
                {
                    checkAccuracy++;
                }
                else
                {
                    continue;
                }
            }

            // Show the accuracy in percent and predicted result in output window
            double accuracy = checkAccuracy/lines.Length * 100;
            Console.WriteLine("The accuracy is {0} %", accuracy);
            foreach (var item in result.PredictedResult)
            {
                Console.WriteLine(item);
            }
        }
    }

    /// <summary>
    /// The dataset is Breast Cancer Data Set, including 500 data samples, which is divided into 2 parts:
    /// Training of the model and Testing of the model.
    /// The training data has 400 data samples saved in Samples\BreastCancer\breast_cancer_own.csv file.
    /// The 100 testing data samples is loaded from Samples\BreastCancer\breast_cancer_predict_own.csv file
    /// Each data sample has 3 features ( BMI, Glucose, Insulin ) and 1 label ( class1 or class2 )
    /// The context is loaded from Samples\BreastCancer\breast_cancer_mapper_own.json, including information of features 
    /// Expected accuracy : 100% (because there are only 3 features for each data sample)
    /// Predicted result: '1' is class1, '-1' is class2
    /// Estimated training time: 1 minute
    /// </summary>
    [TestClass]
    public class BreastCancerDataSetTest
    {
        private const string BreastCancerTrainingSet = @"Samples\BreastCancer\breast_cancer_own.csv";
        private const string BreastCancerTestingSet = @"Samples\BreastCancer\breast_cancer _predict_own.csv";

        /// <summary>
        /// This method is used for testing SVMAlgorithm with Breast cancer data set
        /// </summary>
        [TestMethod]
        public void Test_BreastCancerData()
        {

            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), BreastCancerTrainingSet);

            LearningApi api = new LearningApi(Helpers.GetDescriptor("samples/BreastCancer/breast_cancer_mapper_own.json"));

            api.UseCsvDataProvider(path, ',', true, 0);

            // Convert data in csv file from string to double
            api.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
            {
                List<double[]> newData = new List<double[]>();
                foreach (var item in data)
                {
                    List<double> row = new List<double>();
                    for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
                    {
                        if (double.TryParse((string)item[i], out double converted))
                        {
                            row.Add(converted);
                        }

                        else
                        {
                            throw new System.Exception("Column is not convertable to double.");
                        }
                    }

                    switch (item[ctx.DataDescriptor.LabelIndex])
                    {
                        case "1":
                            row.Add(1);
                            break;
                        case "2":
                            row.Add(-1);
                            break;
                    }

                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });

            // Set the regularization parameter and the tolerance value.
            // In this data set, it has 3 features. Therefore the regularization parameter should be low value to reduce the training time.
            // In this case, the regularization parameter is set to 1. The tolerance value is set to default value (0.01).
            api.UseSVMAlgorithm(1);

            // Training
            SVMScore score = api.Run() as SVMScore;

            // Testing data preparation
            string fileName = BreastCancerTestingSet;

            string[] lines = File.ReadAllLines(fileName);
            double[][] predictedArray = new double[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] strArray = lines[i].Split(',');
                double[] doubleArray = Array.ConvertAll(strArray, double.Parse);
                predictedArray[i] = doubleArray;
            }

            // Predicting
            SVMResult result = api.Algorithm.Predict(predictedArray, api.Context) as SVMResult;

            // Actual value is used to compare with predicted value
            int[] actualValue = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                -1, -1, -1, -1, -1,-1, -1, -1, -1, -1,-1, -1, -1, -1, -1,-1, -1, -1, -1, -1,-1, -1, -1, -1, -1,-1, -1, -1, -1, -1,-1, -1, -1, -1, -1,-1, -1, -1, -1, -1,-1, -1, -1, -1, -1,-1, -1, -1, -1, -1 };

            // Count the number correct answer of prediction
            double checkAccuracy = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (result.PredictedResult[i] == actualValue[i])
                {
                    checkAccuracy++;
                }
                else
                {
                    continue;
                }
            }

            // Show the accuracy in percent and predicted result in output window
            double accuracy = checkAccuracy / lines.Length * 100;
            Console.WriteLine("The accuracy is {0} %", accuracy);
            foreach (var item in result.PredictedResult)
            {
                Console.WriteLine(item);
            }
        }
    }

    /// <summary>
    /// The dataset is Ionosphere Data Set, including 351 data samples, which is divided into 2 parts:
    /// Training of the model and Testing of the model.
    /// The training data has 280 data samples saved in Samples\Ionosphere\ionosphere.csv file.
    /// The 71 testing data samples is loaded from Samples\Ionosphere\ionosphere_predict.csv file
    /// Each data sample has 34 features ( Feature1, Feature2,...,Feature34 ), which are 17 pairs of measurement value and 1 label ( good or bad )
    /// The context is loaded from Samples\Ionosphere\ionosphere_mapper.json, including information of features 
    /// Expected accuracy : around 93% (because there are a lot of data samples (351) and in each data sample, it has 34 features. It will be hard to classify all data with 100% correctly. )
    /// Predicted result: '1' is good, '-1' is bad
    /// Estimated training time: 106-110 minute
    /// </summary>
    [TestClass]
    public class IonosphereDataSetTest
    {
        private const string IonosphereTrainingData = @"Samples\Ionosphere\ionosphere.csv";
        private const string IonosphereTestingData = @"Samples\Ionosphere\ionosphere_predict.csv";
        /// <summary>
        /// This method is used for testing SVMAlgorithm with Ionosphere data set
        /// </summary>
        [TestMethod]
        public void Test_IonosphereData()
        {

            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), IonosphereTrainingData);

            LearningApi api = new LearningApi(Helpers.GetDescriptor("samples/Ionosphere/ionosphere_mapper.json"));

            api.UseCsvDataProvider(path, ',', true, 0);

            // Convert data in csv file from string to double
            api.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
            {
                List<double[]> newData = new List<double[]>();
                foreach (var item in data)
                {
                    List<double> row = new List<double>();
                    for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
                    {
                        if (double.TryParse((string)item[i], out double converted))
                        {
                            row.Add(converted);
                        }

                        else
                        {
                            throw new System.Exception("Column is not convertable to double.");
                        }
                    }

                    switch (item[ctx.DataDescriptor.LabelIndex])
                    {
                        case "g":
                            row.Add(1);
                            break;
                        case "b":
                            row.Add(-1);
                            break;
                    }

                    newData.Add(row.ToArray());
                }
                return newData.ToArray();
            });

            // Set the regularization parameter and the tolerance value.
            // These data samples are non-linear seperable data, therefore the regularization parameter should be high value to avoid the misclassifying class when training data.
            // In this case, the regularization parameter is set to 100. The tolerance value is set to default value (0.01).
            api.UseSVMAlgorithm(100);

            // Training
            SVMScore score = api.Run() as SVMScore;

            // Testing data preparation
            string fileName = IonosphereTestingData;

            string[] lines = File.ReadAllLines(fileName);
            double[][] predictedArray = new double[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] strArray = lines[i].Split(',');
                double[] doubleArray = Array.ConvertAll(strArray, double.Parse);
                predictedArray[i] = doubleArray;
            }

            // Predicting
            SVMResult result = api.Algorithm.Predict(predictedArray, api.Context) as SVMResult;
            
            // Actual value is used to compare with predicted value. 1 is 'good', -1 is 'bad'
            int[] actualValue = new int[] { -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};

            // Count the number correct answer of prediction
            double checkAccuracy = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (result.PredictedResult[i] == actualValue[i])
                {
                    checkAccuracy++;
                }
                else
                {
                    continue;
                }
            }

            // Show the accuracy in percent and predicted result in output window
            double accuracy = checkAccuracy / lines.Length * 100;
            Console.WriteLine("The accuracy is {0} %", accuracy);
            foreach (var item in result.PredictedResult)
            {
                Console.WriteLine(item);
            }
        }
    }
}
