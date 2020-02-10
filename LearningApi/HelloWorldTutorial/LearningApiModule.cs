using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorldTutorial
{
    /// <summary>
    /// This class acts as a concret class for Pipeline module
    /// </summary>
    public class LearningApiModule : IPipelineModule<double[][], double[][]>
    {
        /// <summary>
        /// Assining the data to local Variables finaldata
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns> final data </returns>
        public double[][] Run(double[][] data, IContext ctx)
        {
            double[][] finalData = data;

            return finalData;
        }
    }
}
