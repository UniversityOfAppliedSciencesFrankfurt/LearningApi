using NeuralNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using System.Diagnostics;

namespace NeuralNet.Perceptron
{
    public class PerceptronAlgorithm : NeuralNetCore
    {
        private double m_LearningRate = 0.5;

        private int m_Iterations;

        private Func<double, double> m_ActivationFunction = ActivationFunctions.Boolean;

        private int m_Dimensions;

        private double[] m_Weights;

        //private double[] m_Errors;

        private bool m_TraceTotalError;

        private double m_Threshold;

        //private bool m_PersistConvergenceData = false;

        public PerceptronAlgorithm(PerceptronAlgorithmScore score)
        {

        }

        public PerceptronAlgorithm(double threshold, double learningRate, int iterations, Func<double, double> activationFunction = null, bool traceTotalError = false)
        {
            this.m_Threshold = threshold;
            this.m_LearningRate = learningRate;
            this.m_Iterations = iterations;
            this.m_TraceTotalError = traceTotalError;

            if (activationFunction != null)
                this.m_ActivationFunction = activationFunction;
        }


        public override IScore Run(double[][] featureValues, IContext ctx)
        {
            m_Dimensions = ctx.DataDescriptor.Features.Count();
            int numOfInputVectors = featureValues.Length;

            if (m_Weights == null)
            {
                m_Weights = new double[m_Dimensions];
                initializeWeights();
            }

            //m_Errors = new double[numOfInputVectors];

            double lastError = 0;
            double totalError = 0.0;
            var score = new PerceptronAlgorithmScore();
            for (int i = 0; i < m_Iterations; i++)
            {
                double maxError = 0;

                totalError = 0;

                for (int inputVectIndx = 0; inputVectIndx < numOfInputVectors; inputVectIndx++)
                {
                    // Calculate the output value with current weights.
                    double calculatedOutput = calculateResult(featureValues[inputVectIndx], m_Dimensions);

                    // Get expected output.
                    double expectedOutput = featureValues[inputVectIndx][ctx.DataDescriptor.LabelIndex];

                    // Error is difference between calculated output and expected output.
                    double error = expectedOutput - calculatedOutput;
                
                    // Total error for all input vectors inside of a single iteration.
                    totalError += Math.Abs(error);
         
                    if (this.m_TraceTotalError)
                    {
                        if (maxError < totalError)
                            maxError = totalError;
                        else if (inputVectIndx == numOfInputVectors -1)
                            lastError = totalError;
                    }

                    if (error != 0)
                    {
                        // Y = W * X
                        // error = expectedOutput - calculatedOutput
                        // W = Y/X
                        //
                        // Updating of weights
                        for (int dimensionIndx = 0; dimensionIndx < m_Dimensions; dimensionIndx++)
                        {
                            double delta = m_LearningRate * featureValues[inputVectIndx][dimensionIndx] * error;
                            m_Weights[dimensionIndx] += delta;
                        }
                    }

                    //
                    // Updating of threshold
                    this.m_Threshold += this.m_LearningRate * error;

                    //if (totalError == 0 && i >= (m_Iterations * 0.1)
                    //    && inputVectIndx >= (numOfInputVectors * 0.1)) // We let it calculate at least 10% of max iterations
                    //    break;
                }

                if (this.m_TraceTotalError)
                {
                    Debug.WriteLine($"Interation: {i}\tmaxError:{maxError}\tlastErr:{lastError}");
                }

                if (totalError == 0 && i >= (m_Iterations * 0.1)) // We let it calculate at least 10% of max iterations
                    break;

                //if (totalError == 0)
                //    break;
            }

            score.Weights = this.m_Weights;
            //score.Errors = this.m_Errors;//numberofsample
            score.TotolEpochError = totalError;//all 0
            ctx.Score = score;
            return ctx.Score;
        }


        public override IResult Predict(double[][] data, IContext ctx)
        {
            PerceptronResult res = new PerceptronResult()
            {
                PredictedValues = new double[data.Length],
            };

            for (int i = 0; i < data.Length; i++)
            {
                res.PredictedValues[i] = calculateResult(data[i], ctx.DataDescriptor.Features.Length);
            }

            return res;
        }


        private double calculateResult(double[] input, int numOfFeatures)
        {
            double result = 0.0;

            for (int i = 0; i < numOfFeatures; i++)
            {
                result += m_Weights[i] * input[i];
            }

            result += this.m_Threshold;

            return m_ActivationFunction(result);
        }


        private void initializeWeights()
        {
            Random rnd = new Random();

            for (int i = 0; i < m_Dimensions; i++)
            {
                m_Weights[i] = rnd.Next();
            }
        }
    }
}
