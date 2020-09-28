using System;
using System.Collections.Generic;
using System.Drawing;
using LearningFoundation;

namespace ImageBinarizerLib
{
    /// <summary>
    /// Main class for the Image Binarizer algorithm using Ipipeline
    /// </summary>

    public class ImageBinarizer: IPipelineModule<double[,,],double[,,]>
    {
        private int m_RedThreshold = -1;

        private int m_GreenThreshold = -1;

        private int m_BlueThreshold = -1;
        
        private Size? m_TargetSize;

        /// <summary>
        /// By default constructor without parameter
        /// </summary>
        public ImageBinarizer()
        {

        }
        /// <summary>
        /// constructor with parameter.
        /// </summary>
        /// <param name="imageParams">Parameters given</param>
        public ImageBinarizer(Dictionary<String, int> imageParams)
        {
            int targetWidth = 0;
            int targetHeight = 0;

            if (imageParams.TryGetValue("redThreshold", out int rt))
                this.m_RedThreshold = rt;

            if (imageParams.TryGetValue("greenThreshold", out int gt))
                this.m_GreenThreshold = gt;

            if (imageParams.TryGetValue("blueThreshold", out int bt))
                this.m_BlueThreshold = bt;

            if (imageParams.TryGetValue("imageWidth", out int iw))
                targetWidth = iw;

            if (imageParams.TryGetValue("imageHeight", out int ih))
                targetHeight = ih;

            if (targetHeight > 0 && targetWidth > 0)
                this.m_TargetSize = new Size(targetWidth, targetHeight);
        }
        /// <summary>
        /// Method of Interface Ipipline
        /// </summary>
        /// <param name="data">this is the double data coming from unitest.</param>
        /// <param name="ctx">this define the Interface IContext for Data descriptor</param>
        /// <returns></returns>
        public double[,,] Run(double[,,] data, IContext ctx)
        {            
            return GetBinary(data);
        }        

        /// <summary>
        /// Gets double array representation of the image.I.E.: 010000111000
        /// </summary>
        /// <params name="img">Image instance. Typically bitmap.</params>
        /// <returns></returns>
        public double[,,] GetBinary(double[,,] data)
        {
            Bitmap img = new Bitmap(data.GetLength(0), data.GetLength(1));

            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    int r = (int)data[i, j, 0];
                    int g = (int)data[i, j, 1];
                    int b = (int)data[i, j, 2];

        //set limits,bytes can hold values from 0 upto 255
                    img.SetPixel(i, j, Color.FromArgb(255, r, g, b));
                }
            }

            if (this.m_TargetSize != null)
                img = new Bitmap(img, this.m_TargetSize.Value);
            
            int hg = img.Height;
            int wg = img.Width;

            double[,,] outArray = new double[hg,wg,3];

            int sumR = 0;
            int sumG = 0;
            int sumB = 0;
            for (int i = 0; i < hg; i++)
            {
                for (int j = 0; j < wg; j++)
                {
                    sumR += img.GetPixel(j, i).R;
                    sumG += img.GetPixel(j, i).G;
                    sumB += img.GetPixel(j, i).B;
                }
            }
        //The average is calculated taking the parameters.When no thresholds are given it automatically calculates the average.
            int avgR = sumR / (hg * wg);
            int avgG = sumG / (hg * wg);
            int avgB = sumB / (hg * wg);

            if (m_RedThreshold < 0 || m_RedThreshold > 255)
            {
                m_RedThreshold = avgR;
            }

            if (m_GreenThreshold < 0 || m_GreenThreshold > 255)
            {
                m_GreenThreshold = avgG;
            }

            if (m_BlueThreshold < 0 || m_BlueThreshold > 255)
            {
                m_BlueThreshold = avgB;
            }

            for (int i = 0; i < hg; i++)
            {
                for (int j = 0; j < wg; j++)
                {
                    outArray[i,j,0] = (img.GetPixel(j, i).R > this.m_RedThreshold && img.GetPixel(j, i).G > this.m_GreenThreshold &&
                       img.GetPixel(j, i).B > this.m_BlueThreshold) ? 1 : 0;
                }
            }
            return outArray;
        }
    }
}
