using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Basic statistics of column (feature).
    /// </summary>
    public interface IStatistics
    {

        double Max { get; set; }
        double Min { get; set; }
        double Mean { get; set; }
        double Variance { get; set; }

    }

}
