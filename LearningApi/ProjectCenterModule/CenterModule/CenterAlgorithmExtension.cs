using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;


namespace CenterModule
{
    public static class CenterAlgorithmExtension
    {
        public static LearningApi UseMyPipelineModule(this LearningApi api)
        {

            CenterAlgorithm center = new CenterAlgorithm();
            api.AddModule(center, $"CenterModule-{Guid.NewGuid()}");

            return api;

        }
    }
}
