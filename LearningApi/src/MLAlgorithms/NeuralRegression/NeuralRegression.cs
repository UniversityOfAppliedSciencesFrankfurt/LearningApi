using System;
using System.Collections.Generic;
using System.IO;
using LearningFoundation;

namespace NeuralRegression
{
    /// <summary>
    /// Creates a Neural Network Regression and predicts data
    /// </summary>
    public class NeuralRegression : IAlgorithm
    {
        private int numInput; // number input nodes
        private int numHidden;
        private int numOutput;

        private double[] inputs;
        private double[] hiddens;
        private double[] outputs;

        private double[][] ihWeights; // input-hidden
        private double[] hBiases;

        private double[][] hoWeights; // hidden-output
        private double[] oBiases;

        private double[] m_Errors;

        private double m_Alpha;

        private int m_Iterations;

        private Random rnd;

        /// <summary>
        /// Constructor of NeuralRegression class
        /// </summary>
        /// <param name="learningRate">holds learning rate as a input</param>
        /// <param name="iterations">holds current iteration</param>
        public NeuralRegression(double learningRate, int iterations)
        {
            int numInput = 1;
            int numHidden = 12;
            int numOutput = 1;
            m_Alpha = learningRate;
            m_Iterations = iterations;

            this.numInput = numInput;
            this.numHidden = numHidden;
            this.numOutput = numOutput;

            this.inputs = new double[numInput];
            this.hiddens = new double[numHidden];
            this.outputs = new double[numOutput];


            this.ihWeights = MakeMatrix(numInput, numHidden, 0.0);
            this.hBiases = new double[numHidden];

            this.hoWeights = MakeMatrix(numHidden, numOutput, 0.0);
            this.oBiases = new double[numOutput];

            this.rnd = new Random(1);
            this.InitializeWeights(); // all weights and biases
        } // ctor

        /// <summary>
        /// Makes matrix 
        /// helper for ctor, Train
        /// </summary>
        /// <param name="rows">holds the inputs of rows</param>
        /// <param name="cols">holds the inputs of columns</param>
        /// <param name="v">constant value</param>
        /// <returns>matrix of a 2 dimentional double type array</returns>
        private static double[][] MakeMatrix(int rows,
          int cols, double v) 
        {
            double[][] result = new double[rows][];
            for (int r = 0; r < result.Length; ++r)
                result[r] = new double[cols];
            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result[i][j] = v;
            return result;
        }

        /// <summary>
        ///  initialize weights and biases to small random values
        /// </summary>
        private void InitializeWeights() 
        {
           
            int numWeights = (numInput * numHidden) +
              (numHidden * numOutput) + numHidden + numOutput;
            double[] initialWeights = new double[numWeights];
            double lo = -0.001;
            double hi = +0.001;
            for (int i = 0; i < initialWeights.Length; ++i)
                initialWeights[i] = (hi - lo) * rnd.NextDouble() + lo;  
            this.SetWeights(initialWeights);
        }

        /// <summary>
        /// copy serialized weights and biases in weights[] array
        /// to i-h weights, i-h biases, h-o weights, h-o biases
        /// </summary>
        /// <param name="weights">weights between neural nodes</param>
        public void SetWeights(double[] weights)
        {  
            int numWeights = (numInput * numHidden) +
              (numHidden * numOutput) + numHidden + numOutput;
            if (weights.Length != numWeights)
                throw new Exception("Bad weights array in SetWeights");

            int w = 0;

            for (int i = 0; i < numInput; ++i)
                for (int j = 0; j < numHidden; ++j)
                    ihWeights[i][j] = weights[w++];

            for (int j = 0; j < numHidden; ++j)
                hBiases[j] = weights[w++];

            for (int j = 0; j < numHidden; ++j)
                for (int k = 0; k < numOutput; ++k)
                    hoWeights[j][k] = weights[w++];

            for (int k = 0; k < numOutput; ++k)
                oBiases[k] = weights[k++];
        }

        /// <summary>
        /// Calculate weights
        /// </summary>
        /// <returns>returns weights as a double type of array</returns>
        public double[] GetWeights()
        {
            int numWeights = (numInput * numHidden) +
              (numHidden * numOutput) + numHidden + numOutput;
            double[] result = new double[numWeights];

            int w = 0;
            for (int i = 0; i < numInput; ++i)
                for (int j = 0; j < numHidden; ++j)
                    result[w++] = ihWeights[i][j];

            for (int j = 0; j < numHidden; ++j)
                result[w++] = hBiases[j];

            for (int j = 0; j < numHidden; ++j)
                for (int k = 0; k < numOutput; ++k)
                    result[w++] = hoWeights[j][k];

            for (int k = 0; k < numOutput; ++k)
                result[w++] = oBiases[k];

            return result;
        }

