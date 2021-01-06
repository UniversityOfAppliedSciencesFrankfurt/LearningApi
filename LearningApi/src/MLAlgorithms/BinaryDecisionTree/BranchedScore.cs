using LearningFoundation;

namespace DecisionTreeForLearningAPI.BinaryDecisionTree
{
    /// <summary>
    /// Output of BinaryDTImpl.train(..)
    /// </summary>
    public class BranchedScore : IScore
    {
        private BinaryDTProcessor branchedScore;

        /// <summary>
        /// Gets the BinaryDTProcessor holding branches of trained data.
        /// </summary>
        /// <returns>BinaryDTProcessor holding branches of trained data.</returns>
        public BinaryDTProcessor GetbranchedScore()
        {
            return branchedScore;
        }

        /// <summary>
        /// Sets the BinaryDTProcessor holding branches of trained data.
        /// </summary>
        public void SetbranchedScore(BinaryDTProcessor value)
        {
            branchedScore = value;
        }
    }
}
