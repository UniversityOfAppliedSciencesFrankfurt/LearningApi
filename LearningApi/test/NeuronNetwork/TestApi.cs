using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningFoundation;
using NeuralNetworks.Core.ActivationFunctions;
using NeuralNetworks.Core.Networks;
using NeuralNetworks.Core.Neurons;

namespace test.NeuronNetwork
{
    internal class TestApi
    {
        #region member variables

        private const double m_Momentum = 0.01;
        private ActivationNetwork m_NeuralNetwork;
        //private ISupervisedLearning m_SupervisedLearning;//eForgeLearningAlgor.
        private IUnsupervisedLearning m_UnspervisedLearning;
        //private LearningMethod? m_TypeOfLearningMethod = null;
        private int m_NeuronCount;
        private int m_NeuronCountOfSecondLayer;
        private int m_FeatureCount;
        private int m_MaxNumberOfIterations = 1000;
        private int m_TotalPosImg;
        private int m_TotalNegImg;
        private double m_learningRate;
        private List<double[]> m_DataList = new List<double[]>();
        private List<double[]> m_DesiredOutputList = new List<double[]>();
        private PreProcessing m_PProcessing = new PreProcessing();
        private IFeatureProvider m_FeatureProvider;
        private double m_PositiveValue;
        private double m_NegativeValue;
        private string m_PositiveWeightFile = @"positiveWeights.txt";
        private string m_FinalWeightFile = @"finalWeight.txt";
        private string m_PosNegPosWeightFile = @"posNegPosWeight.txt";
        private string m_PosNegNegWeightFile = @"posNegNegWeight.txt";
        private double m_PosThreshold;
        private double m_FinalThreshold;
        private int[] m_NeuronsPerLayer;
        #endregion

        #region public methods

        /// <summary>
        /// API's constructor
        /// </summary>
        /// <param name="features">The number of inputs</param>
        /// <param name="neurons">The number of neurons</param>
        /// <param name="activationFunction">Activation function : threshold function(as default), sigmoid function, or bipolarsigmoid function</param>
        /// <param name="typeOfLearningMethod"> 1: perceptron learning , 2: backpropagation learning</param>
        /// <remarks>Initialize networks, learning method, activation function </remarks>
        /// <see cref="AForge.Neuro"/>
        public TestApi (int features, int neurons, double learningRate, IActivationFunction activationFunction = null, LearningMethod? typeOfLearningMethod = null, IFeatureProvider featureProvider = null)
        {
            m_FeatureProvider = featureProvider;

            //
            // initialize network variables : number of neurons(m_NeuronCount), number of inputs(m_FeatureCount), learningRate ...
            m_TotalNegImg = 0;
            m_TotalPosImg = 0;
            m_NeuronCount = neurons;
            m_FeatureCount = features;
            m_learningRate = learningRate;
            m_TypeOfLearningMethod = typeOfLearningMethod;
            //
            // intialize activation function.
            if (activationFunction == null)
                activationFunction = new ThresholdFunction();
            initializeNetwork(activationFunction);

            //
            //initialize learning method : perceptron or backpropagation
            initializeLearningMethod();
            // setup inner variables like weights,threshold
            m_NeuralNetwork.Randomize();
        }

        /// <summary>
        /// Start Perceptron or back propagation training with 2d double array .
        /// </summary>
        /// <param name="input">Input which is number of features of image : in this case , feature is pixel.</param>
        /// <param name="output" >Output is desired value or target value. </param>
        /// <remarks>This method starts Perceptron or Backpropation training with one layer.</remarks>
        /// <see cref="AForge.Neuro.Learning"/> 
        public void Learn(double[][] input, double[][] output)
        {
            //currently only supervisedLearning is supported.
            runSupervisedTraining(input, output);
        }

        /// <summary>
        /// temporary learning method for backparopagation
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="desiredOutput"></param>
        /// <param name="isTheLastFile"></param>
        /// <param name="widthFactor"></param>
        /// <param name="heightFactor"></param>
        /// <param name="traceBinDataFn"></param>
      
