using LearningFoundation;
using System;
using System.Linq;

namespace ExampleLearningApiAlgorithm
{
    public class LinearRegression : IAlgorithm
    {
        private double m_LearningRate;
        private int m_Epochs;

        public LinearRegression(double learningRate, int epochs)
        {
            m_LearningRate = learningRate;
            m_Epochs = epochs;
        }

        public IScore Run(double[][] data, IContext ctx)
        {
            return Train(data, ctx);
        }

        public IScore Train(double[][] data, IContext ctx)
        {
            var trainData = data;

            int numTrainData = trainData.Length;

            int numFeatures = ctx.DataDescriptor.Features.Length - 1;

            int labelIndex = ctx.DataDescriptor.LabelIndex;

            double[][] inputFeatures = GetInputFeaturesFromData(trainData, numFeatures);

            double[] actualOutputLabels = GetActualOutputLabelsFromData(trainData, labelIndex);

            double[] estimatedOutputLabels = new double[numTrainData];

            double[] squareErrors = new double[numTrainData];

            double[] loss = new double[m_Epochs];

            double[] weights = new double[numFeatures];

            double bias = 0;

            for (int epoch = 0; epoch < m_Epochs; epoch++)
            {
                for (int trainDataIndex = 0; trainDataIndex < numTrainData; trainDataIndex++)
                {
                    estimatedOutputLabels[trainDataIndex] = ComputeOutput(inputFeatures[trainDataIndex], weights, bias);
                    squareErrors[trainDataIndex] = ComputeSquareError(actualOutputLabels[trainDataIndex], estimatedOutputLabels[trainDataIndex]);
                }

                double meanSquareError = squareErrors.Sum() / numTrainData;

                loss[epoch] = meanSquareError;

                Tuple<double[], double> hyperParameters = GradientDescent(actualOutputLabels, estimatedOutputLabels, inputFeatures, numTrainData, numFeatures);

                // Partial derivatives of loss with respect to weights
                double[] dWeights = hyperParameters.Item1;

                // Updating weights
                for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                {
                    weights[featureIndex] = weights[featureIndex] - m_LearningRate * dWeights[featureIndex];
                }

                // Partial derivative of loss with respect to bias
                double dbias = hyperParameters.Item2;

                // Updating bias
                bias = bias - m_LearningRate * dbias;
            }

            if (ctx.Score as LinearRegressionScore == null)
                ctx.Score = new LinearRegressionScore();

            LinearRegressionScore scr = ctx.Score as LinearRegressionScore;
            scr.Weights = weights;
            scr.Bias = bias;
            scr.Loss = loss;

            return ctx.Score;
        }

        public IResult Predict(double[][] data, IContext ctx)
        {
            var testData = data;

            int numTestData = testData.Length;

            int numFeatures = ctx.DataDescriptor.Features.Length - 1;

            double[][] inputFeatures = GetInputFeaturesFromData(testData, numFeatures);

            double[] predictedOutputLabels = new double[numTestData];

            double[] weights = ((LinearRegressionScore)ctx.Score).Weights;

            double bias = ((LinearRegressionScore)ctx.Score).Bias;

            for (int testDataIndex = 0; testDataIndex < numTestData; testDataIndex++)
            {
                predictedOutputLabels[testDataIndex] = ComputeOutput(inputFeatures[testDataIndex], weights, bias);
            }

            LinearRegressionResult res = new LinearRegressionResult();
            res.PredictedValues = predictedOutputLabels;

            return res;
        }

        private double[][] GetInputFeaturesFromData(double[][] data, int numFeatures)
        {
            int numData = data.Length;

            double[][] inputFeatures = new double[numData][];

            for (int dataIndex = 0; dataIndex < numData; dataIndex++)
            {
                inputFeatures[dataIndex] = new double[numFeatures];
                for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                {
                    inputFeatures[dataIndex][featureIndex] = data[dataIndex][featureIndex];
                }
            }

            return inputFeatures;
        }

        private double[] GetActualOutputLabelsFromData(double[][] data, int labelIndex)
        {
            int numData = data.Length;

            double[] actualOutputLabels = new double[numData];

            for (int dataIndex = 0; dataIndex < numData; dataIndex++)
            {
                actualOutputLabels[dataIndex] = data[dataIndex][labelIndex];
            }

            return actualOutputLabels;
        }

        private double ComputeOutput(double[] input, double[] weights, double bias)
        {
            double output = 0;

            for (int featureIndex = 0; featureIndex < input.Length; featureIndex++)
            {
                output += weights[featureIndex] * input[featureIndex];
            }

            output += bias;

            return output;
        }

        private double ComputeSquareError(double actual, double estimate)
        {
            double squareError = Math.Pow(actual - estimate, 2);

            return squareError;
        }

        private Tuple<double[], double> GradientDescent(double[] actualOutputLabels, double[] estimatedOutputLabels, double[][] inputFeatures, int numTrainData, int numFeatures)
        {
            double[] dWeights = new double[numFeatures];

            for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
            {
                for (int trainDataIndex = 0; trainDataIndex < numTrainData; trainDataIndex++)
                {
                    dWeights[featureIndex] += inputFeatures[trainDataIndex][featureIndex] * (actualOutputLabels[trainDataIndex] - estimatedOutputLabels[trainDataIndex]);
                }
                dWeights[featureIndex] = -2 * dWeights[featureIndex] / numTrainData;
            }

            double dBias = 0;

            for (int trainDataIndex = 0; trainDataIndex < numTrainData; trainDataIndex++)
            {
                dBias += actualOutputLabels[trainDataIndex] - estimatedOutputLabels[trainDataIndex];
            }
            dBias = -2 * dBias / numTrainData;

            return Tuple.Create(dWeights, dBias);
        }
    }
}
