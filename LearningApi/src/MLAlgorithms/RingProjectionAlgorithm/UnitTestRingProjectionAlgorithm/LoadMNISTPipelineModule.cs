using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LearningFoundation;

namespace UnitTestRingProjectionAlgorithm
{
    /// <summary>
    /// Read Image from MNIST database of handwritten digits
    /// </summary>
    public class LoadMNISTPipelineModule : IPipelineModule<BinaryReader[], MNISTImage[]>
    {
        /// <summary>
        /// What actually happens inside pipeline component
        /// </summary>
        /// <param name="data">output data from the previous compatible pipeline component</param>
        /// <param name="ctx">data description</param>
        /// <returns></returns>
        public MNISTImage[] Run(BinaryReader[] data, IContext ctx)
        {
            MNISTImage[] m_mnistImages;
            BinaryReader m_images = data[0];
            BinaryReader m_labels = data[1];

            m_images.ReadInt32(); // Read Magic number
            var m_imageCount = ReverseBytes(m_images.ReadInt32());
            var m_height = ReverseBytes(m_images.ReadInt32());
            var m_width = ReverseBytes(m_images.ReadInt32());

            m_labels.ReadInt32(); // Read Magic number
            var m_labelCount = ReverseBytes(m_labels.ReadInt32());

            if (m_labelCount == m_imageCount)
            {
                m_mnistImages = new MNISTImage[m_imageCount];
            }
            else
            {
                return null;
            }

            byte[][] m_temp = new byte[m_width][];
            for (int i = 0; i < m_width; i++)
            {
                m_temp[i] = new byte[m_height];
            }

            for (int n = 0; n < m_imageCount; n++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    for (int x = 0; x < m_width; x++)
                    {
                        m_temp[x][y] = m_images.ReadByte();
                    }
                }
                m_mnistImages[n] = new MNISTImage(m_width, m_height, m_temp, m_labels.ReadByte());
            }

            return m_mnistImages;
        }

        /// <summary>
        /// Convert high-endian format to low-endian format
        /// </summary>
        /// <param name="number">Input number</param>
        /// <returns></returns>
        internal int ReverseBytes(int number)
        {
            byte[] m_temp = BitConverter.GetBytes(number);
            Array.Reverse(m_temp);
            return BitConverter.ToInt32(m_temp);
        }
    }
}
