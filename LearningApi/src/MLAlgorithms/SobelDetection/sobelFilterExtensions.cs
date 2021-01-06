using LearningFoundation;
using Sobel_Detection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sobel_Detection
{/// <summary>
/// Extention Method class as per Learning api architecture.
/// </summary>
    public static class SobelFilterExtensions
    {    /// <summary>
         /// Creating Object of SobelConvolutionFilter in this method and adding it to api.
         /// </summary>
         /// <param name="api">this is a api used to add module to learningApi. It is used as reference of LearningApi</param>
         /// <returns>It return api of LearningaApi</returns>
        public static LearningApi UseSobelConvolutionFilter(this LearningApi api)
        {
            
            SobelConvolutionFilter sobelconvo = new SobelConvolutionFilter();
            api.AddModule(sobelconvo, "UseSobelConvolutionFilter");
            return api;
        }

    }
}