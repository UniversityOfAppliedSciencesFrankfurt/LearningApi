using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisonForestRegressionAlgorithm
{
    /// <summary>
    /// MyPipelineComponent implement IpipelineModule
    /// </summary>
    public class MyPipelineComponent : IPipelineModule<double[][], double[][]>
    {
        /// <summary>
        /// Run method input data and context
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public double[][] Run(double[][] data, IContext ctx)
        {
            return data;
        }
    }
}
