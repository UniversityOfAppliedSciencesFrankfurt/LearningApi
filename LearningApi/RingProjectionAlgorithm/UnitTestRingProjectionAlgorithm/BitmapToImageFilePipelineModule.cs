using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using LearningFoundation;

namespace UnitTestRingProjectionAlgorithm
{
    /// <summary>
    /// Save Bitmap to Image file
    /// </summary>
    public class BitmapToImageFilePipelineModule : IPipelineModule<Bitmap, object>
    {
        public string Label { get; set; }
        public string BasePath { get; set; }
        public ImageFormat ImageType { get; set; }

        /// <summary>
        /// Save Bitmap to Image file
        /// </summary>
        /// <param name="label">Image name</param>
        /// <param name="basePath">Save path of Image file</param>
        /// <param name="imageType">Save type of Image as static value</param>
        public BitmapToImageFilePipelineModule(string label, string basePath, ImageFormat imageType)
        {
            this.Label = label;
            this.BasePath = basePath;
            this.ImageType = imageType;
        }

        /// <summary>
        /// What actually happens inside pipeline component
        /// </summary>
        /// <param name="data">output data from the previous compatible pipeline component</param>
        /// <param name="ctx">data description</param>
        /// <returns></returns>
        public object Run(Bitmap data, IContext ctx)
        {
            string savePath = Path.Combine(BasePath, $"{Label}.{ImageType.ToString()}");
            data.Save(savePath, ImageType);

            return null;
        }
    }
}