        public void PleaseLearn(object dataSource, bool desiredOutput, Action<int, int, double[]> traceBinDataFn = null)
        {
            var data = m_FeatureProvider.GetData(dataSource);
            //Learn(data, desiredOutput);
        }

      
        /// <summary>
        /// compute Output from network
        /// </summary>
        /// <param name="testInput">Test data includes input and desired output </param>
        /// <remarks>This method computes the output after putting the input data to network. </remarks>
        /// <see cref="AForge.Neuro.Network"/>
        /// <returns>Test output</returns>
        public double[] ComputeOutput(double[] testInput)
        {
            // m_NeuralNetwork = setSpecificWeightsForNetWork(m_NeuralNetwork);
            return m_NeuralNetwork.Compute(testInput);
            // return m_NeuralNetwork.Compute(testInput);
        }

        /// <summary>
        /// compute Output from network
        /// </summary>
        /// <param name="file">File which is the image file </param>
        /// <param name="traceBinDataFn">Display data in console window</param>
        /// <remarks>This method calls other TestNetwork with bitmap converter</remarks>
        /// <returns>Test output</returns>
     

        /// <summary>
        /// compute Output from network
        /// </summary>
        /// <param name="bmp">Bitmap of the image file</param>
        /// <param name="traceBinDataFn">Display data in console window</param>
        /// <remarks> This method binarize bitmap file to 1 and 0,then push them to testNetwork()</remarks>
        /// <exception cref="ArgumentNullException">output result cannot be null</exception>
        /// <returns>Test output</returns>
      
        public void SetSpecificWeightsForNetWork(double[][][] bestWeights = null)
        {
            string str;
            List<string> weights = new List<string>();
            //
            //FOR MULTILAYERS
            if (bestWeights != null)
            {
                for (var i = 0; i < m_NeuralNetwork.Layers.Length; i++)
                {
                    for (var j = 0; j < m_NeuralNetwork.Layers[i].Neurons.Length; j++)
                    {
                        for (var k = 0; k < m_NeuralNetwork.Layers[i].Neurons[j].Weights.Length; k++)
                        {
                            m_NeuralNetwork.Layers[i].Neurons[j].Weights[k] = bestWeights[i][j][k];
                        }
                    }
                }
            }
            else
            {
                //
                // ONLY for single perceptron : 1 neuron , 1layer 
                StreamReader reader = File.OpenText(/*m_PositiveWeightFile*//*m_PosNegPosWeightFile*//*m_PosNegNegWeightFile*/m_FinalWeightFile);

                while ((str = reader.ReadLine()) != null)
                {
                    weights.Add(str);
                }
                reader.Close();

                for (var i = 0; i < m_NeuralNetwork.Layers[0].Neurons[0].Weights.Length; i++)
                {
                    m_NeuralNetwork.Layers[0].Neurons[0].Weights[i] = Convert.ToDouble(weights[i]);
                }
            }



        }
        #endregion

        #region private methods



        /// <summary>
        /// Initialize network
        /// </summary>
        /// <param name="activationFunction">Activation function can be threshold function , sigmoid function ,or bipolarsigmoid function</param>
        /// <param name="typeOfLearningMethod"> typeOfLearningMethod can be null, 1, 2 , if typeOfLearningMethod == null then , typeOfLearningMethod= 1 as default </param>
        /// <remarks> this method creates an instance of neural network with 1 (for perceptron) or 2 (for back-propagation) layers</remarks>
        private void initializeNetwork(IActivationFunction activationFunction)
        {
            //
            //check whether network is used for backpropagation learning or perceptron learning.
            switch (m_TypeOfLearningMethod)
            {
                //if case BackPropation : start runBackPropagationTraining()
                case LearningMethod.BackPropagation:
                    //set number of neuron in the second layer, note : m_NeuronCount is the number of neuron in the 1st layer.
                    m_NeuronCountOfSecondLayer = 2;

                    //m_NeuronsPerLayer indicates the number of layers and the number of neurons per layer , ex : in the case below there are 2 layers, 
                    //and m_NeuronCount represents for the number of neurons in hidden layer ,and the m_NeuronCountOfSecondLayer represents the number 
                    //of the neurons in the output layer.
                    m_NeuronsPerLayer = new int[] { m_NeuronCount, m_NeuronCountOfSecondLayer };
                    //create a network with properties : activation function, featureCount(inputCount),m_NeuronsPerlayer(if needed).
                    m_NeuralNetwork = new ActivationNetwork(activationFunction, m_FeatureCount /*2*//*3*/, m_NeuronsPerLayer);
                    break;
                //if case Perceptron : start runPerceptronTraining()
                case LearningMethod.Perceptron:
                    m_NeuronsPerLayer = new int[] { m_NeuronCount };
                    //create a network.
                    //create a network with properties : activation function, featureCount(inputCount), neuronCount, neuronCountOfSencondLayer(if needed).
                    m_NeuralNetwork = new ActivationNetwork(activationFunction, m_FeatureCount, m_NeuronsPerLayer);
                    break;
                //default: runPerceptronTraining() (for currently case)
                default:
                    m_NeuronsPerLayer = new int[] { m_NeuronCount };
                    //create a network.
                    m_NeuralNetwork = new ActivationNetwork(activationFunction, m_FeatureCount, m_NeuronsPerLayer);
                    break;
            }
        }

