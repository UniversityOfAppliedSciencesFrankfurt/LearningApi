using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Provides extension for injectable function module.
    /// By using of this module, you don't have to implement module as a separeted project.
    /// You can rather inject the function, which will be executed in the pipeline.
    /// </summary>
    public static class FuctionModuleExtensions
    {
        /// <summary>
        /// Enables simplified implementation of some module functionality in a form
        /// of injected function.
        /// </summary>
        /// <typeparam name="TIN">Input value.</typeparam>
        /// <typeparam name="TOUT">Function output value.</typeparam>
        /// <param name="api"></param>
        /// <param name="moduleFunction">Function, which implements the module functionality.</param>
        /// <param name="moduleName">The unique module name. If not specified it is automatically created.
        /// If loading and saving of model is used and action modules are used, then action modules cannot be persisted.
        /// In this case after loading of the model, you will need to use ReplaceActionModule by giving the name, which was
        /// used by calling UseActionModule before model was saved.</param>        
        /// <returns>Output value of the module defined by <see cref="TOUT"/></returns>
        public static LearningApi UseActionModule<TIN, TOUT>(this LearningApi api, Func<TIN, IContext, TOUT> moduleFunction, string moduleName = null)
        {
           var mod = new FunctionModule<TIN, TOUT>(moduleFunction);

            if (String.IsNullOrEmpty(moduleName))
                moduleName = $"ActionModule-{Guid.NewGuid().ToString()}";

           api.AddModule(mod, moduleName);

           return api;
        }


        /// <summary>
        /// Replaces existing module
        /// </summary>
        /// <typeparam name="TIN">Input value.</typeparam>
        /// <typeparam name="TOUT">Function output value.</typeparam>
        /// <param name="api"></param>
        /// <param name="moduleFunction">Function, which implements the module functionality.</param>
        /// <param name="moduleName">The unique module name. If not specified it is automatically created.
        /// If loading and saving of model is used and action modules are used, then action modules cannot be persisted.
        /// In this case after loading of the model, you will need to use ReplaceActionModule by giving the name, which was
        /// used by calling UseActionModule before model was saved.</param>        
        /// <returns>Output value of the module defined by <see cref="TOUT"/></returns>
        /// <returns></returns>
        public static LearningApi ReplaceActionModule<TIN, TOUT>(this LearningApi api, string moduleName, Func<TIN, IContext, TOUT> moduleFunction)
        {
            var existingModule = api.Modules.Keys.FirstOrDefault((m)=>m == moduleName);

            if (existingModule != null)
            {
                api.Modules[moduleName] = new FunctionModule<TIN, TOUT>(moduleFunction);
            }
           
            return api;
        }
    }

    /// <summary>n
    /// Used as predefault module, which can be directly injected in a code without of need to 
    /// implement custom module.
    /// It is usefull for testig.
    /// </summary>
    /// <typeparam name="TIN"></typeparam>
    /// <typeparam name="TOUT"></typeparam>
    public class FunctionModule<TIN, TOUT> : IPipelineModule<TIN, TOUT>
    {
        private Func<TIN, IContext, TOUT> m_ModuleFunction;

        public FunctionModule(Func<TIN, IContext, TOUT> moduleFunction)
        {
            m_ModuleFunction = moduleFunction;
        }

        public TOUT Run(TIN data, IContext ctx)
        {
            if (m_ModuleFunction == null)
                throw new ArgumentException("Module incorrectly initialized!");
           return m_ModuleFunction.Invoke(data, ctx);
        }
    }
}
