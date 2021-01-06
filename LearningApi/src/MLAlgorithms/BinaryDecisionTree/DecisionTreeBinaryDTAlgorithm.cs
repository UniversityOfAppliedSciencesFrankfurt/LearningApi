using Deedle;
using LearningFoundation;
using System;
using System.Collections.Generic;

namespace DecisionTreeForLearningAPI.BinaryDecisionTree
{
    /// <summary>
    /// Decision tree for a binary classification problem
    /// (i.e. the target attribute is a category of two classes)
    /// </summary>
    public class DecisionTreeBinaryDTAlgorithm : IAlgorithm
    {
        private BinaryDTProcessor binaryDTProcessor = new BinaryDTProcessor(6, 1);

        /// <summary>
        /// Trains the data to generate left tree and right tree branches.
        /// Rows with values less than the criteria go to the left branch and 
        /// greater than the criteria go to the right branch.
        /// This happens recursively until the max depth of the split is reached.
        /// </summary>
        /// <param name="data">Data to be trained</param>
        /// <param name="ctx">Additional input params from learning API</param>
        /// <returns>BranchedScore</returns>
        public IScore Train(double[][] data, IContext ctx)
        {
            BranchedScore score = new BranchedScore();
            Frame<int, String> frame = Frame.CreateEmpty<int, String>();
            String trainedFeature = ctx.DataDescriptor.Features[ctx.DataDescriptor.Features.Length - 1].Name;
            for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
            {
                List<int> row = new List<int>();
                foreach (var item in data)
                {
                    row.Add((int)item[i]);
                }

                frame.AddColumn<int>(ctx.DataDescriptor.Features[i].Name, row.ToArray());
            }
            this.binaryDTProcessor.fit(frame, trainedFeature);
            score.SetbranchedScore(this.binaryDTProcessor);
            return score;
        }

        /// <summary>
        /// Trains the data to generate left tree and right tree branches.
        /// Rows with values less than the criteria go to the left branch and 
        /// greater than the criteria go to the right branch.
        /// This happens recursively until the max depth of the split is reached.
        /// </summary>
        /// <param name="data">Data to be trained</param>
        /// <param name="ctx">Additional input params from learning API</param>
        /// <returns>BranchedScore</returns>
        public IScore Run(double[][] data, IContext ctx)
        {
            return this.Train(data, ctx);
        }

        /// <summary>
        /// Predict the probabilities of the test data based on data trained.
        /// </summary>
        /// <param name="data">Test data to be classified</param>
        /// <param name="ctx">Additional input params from learning API</param>
        /// <returns>BinaryDTResult</returns>
        public IResult Predict(double[][] data, IContext ctx)
        {
            Frame<int, string> frame = Frame.CreateEmpty<int, string>();
            for (int i = 0; i < ctx.DataDescriptor.Features.Length; i++)
            {
                List<int> row = new List<int>();
                foreach (var item in data)
                {
                    row.Add((int)item[i]);
                }
                frame.AddColumn<int>(ctx.DataDescriptor.Features[i].Name, row.ToArray());
            }
            return this.binaryDTProcessor.predict(frame);
        }

        static void Main(string[] args)
        {

        }
    }

}
