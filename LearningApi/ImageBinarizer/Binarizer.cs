using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace LearningFoundation.ImageBinarizer
{
    /// <summary>
    /// Creates binarized representation form specified image.
    /// </summary>
    /// <remarks>
    /// Supported on Windows only.
    /// </remarks>
    public class Binarizer
    {

        private int m_RedThreshold = 200;

        private int m_GreenThreshold = 200;

        private int m_BlueThreshold = 200;

        private Size? m_TargetSize;

        public Binarizer(int redThreshold = 200, int greenThreshold = 200, int blueThreshold = 200, int targetWidth = 0, int targetHeight = 0)
        {
            this.m_BlueThreshold = blueThreshold;
            this.m_RedThreshold = redThreshold;
            this.m_GreenThreshold = greenThreshold;

            if (targetHeight > 0 || targetHeight > 0)
                this.m_TargetSize = new Size(targetWidth, targetHeight);
        }

        /// <summary>
        /// Gets string binary representation the image.I.E.: 010000111000
        /// </summary>
        /// <param name="image">The filename of the image.</param>
        /// <returns></returns>
        public string GetBinary(string image)
        {
            Bitmap img;
            return GetBinary(image, out img);
        }

        /// <summary>
        /// Gets string binary representation the image.I.E.: 010000111000
        /// </summary>
        /// <param name="image">The filename of the image.</param>
        /// <param name="img">Image instance. Typically bitmap.</param>
        /// <returns></returns>
        public string GetBinary(string image, out Bitmap img )
        {
            img = (Bitmap)Image.FromFile(image);

            if (this.m_TargetSize != null)
                img = new Bitmap(img, this.m_TargetSize.Value);

            StringBuilder t = new StringBuilder();
            int hg = img.Height;
            int wg = img.Width;
            for (int i = 0; i < hg; i++)
            {
                for (int j = 0; j < wg; j++)
                {
                    t.Append((img.GetPixel(j, i).R > this.m_RedThreshold && img.GetPixel(j, i).G > this.m_GreenThreshold &&
                       img.GetPixel(j, i).B > this.m_BlueThreshold) ? 1 : 0);
                }
                t.AppendLine();
            }
            return t.ToString();
        }


        /// <summary>
        /// Creates binarized string from image and saves it to file.
        /// </summary>
        /// <param name="image">The filename of the image.</param>
        /// <param name="binaryFile">The name of the binary output file.</param>
        public void CreateBinary(string image, string binaryFile)
        {
            Bitmap img;

            string binaryData = GetBinary(image, out img);

            using (StreamWriter writer = File.AppendText(binaryFile))
            {
                StringBuilder t = new StringBuilder();
                int hg = img.Height;
                int wg = img.Width;
                for (int i = 0; i < hg; i++)
                {
                    for (int j = 0; j < wg; j++)
                    {
                        // t = 0 .299R + 0 .587G + 0 .144B
                        t.Append((img.GetPixel(j, i).R > 200 && img.GetPixel(j, i).G > 200 &&
                           img.GetPixel(j, i).B > 200) ? 1 : 0);
                    }
                    t.AppendLine();
                }
                string text = t.ToString();
                writer.Write(text);
            }
        }
    }
}

