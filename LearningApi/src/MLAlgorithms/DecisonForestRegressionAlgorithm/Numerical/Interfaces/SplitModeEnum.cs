using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest.Lib.Numerical.Interfaces
{
    /// <summary>
    /// SplitMode RSS 0 Gini 1
    /// </summary>
    public enum SplitMode
    {
        /// <summary>
        /// RSS method
        /// </summary>
        RSS = 0,
        /// <summary>
        /// Gini method
        /// </summary>
        GINI = 1
    }
}
