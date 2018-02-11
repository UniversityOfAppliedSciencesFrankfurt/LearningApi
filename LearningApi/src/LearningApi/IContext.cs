using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Holds context related information, like description of columns, 
    /// currentlly calculated score, etc.
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
    }
}
