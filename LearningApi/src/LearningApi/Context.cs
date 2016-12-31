using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Holds all processing relevant data.
    /// Instance of the context i spassed to every pipeline
    /// durring processing.
    /// </summary>
    public class Context : IContext
    {
        /// <summary>
        /// Descriptor instance, which was passed in the pipeline
        /// before any processing.
        /// </summary>
        public IDataDescriptor DataDescriptor { get; set; }

        public IScore Score { get; set; }
    }
}
