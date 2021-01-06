using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace DigitRecognizer
{
    /// <summary>Class of NeuroOCR with initialization and training of Neural Network.
    /// 
    /// </summary>
    ///
    public class NeuroOCR : IAlgorithm
    {
        AForge.Neuro.ActivationNetwork network; // Neural Network
        AForge.Neuro.Learning.PerceptronLearning learning; // Learning mechanism

        /// <summary> The desired input data is given.
        /// 
        /// </summary>
        /// <param name="function">Activation function for network, in this example BipolarSigmoid</param>
        /// <param name="inputsCount">number of input neuron, in this case 784</param>
        /// <param name="neuronCount">number of neurons in output layers, in this case 10</param>
        /// <param name="learningRate">Learning rate of the network, in this case 1</param>
        public NeuroOCR(AForge.Neuro.IActivationFunction function, int inputsCount, int neuronCount, double learningRate)
        {

            network = new AForge.Neuro.ActivationNetwork(function, inputsCount, neuronCount); // initialization of network

            network.Randomize(); // randomize weigths
            learning = new AForge.Neuro.Learning.PerceptronLearning(network); // initialization of learning mechanism
            learning.LearningRate = learningRate;


        }

        /// <summary>
        /// This method accepts data as input and predict an output i.e guess written character between 0 and 9
        /// </summary>
        /// <param name="data">input</param>
        /// <param name="ctx">test data descriptions</param>
        /// <returns>double[]</returns>
        public IResult Predict(double[][] data, IContext ctx)
        {
            NeuroOCRResult result = new NeuroOCRResult(); // Result

            // Convert input data to one dimension array of -1 and 1
            double[] input = new double[network.InputsCount];
            for (int i = 1; i < data[0].Length; i++)
            {
                if (data[0][i] > 0)
                {
                    input[i - 1] = 1;
                }
                else
                {
                    input[i - 1] = -1;
                }

            }
            var output = network.Compute(input); // compute the output
            var max = output[0];
            var maxIndex = 0;
            for (int i = 0; i < output.Length; i++)
            {

                if (output[i] > max)
                {
                    max = output[i];
                    maxIndex = i;
                }
            }
            result.Result = maxIndex;
            return result;

        }





        /// The data is given in form of a array with a pattern
        /// 
        /// 
        /// <param name="data">list of data</param>
        /// <param name="ctx">context</param>
        /// <returns>returns the IScore (Error and iterations)</returns>
        /// 
        public IScore Run(double[][] data, IContext ctx)
        {
            NeuroOCRScore score = new NeuroOCRScore(); // Score of training


            double[][] input = new double[data.Length][]; // training input array
            double[][] output = new double[data.Length][]; // training output array
            for (int i = 0; i < data.Length; i++)
            {
                input[i] = new double[network.InputsCount]; // 784 input in each pattern
                output[i] = new double[10]; // 10 output pattern i.e 0-9

                // fill the output with -1 by default
                for (int j = 0; j < output[i].Length; j++)
                {
                    output[i][j] = -1;
                }
            }

            // fill input and output array with 1 and -1 accordingly
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 1; j < data[i].Length; j++)
                {
                    if (data[i][j] > 0)
                    {
                        input[i][j - 1] = 1;
                    }
                    else
                    {
                        input[i][j - 1] = -1;
                    }

                }

                var result = (int)data[i][0];
                output[i][result] = 1;
            }


            bool needToStop = false;
            int iteration = 0;
            List<double> errorList = new List<double>();
            while (!needToStop)
            {
                double error = learning.RunEpoch(input, output
                    );

                if (error <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("{0} {1}", error, iteration);
                    errorList.Add(error);
                    break;
                }

                else if (iteration < 1000) // change this if you need to train network more or less
                    iteration++;
                else
                    needToStop = true;
                System.Diagnostics.Debug.WriteLine("{0} {1}", error, iteration);
                errorList.Add(error);
            }

            score.Iterations = iteration;
            score.Error = errorList.ToArray();
            return score;

        }


        /// Train the network with given data
        /// 
        /// 
        /// <param name="data">data</param>
        /// <param name="ctx">context</param>
        /// <returns>returns the IScore (Error and iterations)</returns>
        /// 
        public IScore Train(double[][] data, IContext ctx)
        {

            return Run(data, ctx);
        }


    }
}

