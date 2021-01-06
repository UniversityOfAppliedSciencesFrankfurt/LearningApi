using LearningFoundation;
//using LearningFoundation.DataProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System;
using LearningFoundation.DataProviders;
using DecisonForestRegressionAlgorithm;

namespace DecisonForestRegressionAlgUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        //private const string m_TestData = @"Samples\training-data-sample.csv";
        private const string TestData = @"Samples\Fertility.csv";

        /// <summary>
        /// this method takes data remove unwanted charectors and train the method by number of trees in result.
        /// then in prediction it takes 20% data and retunr the result based on planted trees
        /// and this result return array of predicted results.
        /// </summary>
        [TestMethod]
        public void TestMyAlgorithm()
        {

            List<double[]> newData = new List<double[]>();
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), TestData);

            LearningApi api = new LearningApi(Helpers.GetDescriptor());

            api.UseCsvDataProvider(path, ',', false, 1);

            api.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
           {
               foreach (var item in data)
               {
                   List<double> row = new List<double>();

                   for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
                   {

                       string Charvalue = (string)item[i];

                       if (i == 0)
                       {
                           string s = Charvalue.Remove(0, 1);
                           item[i] = s;

                       }
                       if (i == 9)
                       {
                           string s = Charvalue.Remove(1, 1);
                           item[i] = s;

                       }

                       if (Charvalue.Contains("/") || Charvalue.Contains(@"\"))
                       {
                           if (Charvalue.Contains(@"\"))
                           {
                               Charvalue = Charvalue.Replace(@"\", string.Empty);
                           }
                           if (Charvalue.Contains(@"/"))
                           {
                               Charvalue = Charvalue.Replace(@"/", string.Empty);
                           }

                       }


                       if (double.TryParse((string)item[i], out double converted))
                           row.Add(converted);


                       else
                           throw new System.Exception("Column is not convertable to double.");


                   }

                   newData.Add(row.ToArray());
               }
               return newData.ToArray();
           });

            api.UseDecisionForestRegressionModule();

            api.UseDecisonForestRegressionAlgorithm(0.1);

            // Training

            DecisonForestRegressionAlgorithmScore score = api.Run() as DecisonForestRegressionAlgorithmScore;
            Assert.IsNotNull(score.ScoreResult);

            double[][] predictingData = new double[1][]; // scores
            predictingData[0] = new double[2] { 4.4, 5.5 }; // input into region0 array


            // test data
            List<double[]> testdata = new List<double[]>();

            // We want to have 20%  from our data for testing.
            int percent = 20;
            int total = newData.Count;

            int Testingdata = (int)(total * percent) / 100;


            testdata = newData.GetRange(newData.Count - int.Parse(Testingdata.ToString()), int.Parse((Testingdata).ToString()));

            testdata = newData.GetRange(0, int.Parse((Testingdata).ToString()));

            DecisonForestRegressionAlgorithmResult results = api.Algorithm.Predict(testdata.ToArray(), api.Context) as DecisonForestRegressionAlgorithmResult;

            //this is prcentage of accuracy in future prediction
            double resultpercentage = Accuracymetrics(results, testdata);
            //so just chekcing that acuracy of this algorithm is more than 60%           
            Assert.IsTrue(resultpercentage > 60);

        }

        /// <summary>
        /// this method compare the predicted result with known results
        /// and calculate the percentage of accuracy
        /// </summary>
        /// <param name="predicted">the retrun of predict method result</param>
        /// <param name="actual">known result</param>
        /// <returns>percentage of accuracy</returns>
        public double Accuracymetrics(DecisonForestRegressionAlgorithmResult predicted, List<double[]> actual)
        {
            // how many correct predictions
            double correct = 0;
            //for each actual lable

            for (int i = 0; i < actual.Count; i++)
            {
                double actualdata = Math.Round(actual[i][9]);
                double predicteddata = Math.Round(predicted.Results[i]);


                int actualdataq = Convert.ToInt32(actualdata.ToString());
                int predicteddataq = Convert.ToInt32(predicteddata.ToString());
                if (actualdata == predicteddata)
                {
                    //add one to correct prediction
                    correct += 1;
                }

            }
            //Output: Diagnosis	normal (N), altered (O)	  N=1  O=2
            //return percentage of prediction that was correct
            double percentComplete = Math.Round((double)(100 * correct) / actual.Count);
            //return ((correct / actual.Count) * 100.0);
            return percentComplete;

        }
        /// <summary>
        /// this method takes only one data row
        /// evaluate the result and apply assert test on result
        /// </summary>
        [TestMethod]
        public void TestAlgorithmWithSimpleData()
        {
            List<double[]> newData = new List<double[]>();
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), TestData);

            LearningApi api = new LearningApi(Helpers.GetDescriptor());


            api.UseCsvDataProvider(path, ',', false, 1);

            api.UseActionModule<object[][], double[][]>((object[][] data, IContext ctx) =>
              {
                  foreach (var item in data)
                  {
                      List<double> row = new List<double>();
                      for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
                      {
                          string Charvalue = (string)item[i];

                          if (i == 0)
                          {
                              string s = Charvalue.Remove(0, 1);
                              item[i] = s;

                          }
                          if (i == 9)
                          {
                              string s = Charvalue.Remove(1, 1);
                              item[i] = s;

                          }

                          if (Charvalue.Contains("/") || Charvalue.Contains(@"\"))
                          {
                              if (Charvalue.Contains(@"\"))
                              {
                                  Charvalue = Charvalue.Replace(@"\", string.Empty);
                              }
                              if (Charvalue.Contains(@"/"))
                              {
                                  Charvalue = Charvalue.Replace(@"/", string.Empty);
                              }

                          }
                          if (double.TryParse((string)item[i], out double converted))
                              row.Add(converted);

                          else
                              throw new System.Exception("Column is not convertable to double.");

                      }

                      newData.Add(row.ToArray());
                  }
                  return newData.ToArray();
              });

            api.UseDecisionForestRegressionModule();

            api.UseDecisonForestRegressionAlgorithm(0.1);

            // Training

            DecisonForestRegressionAlgorithmScore score = api.Run() as DecisonForestRegressionAlgorithmScore;

            Assert.AreEqual(score.ScoreResult, 30);
            double[][] predictingData = new double[1][]; // scores
            predictingData[0] = new double[10] { -1, 0.69, 0, 1, 1, 0, 0.6, -1, 0.19, 1 }; // input into region0 array


            DecisonForestRegressionAlgorithmResult result = api.Algorithm.Predict(predictingData, api.Context) as DecisonForestRegressionAlgorithmResult;
            int myresult = Convert.ToInt32(result.Results[0]);

            Assert.AreEqual(myresult, 1);
        }

    }
}
