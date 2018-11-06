using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLPerceptron
{
    /// <summary>
    /// Class MyAlgorithmExtensions contains the MLPerceptron extension methods to invoke the MLPerceptronAlgorithm object from the unit test project
    /// </summary>
    public static class MyAlgorithmExtensions
    {
        /// <summary>
        /// This method is invoked from the unit test project to train the neural network on the training data
        /// </summary>
        /// <param name="api">Learning api</param>
        /// <param name="learningRate">learning rate of the network</param>
        /// <param name="iterations">Number of epochs</param>
        /// <param name="hiddenlayerneurons">Defines list of layers with number of hidden layer neurons at every layer.</param>
        /// <param name="activationFnc">activation function</param>
        /// <returns>LearningApi</returns>
        public static LearningApi UseMLPerceptron(this LearningApi api,
          double learningRate, int iterations, int batchSize, int testCaseNumber, int[] hiddenlayerneurons = null, 
               IActivationFunction activationFnc = null)

        {
            var alg = new MLPerceptronAlgorithm(learningRate, iterations, batchSize, testCaseNumber, hiddenlayerneurons);
            api.AddModule(alg, "MLPerceptronAlgorithm");
            return api;
        }
    }
}
