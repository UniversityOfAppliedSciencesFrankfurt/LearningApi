using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation.Statistics;
using LearningFoundation.Fitting;

namespace LearningFoundation.Statistics
{
    


    /// <summary>
    ///   Common interface for distributions which can be estimated from data.
    /// </summary>
    /// 
    /// <typeparam name="TObservations">The type of the observations, such as <see cref="System.Double"/>.</typeparam>
    /// <typeparam name="TOptions">The type of the options specifying object.</typeparam>
    /// 
    public interface IFittableDistribution<in TObservations, in TOptions> :
        IFittable<TObservations, TOptions>,
        IFittableDistribution<TObservations>
        where TOptions : class, IFittingOptions
    {
    }

    /// <summary>
    ///   Common interface for distributions which can be estimated from data.
    /// </summary>
    /// 
    /// <typeparam name="TObservations">The type of the observations, such as <see cref="System.Double"/>.</typeparam>
    /// <typeparam name="TOptions">The type of the options specifying object.</typeparam>
    /// 
    public interface IFittable<in TObservations, in TOptions> :
        IFittable<TObservations>
        where TOptions : class, IFittingOptions
    {

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        void Fit(TObservations[] observations, double[] weights = null, TOptions options = default(TOptions));

    }

    /// <summary>
    ///   Common interface for distributions which can be estimated from data.
    /// </summary>
    /// 
    /// <typeparam name="TObservations">The type of the observations, such as <see cref="System.Double"/>.</typeparam>
    /// 
    public interface IFittableDistribution<in TObservations> :
        IFittable<TObservations>, IDistribution<TObservations>
    {
    }

    /// <summary>
    ///   Common interface for distributions which can be estimated from data.
    /// </summary>
    /// 
    public interface IUnivariateFittableDistribution :
        IFittableDistribution<double>,
        IUnivariateDistribution<double>,
        IUnivariateDistribution
    {
    }

    /// <summary>
    ///   Common interface for distributions which can be estimated from data.
    /// </summary>
    /// 
    /// <typeparam name="TObservations">The type of the observations, such as <see cref="System.Double"/>.</typeparam>
    /// 
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
        void Fit(TObservations[] observations);

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        ///   
        void Fit(TObservations[] observations, double[] weights);

    }
}