        /// <summary>
        /// Initialize learning method : perceptron learning method or back propagation learning method.
        /// </summary>
        /// <param name="typeOfLearningMethod">typeOfLearningMethod can be null, 1, 2 , if typeOfLearningMethod == null then , typeOfLearningMethod= 1 as default </param>
        /// <param name="perceptronLearningMethod">perceptronLearningMethod is 1</param>
        /// <param name="backPropagationLearningMethod">backPropagationLearningMethod is 2</param>
        /// <see cref="PerceptronLearning.LearningRate" />
        /// <seealso cref="BackPropagationLearning.LearningRate"/>
        /// <seealso cref="BackPropagationLearning.Momentum"/>
        private void initializeLearningMethod()
        {
            //switch (m_TypeOfLearningMethod)
            //{
                //if case Perceptron : start runPerceptronTraining()
                //case LearningMethod.Perceptron:
                //    m_PositiveValue = 1;
                //    m_NegativeValue = 0;
                //    // create new perceptron learning instance
                //    m_SupervisedLearning = new PerceptronLearning(m_NeuralNetwork);
                //    //set learningrate
                //    ((PerceptronLearning)m_SupervisedLearning).LearningRate = m_learningRate;
                //    break;

                ////if case BackPropation : start runBackPropagationTraining()
                //case LearningMethod.BackPropagation:
                //    m_PositiveValue = 1;
                //    m_NegativeValue = -1;
                //    //create new back-propagation learning instance
                //    m_SupervisedLearning = new BackPropagationLearning(m_NeuralNetwork);

                //    //set learningrate
                //    ((BackPropagationLearning)m_SupervisedLearning).LearningRate = m_learningRate;

                //    //set momentum
                //    ((BackPropagationLearning)m_SupervisedLearning).Momentum = m_Momentum;
                //    break;

                //if case Perceptron : start runPerceptronTraining()
                //default:
                //    throw new NotSupportedException("Please specify 'LearningMethod'");
                //    break;
            }
        }

        /// <summary>
        /// stop the loop if error=0, or newDelError > double.MaxValue(only happen if error increases over time), or iteration>1000
        /// </summary>
        /// <param name="error"></param>
        /// <param name="iteration"></param>
        /// <returns></returns>
        private bool shouldExitTest(double error, int iteration)
        {
            if (error == 0 || iteration > m_MaxNumberOfIterations)
                return true;
            else
                return false;
        }

        /// <summary>
        /// run perceptron training algorithm , and used for a bunch of data or images
        /// </summary>
        /// <param name="input">input which is number of features of image : in this case , feature is pixel</param>
        /// <param name="output" >output is desired value or target value  </param>
        /// <see cref="AForge.Neuro.Learning.PerceptronLearning.RunEpoch(double[][], double[][])"/> 
        private void runSupervisedTraining(double[][] input, double[][] output)
        {
            int iteration = 0;

            //for (var i = 0; i < output.Length; i++)
            //{
            //    if (output[i][0] == 1)
            //    {
            //        supervisedRunAlgorithm(input[i], output[i], iteration);
            //    }             
            //}

            //string[] posWeights = getWeightArray();
            //  m_PosThreshold = getThreshold();

            //System.IO.File.WriteAllLines(m_PositiveWeightFile, posWeights);

            //for (var i = 0; i < output.Length; i++)
            //{
            //    //if (output[i][0] == 0)
            //    //{
            //    implementSupervisedRun(input[i], output[i], iteration);
            //    //}
            //}
            implementSupervisedRunEpoch(input, output, iteration);
            //    string[] finalWeights = GetWeightArray();
            // m_FinalThreshold = getThreshold();
            //    System.IO.File.WriteAllLines(m_FinalWeightFile, finalWeights);


            //for (var i = 0; i < output.Length; i++)
            //{
            //    if (output[i][0] == 0)
            //    {
            //        supervisedRunAlgorithm(input[i], output[i], iteration);
            //    }
            //}

            //string[] posNegNegWeights = getWeightArray();
            //System.IO.File.WriteAllLines(m_PosNegNegWeightFile, posNegNegWeights);
            //runEpochAlgorithm(input, output, iteration);
        }

