
using LearningFoundation;
using LearningFoundation.Binarization.ImageBinarizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.ImageBinarizer
{
    public static class ImageBinarizerModuleExtension
    {
        /// <summary>
        /// Creating Object of Image Binarization in this method and adding it to Api
        /// </summary>
        /// <param name="api">this is a Api used to add module to Learning Api.It is used as a reference of Learning Api</param>
        /// <param name="imageParams"></param>
        /// <returns>It return Api of Learning Api </returns>
        public static LearningApi UseImageBinarizer(this LearningApi api, Dictionary<String, int> imageParams)
        {
            ImageBinarizerModule module = new ImageBinarizerModule(imageParams);
            api.AddModule(module, $"ImageBinarizer-{Guid.NewGuid()}");
            return api;
        }
    }
}
