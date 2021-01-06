using System;

namespace LearningFoundation.DimensionalityReduction.PCA.Exceptions
{
    /// <summary>
    /// This will be thown when ever user try to input an invalid matrix.
    /// Invalid matrix could be caused the module use jagged array.
    /// </summary>
    [Serializable]
    public class InvalidDimensionException : Exception
    {
        /// <summary>
        /// Constructor to create a new instance of InvalidDimensionException
        /// </summary>
        /// <param name="message">string message for the exception</param>
        public InvalidDimensionException(string message) : base(message)
        {
        }
    }
}