        /// <summary>
        /// Based on given data computes predicted value
        /// </summary>
        /// <param name="xValues">input dataset</param>
        /// <returns></returns>
        public double[] ComputeOutputs(double[] xValues)
        {
            double[] hSums = new double[numHidden]; // hidden nodes sums scratch array
            double[] oSums = new double[numOutput]; // output nodes sums

            for (int i = 0; i < numInput; ++i) // copy x-values to inputs
                this.inputs[i] = xValues[i];

            for (int j = 0; j < numHidden; ++j)  // compute i-h sum of weights * inputs
                for (int i = 0; i < numInput; ++i)
                    hSums[j] += this.inputs[i] * this.ihWeights[i][j]; // note +=

            for (int j = 0; j < numHidden; ++j)  // add biases to input-to-hidden sums
                hSums[j] += this.hBiases[j];

            for (int j = 0; j < numHidden; ++j)   // apply activation
                this.hiddens[j] = HyperTan(hSums[j]); // hard-coded

            for (int k = 0; k < numOutput; ++k)   // compute h-o sum of weights * hOutputs
                for (int j = 0; j < numHidden; ++j)
                    oSums[k] += hiddens[j] * hoWeights[j][k];

            for (int k = 0; k < numOutput; ++k)  // add biases to input-to-hidden sums
                oSums[k] += oBiases[k];

            Array.Copy(oSums, this.outputs, outputs.Length);  // copy without activation

            double[] retResult = new double[numOutput]; // could define a GetOutputs 
            Array.Copy(this.outputs, retResult, retResult.Length);
            return retResult;
        }

        /// <summary>
        ///  HyperTan is program-defined to avoid extreme values
        /// </summary>
        /// <param name="x">Input X</param>
        /// <returns></returns>
        private static double HyperTan(double x)
        {
            if (x < -20.0) return -1.0; 
            else if (x > 20.0) return 1.0;
            else return Math.Tanh(x);
        }

        /// <summary>
        /// TrainGivenData trains the neurons on provided data
        /// </summary>
        /// <param name="trainData">Training set of Data</param>
        /// <param name="maxEpochs">Maximum epoch number</param>
        /// <param name="learnRate">Leaning rate for the neural network</param>
        /// <returns></returns>
        public double[] Train(double[][] trainData,
          int maxEpochs, double learnRate, double momentum)
        {

            double[][] hoGrads = MakeMatrix(numHidden, numOutput, 0.0); // hidden-to-output weights gradients
            double[] obGrads = new double[numOutput];                   // output biases gradients

            double[][] ihGrads = MakeMatrix(numInput, numHidden, 0.0);  // input-to-hidden weights gradients
            double[] hbGrads = new double[numHidden];                   // hidden biases gradients

            double[] oSignals = new double[numOutput];                  // signals == gradients w/o associated input terms
            double[] hSignals = new double[numHidden];                  // hidden node signals

            // back-prop momentum specific arrays 
            double[][] ihPrevWeightsDelta = MakeMatrix(numInput, numHidden, 0.0);
            double[] hPrevBiasesDelta = new double[numHidden];
            double[][] hoPrevWeightsDelta = MakeMatrix(numHidden, numOutput, 0.0);
            double[] oPrevBiasesDelta = new double[numOutput];

            // train a back-prop style NN regression using learning rate and momentum
            int epoch = 0;
            double[] xValues = new double[numInput]; // inputs
            double[] tValues = new double[numOutput]; // target values

            int[] sequence = new int[trainData.Length];
            for (int i = 0; i < sequence.Length; ++i)
                sequence[i] = i;

            List<double> errorlist = new List<double>();
            int errInterval = maxEpochs / 10; // interval to check validation data
            while (epoch < maxEpochs)
            {
                ++epoch;  // immediately to prevent display when 0

                if (epoch % errInterval == 0 && epoch < maxEpochs)
                {
                    double trainErr = Error(trainData);
                    errorlist.Add(trainErr);
                }

                Shuffle(sequence); // visit each training data in random order
                for (int ii = 0; ii < trainData.Length; ++ii)
                {
                    int idx = sequence[ii];
                    Array.Copy(trainData[idx], xValues, numInput);
                    Array.Copy(trainData[idx], numInput, tValues, 0, numOutput);
                    ComputeOutputs(xValues); // copy xValues in, compute outputs 

                    // indices: i = inputs, j = hiddens, k = outputs

                    // 1. compute output nodes signals (assumes constant activation)
                    for (int k = 0; k < numOutput; ++k)
                    {
                        double derivative = 1.0; // for dummy output activation f'
                        oSignals[k] = (tValues[k] - outputs[k]) * derivative;
                    }

                    // 2. compute hidden-to-output weights gradients using output signals
                    for (int j = 0; j < numHidden; ++j)
                        for (int k = 0; k < numOutput; ++k)
                            hoGrads[j][k] = oSignals[k] * hiddens[j];

                    // 2b. compute output biases gradients using output signals
                    for (int k = 0; k < numOutput; ++k)
                        obGrads[k] = oSignals[k] * 1.0; // dummy assoc. input value

                    // 3. compute hidden nodes signals
                    for (int j = 0; j < numHidden; ++j)
                    {
                        double sum = 0.0; // need sums of output signals times hidden-to-output weights
                        for (int k = 0; k < numOutput; ++k)
                        {
                            sum += oSignals[k] * hoWeights[j][k];
                        }
                        double derivative = (1 + hiddens[j]) * (1 - hiddens[j]); // for tanh
                        hSignals[j] = sum * derivative;
                    }

                    // 4. compute input-hidden weights gradients
                    for (int i = 0; i < numInput; ++i)
                        for (int j = 0; j < numHidden; ++j)
                            ihGrads[i][j] = hSignals[j] * inputs[i];

                    // 4b. compute hidden node biases gradienys
                    for (int j = 0; j < numHidden; ++j)
                        hbGrads[j] = hSignals[j] * 1.0; // dummy 1.0 input

                    // == update weights and biases

                    // 1. update input-to-hidden weights
                    for (int i = 0; i < numInput; ++i)
                    {
                        for (int j = 0; j < numHidden; ++j)
                        {
                            double delta = ihGrads[i][j] * learnRate;
                            ihWeights[i][j] += delta;
                            ihWeights[i][j] += ihPrevWeightsDelta[i][j] * momentum;
                            ihPrevWeightsDelta[i][j] = delta; // save for next time
                        }
                    }

                    // 2. update hidden biases
                    for (int j = 0; j < numHidden; ++j)
                    {
                        double delta = hbGrads[j] * learnRate;
                        hBiases[j] += delta;
                        hBiases[j] += hPrevBiasesDelta[j] * momentum;
                        hPrevBiasesDelta[j] = delta;
                    }

                    // 3. update hidden-to-output weights
                    for (int j = 0; j < numHidden; ++j)
                    {
                        for (int k = 0; k < numOutput; ++k)
                        {
                            double delta = hoGrads[j][k] * learnRate;
                            hoWeights[j][k] += delta;
                            hoWeights[j][k] += hoPrevWeightsDelta[j][k] * momentum;
                            hoPrevWeightsDelta[j][k] = delta;
                        }
                    }

                    // 4. update output node biases
                    for (int k = 0; k < numOutput; ++k)
                    {
                        double delta = obGrads[k] * learnRate;
                        oBiases[k] += delta;
                        oBiases[k] += oPrevBiasesDelta[k] * momentum;
                        oPrevBiasesDelta[k] = delta;
                    }

                } // each training item

            } // while
            m_Errors = errorlist.ToArray();
            double[] bestWts = this.GetWeights();
            return bestWts;
        } // Train

