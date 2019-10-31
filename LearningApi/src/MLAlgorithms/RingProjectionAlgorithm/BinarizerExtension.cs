using LearningFoundation.ImageBinarizer;
using System.Drawing;

namespace LearningFoundation.MlAlgorithms.RingProjectionAlgorithm
{
    public static class BinarizerExtension
    {
        /// <summary>
        /// Binarizing image and create 2D array output
        /// </summary>
        /// <param name="bi"></param>
        /// <param name="image">Path of input image</param>
        /// <returns></returns>
        public static double[][] GetBinaryArray(this Binarizer bi, string image, int threshold)
        {
            Bitmap m_img;
            return GetBinaryArray(bi, image, out m_img, threshold);
        }

        /// <summary>
        /// Binarizing image and create 2D array output
        /// </summary>
        /// <param name="bi"></param>
        /// <param name="image">Path of input image</param>
        /// <param name="img"></param>
        /// <returns></returns>
        public static double[][] GetBinaryArray(
            this Binarizer bi, string image, 
            out Bitmap img, int threshold)
        {
            img = (Bitmap)Image.FromFile(image);

            int m_height = img.Height;
            int m_width = img.Width;
            double[][] m_data = new double[m_width][];
            for (int i = 0; i < m_width; i++)
            {
                m_data[i] = new double[m_height];
            }

            for (int i = 0; i < m_height; i++)
            {
                for (int j = 0; j < m_width; j++)
                {
                    if ((img.GetPixel(j, i).R < threshold && img.GetPixel(j, i).G < threshold && img.GetPixel(j, i).B < threshold))
                    {
                        m_data[j][i] = 1;
                    }
                    else
                    {
                        m_data[j][i] = 0;
                    }
                }
            }
            return m_data;
        }

        /// <summary>
        /// Binarizing MNIST database of handwritten image
        /// </summary>
        /// <param name="bi"></param>
        /// <param name="mnistImage">MNIST image array representation</param>
        /// <param name="threshold">Threshold of binarizing (0..255)</param>
        /// <returns></returns>
        public static double[][] ConvertToBinary(
            this Binarizer bi, double[][] mnistImage, int threshold)
        {
            int m_width = mnistImage.Length;
            int m_height = mnistImage[0].Length;

            double[][] m_data = new double[m_width][];
            for (int i = 0; i < m_width; i++)
            {
                m_data[i] = new double[m_height];
            }

            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    if(mnistImage[x][y] > threshold)
                    {
                        m_data[x][y] = 1;
                    }
                    else
                    {
                        m_data[x][y] = 0;
                    }
                }
            }
            return m_data;
        }
    }
}
