using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageBinarizerLib
{
    /// <summary>
    /// Extention Method class as per Learning Api Architecture
    /// </summary>
    public static class ImageBinarizerExtension
    {
        /// <summary>
        /// Creating Object of Image Binarization in this method and adding it to Api
        /// </summary>
        /// <param name="api">this is a Api used to add module to Learning Api.It is used as a reference of Learning Api</param>
        /// <param name="imageParams"></param>
        /// <returns>It return Api of Learning Api </returns>
        public static LearningApi UseImageBinarizer(this LearningApi api, Dictionary<String, int> imageParams)
        {
            ImageBinarizer module = new ImageBinarizer(imageParams);
            api.AddModule(module, $"ImageBinarizer-{Guid.NewGuid()}");
            return api;
        }
    }
}
