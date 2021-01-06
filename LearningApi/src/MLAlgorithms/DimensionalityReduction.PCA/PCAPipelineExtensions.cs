using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.DimensionalityReduction.PCA
{
    /// <summary>
    /// This serve as the extension part of the LearningApi
    /// Call UsePCAPipelineModule() will append a new instance of PCAPipelineModule into the pipeline list of LearningApi
    /// </summary>
    public static class PCAPipelineExtensions
    {
        /// <summary>
        /// Extension method to inject an PCAPipelineModule instance to LearningApi.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="newDimensionSize">choose the new dimension size (optional)</param>
        /// <param name="maximumLoss">define how many of data should be retains (optional, range: 0.0 -> 1.0)</param>
        /// <param name="moduleName">name for the module instance. Using this name the module instance could be queried again to get more information.</param>
        /// <returns>current instance of LearningApi</returns>
        public static LearningApi UsePCAPipelineModule(
            this LearningApi api,  
            int newDimensionSize = 0, 
            double maximumLoss = 0.05,
            string moduleName = "PCAPipelineModule")
        {
            PCAPipelineModule pcaModule = new PCAPipelineModule(newDimensionSize: newDimensionSize, 
                                                                maximumLoss: maximumLoss);
            api.AddModule(pcaModule, $"{moduleName}");
            return api;
        }
    }
}
