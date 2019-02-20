using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using LearningFoundation;

namespace UnitTestRingProjectionAlgorithm
{
    /// <summary>
    /// Convert double[][] to Bitmap
    /// </summary>
    public class DoubleArrrayToBitmapPipelineModule : IPipelineModule<double[][], Bitmap>
    {
        public int Width { get; set; }
        public int Height { get; set; }

        /// <summary>
        /// Transform double[][] representation of Image into Bitmap object
        /// </summary>
        /// <param name="width">Width of Image</param>
        /// <param name="height">Height of Image</param>
        public DoubleArrrayToBitmapPipelineModule(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// What actually happens inside pipeline component
        /// </summary>
        /// <param name="data">output data from the previous compatible pipeline component</param>
        /// <param name="ctx">data description</param>
        /// <returns></returns>
        public Bitmap Run(double[][] data, IContext ctx)
        {
            Bitmap m_image = new Bitmap(Width, Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int plainColor = 255 - (int)data[x][y];
                    m_image.SetPixel(x, y, Color.FromArgb(plainColor, plainColor, plainColor));
                }
            }
            return m_image;
        }
    }
}
