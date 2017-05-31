using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.Statistics
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///   Common interface for distribution fitting option objects.
    /// </summary>
    /// 
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
    public interface IFittingOptions // : ICloneable
    {
        // This is an empty interface used to identify a set of types at compile
        // time. The CA rule "CA1040: Avoid empty interfaces" has been suppressed.
    }
}

