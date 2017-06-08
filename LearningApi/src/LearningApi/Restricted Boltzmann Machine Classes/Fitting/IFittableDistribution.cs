using System;
using LearningFoundation.Fitting;

namespace LearningFoundation.Statistics
{

    public interface IFittableDistribution<in TObservations, in TOptions> :
        IFittable<TObservations, TOptions>,
        IFittableDistribution<TObservations>
        where TOptions : class, IFittingOptions
    {
    }
    public interface IFittable<in TObservations, in TOptions> :
        IFittable<TObservations>
        where TOptions : class, IFittingOptions
    {
        void Fit(TObservations[] observations, double[] weights = null, TOptions options = default(TOptions));

    }
    public interface IFittableDistribution<in TObservations> :
        IFittable<TObservations>, IDistribution<TObservations>
    {
    }
    public interface IUnivariateFittableDistribution :
        IFittableDistribution<double>,
        IUnivariateDistribution<double>,
        IUnivariateDistribution
    {
    }
    public interface IFittable<in TObservations>
    {
        void Fit(TObservations[] observations);
        void Fit(TObservations[] observations, double[] weights);

    }
}
