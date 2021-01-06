using LearningFoundation;
using System.Collections.Generic;

namespace DecisionTreeForLearningAPI.BinaryDecisionTree
{
    /// <summary>
    /// Output of BinaryDTImpl.predict(...) holding prediction data.
    /// </summary>
    public class BinaryDTResult : IResult
    {
        private List<BinaryProbability> binaryDTResult = new List<BinaryProbability>();

        /// <summary>
        /// Sets BinaryDTResult.
        /// </summary>
        /// <param name="binaryDTResult"></param>
        public void setBinaryDTResult(List<BinaryProbability> binaryDTResult)
        {
            this.binaryDTResult = binaryDTResult;
        }

        /// <summary>
        /// Gets BinaryDTResult.
        /// </summary>
        /// <returns></returns>
        public List<BinaryProbability> getBinaryDTResult()
        {
            return this.binaryDTResult;
        }

        /// <summary>
        /// Adds to BinaryProbability BinaryDTResult.
        /// </summary>
        /// <param name="binaryProbability"></param>
        public void addToBinaryDTResult(BinaryProbability binaryProbability)
        {
            this.binaryDTResult.Add(binaryProbability);
        }
    }
}
