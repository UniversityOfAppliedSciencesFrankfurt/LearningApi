using LearningFoundation;
using System;
using System.Linq;

namespace SumAlgorithm
{
    public class Sum : IAlgorithm
    {
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

            double loss = 0;

            for (int trainDataIndex = 0; trainDataIndex < numTrainData; trainDataIndex++)
            {
                estimatedOutputLabels[trainDataIndex] = ComputeSum(inputFeatures[trainDataIndex]);
                squareErrors[trainDataIndex] = ComputeSquareError(actualOutputLabels[trainDataIndex], estimatedOutputLabels[trainDataIndex]);
            }

            double meanSquareError = squareErrors.Sum() / numTrainData;

            loss = meanSquareError;

            if (ctx.Score as SumScore == null)
                ctx.Score = new SumScore();

            SumScore scr = ctx.Score as SumScore;
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

            for (int testDataIndex = 0; testDataIndex < numTestData; testDataIndex++)
            {
                predictedOutputLabels[testDataIndex] = ComputeSum(inputFeatures[testDataIndex]);
            }

            SumResult res = new SumResult();
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

        private double ComputeSum(double[] input)
        {
            double sum = 0;

            for (int featureIndex = 0; featureIndex < input.Length; featureIndex++)
            {
                sum += input[featureIndex];
            }

            return sum;
        }

        private double ComputeSquareError(double actual, double estimate)
        {
            double squareError = Math.Pow(actual - estimate, 2);

            return squareError;
        }
    }
}
