using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Defines a Learning Algorithm.
    /// </summary>
    public interface IAlgorithm : IPipelineModule<double[][], IScore>
    {
        /// <summary>
        /// Used for model training.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        IScore Train(double[][] data, IContext ctx);

        /// <summary>
        /// Used for prediction on trained model.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        double[] Predict(double[][] data, IContext ctx);


        // TODO: Validate();
    }
}
