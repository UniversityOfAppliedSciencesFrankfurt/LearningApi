using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    public static class FuctionModuleExtensions
    {
        public static LearningApi UseActionModule<TIN, TOUT>(this LearningApi api, Func<TIN, IContext, TOUT> moduleFunction)
        {
           var mod = new FunctionModule<TIN, TOUT>(moduleFunction);
           api.AddModule(mod, $"ActionModule-{Guid.NewGuid().ToString()}");
           return api;
        }
    }

    
    public class FunctionModule<TIN, TOUT> : IPipelineModule<TIN, TOUT>
    {
        private Func<TIN, IContext, TOUT> m_ModuleFunction;

        public FunctionModule(Func<TIN, IContext, TOUT> moduleFunction)
        {
            m_ModuleFunction = moduleFunction;
        }

        public TOUT Run(TIN data, IContext ctx)
        {
           return m_ModuleFunction(data, ctx);
        }
    }
}
