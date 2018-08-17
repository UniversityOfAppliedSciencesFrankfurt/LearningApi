using NeuralNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using System.Diagnostics;
using NeuralNetworks.Core.ActivationFunctions;
namespace NeuralNet.Perceptron
{
    public class PerceptronAlgorithm : NeuralNetCore
    {
        private double m_LearningRate = 0.5;
        private int m_Iterations;
        private Func<double, double> m_ActivationFunction = new BooleanActivationFunction().Function;
        private int m_Dimensions;
        private double[] m_Weights;
        private double[] m_Errors;
        private double m_Threshold;
        //  private bool m_PersistConvergenceData = false;

        public PerceptronAlgorithm(double threshold, double learningRate, int iterations, Func<double, double> activationFunction = null)
        {
            this.m_Threshold = threshold;
            this.m_LearningRate = learningRate;
            this.m_Iterations = iterations;
            if (activationFunction != null)
                this.m_ActivationFunction = activationFunction;
        }

        public override IScore Run(double[][] featureValues, IContext ctx)
        {
            m_Dimensions = ctx.DataDescriptor.Features.Count();
            int numOfInputVectors = featureValues.Length;
            m_Weights = new double[m_Dimensions];
            m_Errors = new double[numOfInputVectors];
            initializeWeights();
            double totalError = 0.0;
            var score = new PerceptronAlgorithmScore();
            for (int i = 0; i < m_Iterations; i++)
            {
                // totalError = 0;

                for (int inputVectIndx = 0; inputVectIndx < numOfInputVectors; inputVectIndx++)
                {
                    // Calculate the output value with current weights.
                    double calculatedOutput = calculateResult(featureValues[inputVectIndx], m_Dimensions);

                    // Get expected output.
                    double expectedOutput = featureValues[inputVectIndx][ctx.DataDescriptor.LabelIndex];

                    // Error is difference between calculated output and expected output.
                    double error = expectedOutput - calculatedOutput;
                    this.m_Errors[inputVectIndx] = error;

                    // Total error for all input vectors.
                    totalError += Math.Abs(error);

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

                }

                if (totalError == 0) break;
                
            }

            score.Weights = this.m_Weights;
            score.Errors = this.m_Errors;//numberofsample
            score.TotolEpochError = totalError;//all 0
            ctx.Score = score;
            return ctx.Score;
        }
        public override double[] Predict(double[][] data, IContext ctx)
        {
            double[] results = new double[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                results[i] = calculateResult(data[i], ctx.DataDescriptor.Features.Length);
            }

            return results;
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