        /// <summary>
        /// Shuffles the data
        /// </summary>
        /// <param name="sequence">Sequence number</param>
        private void Shuffle(int[] sequence) 
        {
            for (int i = 0; i < sequence.Length; ++i)
            {
                int r = this.rnd.Next(i, sequence.Length);
                int tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        } // Shuffle

        /// <summary>
        /// Calculates Error
        /// </summary>
        /// <param name="data">Training dataset</param>
        /// <returns></returns>
        private double Error(double[][] data)
        {
            // MSE == average squared error per training item
            double sumSquaredError = 0.0;
            double[] xValues = new double[numInput]; // first numInput values in trainData
            double[] tValues = new double[numOutput]; // last numOutput values

            // walk through each training case
            for (int i = 0; i < data.Length; ++i)
            {
                Array.Copy(data[i], xValues, numInput);
                Array.Copy(data[i], numInput, tValues, 0, numOutput); // get target value(s)
                double[] yValues = this.ComputeOutputs(xValues); // outputs using current weights

                for (int j = 0; j < numOutput; ++j)
                {
                    double err = tValues[j] - yValues[j];
                    sumSquaredError += err * err;
                }
            }
            return sumSquaredError / data.Length;
        } // Error

        /// <summary>
        /// Actually it just returns run function
        /// </summary>
        /// <param name="data">Actual data set</param>
        /// <param name="ctx">Icontext type parameter</param>
        /// <returns></returns>
        public IScore Train(double[][] data, IContext ctx)
        {
            return Run(data, ctx);
        }

        /// <summary>
        /// Trains up the data and returns weight,errors and accuracy
        /// </summary>
        /// <param name="data">Actual data set</param>
        /// <param name="ctx">IContext type parameter</param>
        /// <returns></returns>
        public IScore Run(double[][] data, IContext ctx)
        {
            double momentum = 0.001;
            double[] weights = Train(data, m_Iterations, m_Alpha, momentum);
         
            return new NeuralRegressionScore()
            {
                Weights = weights,
                Errors = m_Errors
            };
        }

        /// <summary>
        /// Predicts values based on given input
        /// </summary>
        /// <param name="data">Actual data set</param>
        /// <param name="ctx">IContext type parameter</param>
        /// <returns></returns>
        IResult IAlgorithm.Predict(double[][] data, IContext ctx)
        {
          
            NeuralRegressionResult result = new NeuralRegressionResult()
            {
                PredictedValues = new double[data.Length][],
            };

            for (int i = 0; i < data.Length; ++i)
            {
                var dataRow = data[i];
                double[] computed = ComputeOutputs(data[i]);
                result.PredictedValues[i] = computed;

            }


            return result;
        }
    } // class NeuralNetwork
}
