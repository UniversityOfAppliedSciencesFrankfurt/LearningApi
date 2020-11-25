using System;
using LearningFoundation;
using System.Collections.Generic;
using System.Text;

namespace HelloLearningApiAlgorithm
{
    public static class LearningApiAlgorithmExtension
    {
            public static LearningApi UseLearningApiAlgorithm(this LearningApi api)
            {
                var alg = new LearningApiAlgorithm();
                api.AddModule(alg, "LearningApiAlgorithm");
                return api;
            }
        }

    }
}
