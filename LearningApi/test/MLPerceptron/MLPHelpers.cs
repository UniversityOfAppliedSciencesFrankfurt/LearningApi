using System;
using System.Collections.Generic;
using System.Text;

namespace test.MLPerceptron
{
    internal static class MLPHelpers
    {
        public static float GetAccuracy(double[][] testData, double[] result, int numberOfOutputs)
        {
            int matches = 0;

            // Check if the test data has been correctly classified by the neural network
            for (int i = 0; i < testData.Length; i++)
            {
                for (int j = 0; j < numberOfOutputs; j++)
                {
                    if (testData[i][(testData[i].Length - numberOfOutputs) + j] == (result[i * numberOfOutputs + j] >= 0.5 ? 1 : 0))
                        matches++;
                }
            }

            return (float)matches / (float)numberOfOutputs / (float)testData.Length; ;
        }
    }
}
