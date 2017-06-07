using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Diagnostics.CodeAnalysis;

namespace LearningFoundation.Fitting
{

    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
    public interface IFittingOptions
    {
        object Clone();
        

    }
}
