using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.MlAlgorithms.SupportVectorMachineAlgorithm
{
    /// <summary>
    /// SVMExtensions is an extension that call SVM through Learning API
    /// </summary>
    public static class SVMExtensions
    {
        /// <summary>
        /// UseSVMAlgorithm is used to call SVMAlgorithm from Learning Api NuGet
        /// </summary>
        /// <param name="api"></param>
        /// <param name="c"></param>
        /// <param name="tol"></param>
        /// <returns></returns>
        public static LearningApi UseSVMAlgorithm(this LearningApi api, double c, double tol = 0.01) {
            
            SVMAlgorithm alg = new SVMAlgorithm(c,tol);
            api.AddModule(alg, "SVMAlgorithm");
            return api;
        }
    }
}
