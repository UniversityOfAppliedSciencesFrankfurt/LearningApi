using LearningFoundation;

namespace DecisionTreeForLearningAPI.BinaryDecisionTree
{
    /// <summary>
    /// Algorithm extension to the learning API.
    /// </summary>
    public static class DecisionTreeAlgorithmExtensions
    {
        /// <summary>
        /// Adds BinaryDTImpl as a module of the learning API.
        /// </summary>
        /// <param name="api"></param>
        /// <returns>BinaryDTImpl module as part of learning API.</returns>
        public static LearningApi UseBinaryDTAlgorithm(this LearningApi api)
        {
            DecisionTreeBinaryDTAlgorithm alg = new DecisionTreeBinaryDTAlgorithm();
            api.AddModule(alg, "BinaryDTImpl"); 
            return api;
        }
    }
}
