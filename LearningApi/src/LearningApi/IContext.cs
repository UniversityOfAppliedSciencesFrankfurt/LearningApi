using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Holds context related information, like description of columns, 
    /// currently calculated score, etc.
    /// </summary>
    public interface IContext 
    {
        /// <summary>
        /// 
        /// </summary>
        IDataDescriptor DataDescriptor { get; set; }


        /// <summary>
        /// Current score of algorithm.
        /// </summary>
        IScore Score { get; set; }

        bool IsMoreDataAvailable { get; set; }

        /// <summary>
        /// Mini-batch size.
        /// It must be define at the beginning of the iteration process
        /// In case it is not defined (LTE 0) mini-batching will not be happen.In fact will be performed full batching
        /// </summary>
        int MiniBatchSize { get; set; }
        /// <summary>
        /// Indication of the current mini-batch iteration. It is incremented over each batch iteration,
        /// since we need how much data should be skipped in the current batch iteration. 
        /// </summary> 
        int MiniBatchIteration { get; set; }
    }
}
