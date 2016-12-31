using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Defines the status of the trained model.
    /// </summary>
    public interface IScore
    {

        //
        double [] Errors { get;set;}

        double [] Weights { get; set;}

    }

    
}
