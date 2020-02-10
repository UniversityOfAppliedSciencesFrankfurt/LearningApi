using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelloWorldTutorial
{
    public static class LearningApiModuleExtension
    {
        /// <summary>
        /// Adding the modules to the Learning API
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public static LearningApi UseLearningApiModule(this LearningApi api)
        {
            LearningApiModule module = new LearningApiModule();
            api.AddModule(module, $"LearningApiModule-{Guid.NewGuid()}");
            return api;
        }
    }
}
