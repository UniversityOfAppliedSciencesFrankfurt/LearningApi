using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DecisonForestRegressionAlgorithm
{
    /// <summary>
    /// MyPipelineModuleExtensions
    /// </summary>
    public static class MyPipelineModuleExtensions
    {
        /// <summary>
        /// UseMyPipelineModule take learning api as input
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public static LearningApi UseDecisionForestRegressionModule(this LearningApi api)
        {

            MyPipelineComponent alg = new MyPipelineComponent();
            api.AddModule(alg, $"MyPipelineComponent-{Guid.NewGuid()}");
            return api;
        }

    }
}
