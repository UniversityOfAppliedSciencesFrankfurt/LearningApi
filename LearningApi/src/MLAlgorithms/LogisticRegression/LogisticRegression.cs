using LearningFoundation;
using LogisticRegression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LogisticRegression
{
    public class LogisticRegression: IAlgorithm
    {
        private bool m_UseSGD= false;

        private double m_Alpha;//learning rates
      
        private double m_L1, m_L2; //regularization

        private Random m_Rnd;

        internal int Iterations { get; set; }

        public LogisticRegression(double learningrate)
        {
            m_Alpha = learningrate;
            m_Rnd = new Random((int)DateTime.Now.Ticks);          
        }


        /// <summary>
        /// Training logistic regression algortim for specified dataset
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns>Errors during each iteration</returns>
        public IScore Run(double[][] data, IContext ctx)
        {
            if(ctx.Score as LogisticRegressionScore == null)
                ctx.Score = new LogisticRegressionScore();

            var trainData = data;

            //construct weights acording to features count
            var weights = new double[trainData[0].Length];

            // alpha is the learning rate
            int epoch = 0;
            int[] sequence = new int[trainData.Length]; // random order
            for (int i = 0; i < sequence.Length; ++i)
                sequence[i] = i;
            double[] errors = new double[Iterations];
            while (epoch < Iterations)//Todo: termination criteria is not implemented yet.
            {
                Shuffle(sequence); // process data in random order

                // batch/offline standard approach
                weights = GDLearner(trainData, sequence, weights);

                errors[epoch] = Error(trainData, weights);

                ++epoch;

            } // while

            ctx.Score.Weights = weights;
            ctx.Score.Errors = errors;
            
            return ctx.Score;
        }


        /// <summary>
        /// TODO: We should think about separate all Learner from the ML Algortim
        /// because same learner can be use more than one MLAlgoritm
        /// ..
        /// Gradient Descadent learner.
        /// </summary>
        /// <param name="trainData"></param>
        /// <param name="weights"></param>
        private double[] GDLearner(double[][] trainData, int[] sequence, double[] weights)
        {
            double[] accumulatedGradients = new double[weights.Length]; // one acc for each weight

            for (int ti = 0; ti < trainData.Length; ++ti)  // accumulate
            {
                int i = sequence[ti];

                double computed = ComputeOutput(trainData[i], weights); // no need to shuffle order

                int targetIndex = trainData[i].Length - 1;
                double target = trainData[i][targetIndex];

                accumulatedGradients[0] += (target - computed) * 1; // for b0

                for (int j = 1; j < weights.Length; ++j)
                    accumulatedGradients[j] += (target - computed) * trainData[i][j - 1];
            }

            for (int j = 0; j < weights.Length; ++j) // update
                weights[j] += m_Alpha * accumulatedGradients[j];

            return weights;
        }


        /// <summary>
        /// For currently weights calculate the output values
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        private double ComputeOutput(double[] dataRow, double[] weights)
        {
            double z = 0.0;
            z += weights[0]; // the b0 constant

            for (int i = 0; i < weights.Length - 1; ++i) // data include Y
                z += (weights[i + 1] * dataRow[i]); // skip first weight because we already used it

            return 1.0 / (1.0 + Math.Exp(-z));//sigmoid function

        }

        /// <summary>
        /// Shufles the data
        /// </summary>
        /// <param name="sequence"></param>
        private void Shuffle(int[] sequence)
        {
            for (int i = 0; i < sequence.Length; ++i)
            {
                int r = m_Rnd.Next(i, sequence.Length);
                int tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        }
         
        /// <summary>
        /// Calculate Sum of Square Error from calculate and expected result
        /// </summary>
        /// <param name="trainData"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        private double Error(double[][] trainData, double[] weights)
        {
            // mean squared error using supplied weights

            int yIndex = trainData[0].Length - 1; // y-value (0/1) is last column
            double sumSquaredError = 0.0;
            for (int i = 0; i < trainData.Length; ++i) // each data
            {
                double computed = ComputeOutput(trainData[i], weights);
                double desired = trainData[i][yIndex]; // ex: 0.0 or 1.0
                sumSquaredError += (computed - desired) * (computed - desired);
            }

            return sumSquaredError / trainData.Length;

            // L1 regularization adds the sum of the absolute values of the weights
            // L2 regularization adds the sqrt of sum of squared values

            //*********L1 L2 regularization**************
            //double sumAbsVals = 0.0; // L1 penalty
            //for (int i = 0; i < weights.Length; ++i)
            //    sumAbsVals += Math.Abs(weights[i]);

            //double sumSquaredVals = 0.0; // L2 penalty
            //for (int i = 0; i < weights.Length; ++i)
            //    sumSquaredVals += (weights[i] * weights[i]);
            ////double rootSum = Math.Sqrt(sumSquaredVals);

            //return (sumSquaredError / trainData.Length) +
            //  (l1 * sumAbsVals) +
            //  (l2 * sumSquaredVals);

        }

        public IScore Train(double[][] featureValues, IContext ctx)
        {
            return Run(featureValues, ctx);
        }

        public double[] Predict(double[][] data, IContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