        private double getThreshold()
        {
            ActivationNeuron aN = m_NeuralNetwork.Layers[0].Neurons[0] as ActivationNeuron;
            double threshold = aN.Threshold;
            return threshold;
        }



        /// <summary>
        /// get the array of weights from the neural network.
        /// </summary>
        /// <returns>the weight's array</returns>
        public double[][][] GetWeightArray()
        {
            double[][][] weights = new double[m_NeuralNetwork.Layers.Length][][];
            for (var i = 0; i < m_NeuralNetwork.Layers.Length; i++)
            {
                weights[i] = new double[m_NeuralNetwork.Layers[i].Neurons.Length][];
                for (var j = 0; j < m_NeuralNetwork.Layers[i].Neurons.Length; j++)
                {
                    weights[i][j] = new double[m_NeuralNetwork.Layers[i].Neurons[j].Weights.Length];
                    for (var k = 0; k < m_NeuralNetwork.Layers[i].Neurons[j].Weights.Length; k++)
                    {

                        weights[i][j][k] = m_NeuralNetwork.Layers[i].Neurons[j].Weights[k];

                    }
                }
            }

            //  string[] weightsArr = Array.ConvertAll<double[][], string>(weights, Convert.ToString);
            return weights;
        }

        /// <summary>
        /// implementing the run() method in ISupervisedLearning <see cref="ISupervisedLearning"/>
        /// </summary>
        /// <param name="input">input data </param>
        /// <param name="output">target ouput data</param>
        /// <param name="iteration">iteration</param>
        private void implementSupervisedRun(double[] input, double[] output, int iteration)
        {
            while (true)
            {
                // calculate error 
                double error = m_SupervisedLearning.Run(input, output);


                // stop if no error or the iteration > minIteration
                // if the activation function is threshold then error is 0 or 1 then no need the boundary to round up the error to a certain value.
                if (shouldExitTest(error, iteration))
                {
                    // write  the error and iteration in debug window
                    Debug.WriteLine("{0} {1}", error, iteration);
                    break;
                }

                // write  the error and iteration in debug window    
                Debug.WriteLine("{0} {1}", error, iteration);
                iteration++;
            }
        }

        /// <summary>
        /// Implementing the RunEpoch() method in ISupervisedLearning <see cref="ISupervisedLearning"/>
        /// </summary>
        /// <param name="input">input data</param>
        /// <param name="output">target output data</param>
        /// <param name="iteration"> iteration </param>
        private void implementSupervisedRunEpoch(double[][] input, double[][] output, int iteration)
        {
            double lowerLimit = 0;
            double upperLimit = 0.1;
            double lastErr = 0;
            double limitDifference = 0.001;
            while (true)
            {
                // calculate error 
                double error = m_SupervisedLearning.RunEpoch(input, output);
                if (lastErr != 0 && Math.Abs(error - lastErr) < limitDifference)
                {
                    error = lastErr;
                }

                lastErr = error;

                if (error < upperLimit && error > lowerLimit)
                {
                    error = 0;
                }
                // stop if no error or the iteration > minIteration
                // if the activation function is threshold then error is 0 or 1 then no need the boundary to round up the error to a certain value.
                if (shouldExitTest(error, iteration))
                {
                    // write  the error and iteration in debug window
                    Debug.WriteLine("{0} {1}", error, iteration);
                    break;
                }

                // write  the error and iteration in debug window    
                Debug.WriteLine("{0} {1}", error, iteration);
                iteration++;
            }
        }



        /// <summary>
        /// compute output by putting the testdata to network
        /// </summary>
        /// <param name="testingData">testingData is from single file/image</param>
        /// <see cref="AForge.Neuro.Network.Compute(double[])"/>
        /// <exception cref="ArgumentNullException">listOutPutVector</exception>
        /// <returns>test output</returns>
        private double[] computeOutput(double[] testingData)
        {
            try
            {
                double[] listOutPutVector = null;
                //compute output in double[].
                listOutPutVector = m_NeuralNetwork.Compute(testingData);

                return listOutPutVector;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //private INetworkSerializer getSerializer()
        //{
        //    return new MySerializer();
        //} 
        #endregion
    }
}
