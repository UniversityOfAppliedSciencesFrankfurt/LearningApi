using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation
{
    public class PipelineModuleModel
    {
        public string ModuleName { get; set; }

        public string AssemblyQualifiedName { get; set; }

        public IModel Model { get; set; }
    }

    public class PipelineModel
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public List<PipelineModuleModel> Modules { get; set; }
    }
}
