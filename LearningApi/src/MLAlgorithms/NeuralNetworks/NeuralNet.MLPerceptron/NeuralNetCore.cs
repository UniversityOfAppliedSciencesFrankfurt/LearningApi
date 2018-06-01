using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLPerceptron.NeuralNetworkCore
{
    /// <summary>
    /// Class NeuralNetCore inherits from interface IAlgorithm and also contains the Run method declaration to implement the learning algorithm of the neural network
    /// </summary>
    public abstract class NeuralNetCore : IAlgorithm
    {
        /// <summary>
        /// NeuralNetCore Default Constructor
        /// </summary>
        public NeuralNetCore()
        {
            //do nothing
        }

        /// <summary>
        /// This method accepts the training examples as input and performs the training of the MLP neural network
        /// </summary>
        /// <param name="featureValues">training data pairs</param>
        /// <param name="ctx">training data descriptions</param>
        /// <returns>IScore</returns>
        public abstract IScore Run(double[][] featureValues, IContext ctx);

        /// <summary>
        /// This method is used to train the neural network
        /// </summary>
        /// <param name="featureValues">training data pairs</param>
        /// <param name="ctx">training data descriptions</param>
        /// <returns>IScore</returns>
        public IScore Train(double[][] featureValues, IContext ctx)
        {
            return Run(featureValues, ctx);
        }

       

        /// <summary>
        /// This method accepts the test data as input and determines the output for each sample of test data
        /// </summary>
        /// <param name="data">test data inputs</param>
        /// <param name="ctx">test data descriptions</param>
        /// <returns>double[]</returns>

        public IResult Predict(double[][] data, IContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
