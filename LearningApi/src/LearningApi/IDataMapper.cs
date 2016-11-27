using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Used to map data from input values to features and label.
    /// </summary>
    public interface IDataMapper : IPipelineModule
    {
        /// <summary>
        /// Index of the label.
        /// </summary>
        int LabelIndex { get; set; }

        /// <summary>
        /// Number of features in input vactor (dimension).
        /// </summary>
        int NumOfFeatures { get; set; }

        
        /// <summary>
        /// Gets the index of specified feature.
        /// Features: 
        /// 0: Selery   - 4
        /// 1: Color    - 2
        /// 
        /// Input vector:
        /// 1. 2.3, blue, 4.5, 7.8,..
        /// </summary>
        /// <param name="featureId">Feature ordinal.</param>
        /// <returns>Index of the feature in inoput vector.</returns>
        int GetFeatureIndex(int featureId);

        /// <summary>
        /// Performs any mapping of input vector. I.e: blue->1, yellow->2.
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        object[] RunAsync(object[] rawData);
      
    }
}
