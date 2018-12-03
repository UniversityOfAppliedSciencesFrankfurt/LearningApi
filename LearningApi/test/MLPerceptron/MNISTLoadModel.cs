using LearningFoundation;
using NeuralNet.MLPerceptron;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace test.MLPerceptron
{
    public class MNISTLoadModel
    {
        /// <summary>
        /// This method loads the saved model and runs the neural network prediction algorithm on the model by feeding the test data to the network
        /// </summary>
        /// <param name="jsonFile: File name corresponding to the saved model that is to be loaded and tested"></param>
        public void LoadMNISTModel(String jsonFile)
        {
            //string moduleName = "test-action";

            int numberOfOutputNeurons = 10;

            // Loads the saved model.
            var loadedApi = LearningApi.Load(jsonFile);

            var predictedResult = ((MLPerceptronResult)loadedApi.Algorithm.Predict(MNISTFileRead.testData, loadedApi.Context)).results;

            //Create file to store the test data results
            StreamWriter resultFile = new StreamWriter($"{Directory.GetCurrentDirectory()}\\MLPerceptron\\TestFiles\\" + jsonFile + "_result.csv");

            double[] tempResultArray = new double[numberOfOutputNeurons];

            int index = 0;

            // Update the predictedResult file with the predicted results on the test dataset
            while (index < predictedResult.Length)
            {
                for (int i = index; i < index + numberOfOutputNeurons; i++)
                {
                    tempResultArray[i - index] = predictedResult[i];
                }

                double max2 = tempResultArray.Max();

                resultFile.WriteLine(Array.IndexOf(tempResultArray, max2));

                index = index + numberOfOutputNeurons;
            }

            int numberOfCorrectClassifications = 0;

            // Calculate the number of test data elements that have been correctly classified
            for (int i = 0; i < MNISTFileRead.testData.Length; i++)
            {
                numberOfCorrectClassifications++;

                for (int j = 0; j < numberOfOutputNeurons; j++)
                {
                    if (MNISTFileRead.testData[i][(MNISTFileRead.testData[i].Length - numberOfOutputNeurons) + j] != (predictedResult[i * numberOfOutputNeurons + j] >= 0.5 ? 1 : 0))
                    {
                        numberOfCorrectClassifications--;
                        break;
                    }
                }
            }

            //Calculate accuracy by using the following formula:
            // accuracy = numberOfCorrectClassifications/numberOfTestDataElements
            double accuracy = ((double)numberOfCorrectClassifications * numberOfOutputNeurons) / predictedResult.Length;
            resultFile.WriteLine("Saved Model Accuracy = {0}", accuracy.ToString());

            resultFile.Close();
        }

    }
}
