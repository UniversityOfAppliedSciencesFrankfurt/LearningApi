using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using DeltaLearning;
using System.IO;
using DeltaRuleLearning;
using NeuralNetworks.Core;

namespace DeltaLearning
{
    /// <summary>
    /// Defining Input and Output values
    /// 
    /// </summary>
    public class DeltaLearning :IAlgorithm
    {
        
        private int m_Dimensions;
        private double m_LearningRate = 0.5;

        private int m_Iterations;


        private double[] m_Weights;

        private double[] m_Errors;

        double Aggregate_Weight = 0;

        private Func<double, double> m_ActivationFunction = ActivationFunctions.Sigmoid;


        public IScore Train(double[][] featureValues, IContext ctx)
        {
            return Run(featureValues, ctx);
        }
        public DeltaLearning(double learningRate, int iterations, Func<double, double> activationFunction = null)
        {
            this.m_LearningRate = learningRate;
            this.m_Iterations = iterations;

            if (activationFunction != null)
                this.m_ActivationFunction = activationFunction;
        }

        /// <summary>
        /// Prediction for the model
        /// </summary>
        /// <param name="data"> Input values from the file to predict </param>
        /// <param name="ctx">Context <seealso cref"LearningFoundation.IContext"></param>
        /// <returns></returns>



        /// <summary>
        /// Runinng to update the weights to model the filter
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx">Context <seealso cref"LearningFoundation.IContext"></param>
        /// <returns></returns>
        public IScore Run(double[][] featureValues, IContext ctx)
        {
            m_Dimensions = ctx.DataDescriptor.Features.Count();

            int numOfInputVectors = featureValues.Length;

            m_Weights = new double[m_Dimensions];
            
            m_Errors = new double[numOfInputVectors];

            double error=0;


            double delta = 0;

            initializeWeights();

            var score = new DeltaLearningScore();

            for (int i = 0; i < m_Iterations; i++)
            {
                double totalError = 0;
                int inputVectIndx = 0;
                for (inputVectIndx = 0; inputVectIndx < numOfInputVectors; inputVectIndx++)
                {
                    // Calculate the output value with current weights.
                    double calculatedOutput = calculateResult(featureValues[inputVectIndx], m_Dimensions);

                    
                    // Get expected output.


                    double expectedOutput = featureValues[inputVectIndx][ctx.DataDescriptor.LabelIndex];

                    // Error is difference between calculated output and expected output.
                    error = expectedOutput - calculatedOutput;


                    this.m_Errors[inputVectIndx] = error;

                   

                    for (int dimensionIndx = 0; dimensionIndx < m_Dimensions; dimensionIndx++)
                    {

                          
                            delta = m_LearningRate * featureValues[inputVectIndx][dimensionIndx]*calculatedOutput*(1-calculatedOutput) *error;
                        m_Weights[dimensionIndx] += delta;
                    }
                     
                    totalError += 0.5 * (error * error);
                }

            if (totalError <= 0.1)
                {
                    score.Iterations = i;
                    break;
                }
            }

            score.Weights = this.m_Weights;

            score.Errors = this.m_Errors;

            ctx.Score = score;

            return ctx.Score;
        }


        /// <summary>
        /// Calculation of Result is done here.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="numOfFeatures"></param>
        /// <returns></returns>

        private double calculateResult(double[] input, int numOfFeatures)
        {
            double result = 0.0;


            for (int j = 0; j < numOfFeatures; j++)
            {
                result += m_Weights[j] * input[j];
            }
           
            return m_ActivationFunction(result);
        }

        /// <summary>
        /// The Weights will be Initialised to zeros.
        /// </summary>
        private void initializeWeights()
        {

            for (int i = 0; i < m_Dimensions; i++)
            {
                m_Weights[i] = 0;
            }
        }


        /// <summary>
        /// Prediction from the data given.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>

        public IResult Predict(double[][] data, IContext ctx)
        {
            DeltaLearningResult res = new DeltaLearningResult()
            {
                 PredictedResults = new double[data.Length],
            };
        
            for (int i = 0; i < data.Length; i++)
            {                
                double result = 0.0;
                
                for (int j = 0; j < ctx.DataDescriptor.Features.Length; j++)
                {
                    result += m_Weights[j] * data[i][j];
                    if(result > 1)
                    {
                        result = 1;
                    }else if(result <= 1)
                    {
                        result = 0;
                    }
                    res.PredictedResults[i] = result ;
                }
            }
            
            return res;
        }
    }
}
