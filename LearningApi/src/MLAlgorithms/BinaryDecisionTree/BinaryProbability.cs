
namespace DecisionTreeForLearningAPI.BinaryDecisionTree
{
    /// <summary>
    /// The class representing a row object in the test data,
    /// that gives the probability of occurance of the event
    /// of zero (false) and the probability of occurance of the event
    /// of one (true).
    /// </summary>
    public class BinaryProbability
    {
        private string indexOfRow;

        private double probabilityOfOccuranceOfZero;

        private double probabilityOfOccuranceOfOne;

        /// <summary>
        /// Gets the Index of the row in test data.
        /// </summary>
        /// <returns>Index of the row in test data.</returns>
        public string GetindexOfRow()
        {
            return this.indexOfRow;
        }

        /// <summary>
        /// Sets the Index of the row in test data.
        /// </summary>
        public void SetindexOfRow(string value)
        {
            this.indexOfRow = value;
        }
        /// <summary>
        /// Gets the probability of occurance of the event
        /// of zero (false).
        /// </summary>
        /// <returns>probability of occurance of the event of zero</returns>
        public double GetprobabilityOfOccuranceOfZero()
        {
            return this.probabilityOfOccuranceOfZero;
        }

        /// <summary>
        /// Sets the probability of occurance of the event
        /// of zero (false).
        /// </summary>
        public void SetprobabilityOfOccuranceOfZero(double value)
        {
            this.probabilityOfOccuranceOfZero = value;
        }

        /// <summary>
        /// Gets the probability of occurance of the event
        /// of one (true).
        /// </summary>
        /// <returns>probability of occurance of the event of one</returns>
        public double GetprobabilityOfOccuranceOfOne()
        {
            return this.probabilityOfOccuranceOfOne;
        }

        /// <summary>
        /// Sets the probability of occurance of the event
        /// of one (true).
        /// </summary>
        public void SetprobabilityOfOccuranceOfOne(double value)
        {
            this.probabilityOfOccuranceOfOne = value;
        }
    }
}