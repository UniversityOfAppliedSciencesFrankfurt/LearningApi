using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    public interface IPipelineModule
    {

    }

    public interface IPipelineModule<TIN, TOUT> : IPipelineModule
    {
        TOUT Run(TIN data, IContext ctx);
    }
}
