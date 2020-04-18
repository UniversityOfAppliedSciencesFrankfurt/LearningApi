using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestRingProjectionAlgorithm
{
    /// <summary>
    /// MNISTImage representation
    /// </summary>
    public class MNISTImage
    {
        public byte[][] Pixels { get; }
        public int Label { get; }
        public int Width { get; set; }
        public int Height { get; set; }

        /// <summary>
        /// Create instance of MNISTimage with parameters
        /// </summary>
        /// <param name="width">Width of MNISTimage</param>
        /// <param name="height">Height of MNISTimage</param>
        /// <param name="pixels">array representation of MNISTimage</param>
        /// <param name="label">Label digits</param>
        public MNISTImage(int width, int height, byte[][] pixels, byte label)
        {
            this.Width = width;
            this.Height = height;
            this.Label = label;
            this.Pixels = new byte[width][];
            for (int x = 0; x < width; x++)
            {
                this.Pixels[x] = new byte[height];
                for (int y = 0; y < height; y++)
                {
                    this.Pixels[x][y] = pixels[x][y];
                }
            }
        }
    }
}
