using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;

namespace DeltaLearning
{
    public static class DeltaLearningExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="api">Context <seealso cref"LearningFoundation.LearningApi"></param></param>
        /// <param name="learningRate"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static LearningApi UseDeltaLearning(this LearningApi api, double learningRate, int iteration, IActivationFunction activationFnc = null)
        {

            //Invoking the class of the algorithm to use
            var alg = new DeltaLearning(learningRate, iteration);

            //now we add this algorithm(new instance of the class) to the learning api
            api.AddModule(alg, "Delta Learning");

            //after adding the instance to the api, return the api
            return api;
        }

        static void Main(string[] args)
        {

            Console.ReadLine();
        }
    }
}
