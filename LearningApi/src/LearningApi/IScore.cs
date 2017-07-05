using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Defines the status of the trained model.
    /// By implementing of an algorithm, create a class, which derives
    /// from IScore interface, which is basically an empty interface, because
    /// we do not have a generalized descriptiopn of all algorithms.
    /// The created score class shoucl contain any required properties, which
    /// describes the result of the specific alogorithm.
    /// </summary>
    public interface IScore
    {

        //
        double [] Errors { get;set;}

        double [] Weights { get; set;}

    }

    
}
