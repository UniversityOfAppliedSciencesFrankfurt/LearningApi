using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.RingProjectionAlgorithm
{
    /// <summary>
    /// Extensions Methods for LearningAPI
    /// </summary>
    public static class RingProjectionPipelineModuleExtensions
    {
        /// <summary>
        /// Ring Projection Pipeline Component
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public static LearningApi UseRingProjectionPipelineComponent(this LearningApi api)
        {
            RingProjectionPipelineModule alg = new RingProjectionPipelineModule();
            api.AddModule(alg, $"RingProjectionPipelineComponent-{Guid.NewGuid()}");
            return api;
        }
    }
}
