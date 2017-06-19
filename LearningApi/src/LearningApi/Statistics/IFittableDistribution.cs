namespace LearningFoundation.Statistics
{

    /// <summary>
    ///   Common interface for distributions which can be estimated from data.
    /// </summary>
    /// 
    /// <typeparam name="TObservations">The type of the observations, such as System.Double
    /// <typeparam name="TOptions">The type of the options specifying object.</typeparam>
    /// 
    public interface IFittableDistribution<in TObservations> :
        IFittable<TObservations>//, IDistribution<TObservations>
    {
    }

    public interface IFittable<in TObservations>
    {
        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        ///   
        void Fit( TObservations[] observations );

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        ///   
        void Fit( TObservations[] observations, double[] weights );

    }
}
