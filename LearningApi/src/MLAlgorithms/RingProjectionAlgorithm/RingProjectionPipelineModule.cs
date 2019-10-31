using System;
using LearningFoundation;

namespace LearningFoundation.RingProjectionAlgorithm
{
    /// <summary>
    /// public level is used to test RingProjection() method
    /// </summary>
    public class RingProjectionPipelineModule : IPipelineModule<double[][], double[]>
    {
        /// <summary>
        /// What actually happens inside pipeline component
        /// </summary>
        /// <param name="data">output data from the previous compatible pipeline component</param>
        /// <param name="ctx">data description</param>
        /// <returns></returns>
        public double[] Run(double[][] data, IContext ctx)
        {
            return RingProjection(data, out double[][] test);
        }

        /// <summary>
        /// Reduce the dimension of 2D binary Image by using Ring Projection Algorithm
        /// </summary>
        /// <param name="data">2D array binary input data</param>
        /// <param name="loopPath">iteration path of Ring Projection Algorithm as output for testing</param>
        /// <returns></returns>
        public double[] RingProjection(double[][] data, out double[][] loopPath)
        {
            int m_ColLength;
            int m_RowLength;
            int m_MaxRadius;
            int m_xCenter, m_yCenter;
            double[] output;
            loopPath = new double[data.Length][];
            for (int i = 0; i < loopPath.Length; i++)
            {
                loopPath[i] = new double[data[0].Length];
            }

            GetDimension(data, out m_ColLength, out m_RowLength);

            m_xCenter = m_RowLength / 2;
            m_yCenter = m_ColLength / 2;
            m_MaxRadius = (int)Math.Sqrt(m_xCenter * m_xCenter + m_yCenter * m_yCenter);

            output = new double[m_MaxRadius + 1];

            for (int r = 0; r <= m_MaxRadius; r++)
            {
                output[r] = 0;
                for (int y = m_yCenter - r; y <= m_yCenter + r; y++)
                {
                    if (y >= 0 && y < m_ColLength)
                    {
                        for (int x = m_xCenter - r; x <= m_xCenter + r; x++)
                        {
                            if (x >= 0 && x < m_RowLength)
                            {
                                int d = (int)Math.Sqrt((x - m_xCenter) * (x - m_xCenter) + (y - m_yCenter) * (y - m_yCenter));
                                if (d == r)
                                {
                                    loopPath[x][y] = r;
                                    output[r] += data[x][y];
                                }
                            }
                        }
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Provide width and height of a 2D array
        /// </summary>
        /// <param name="array">Input array</param>
        /// <param name="colLength">Output Column Length - height</param>
        /// <param name="rowLength">Output Row Length - width</param>
        private void GetDimension(double[][] array, out int colLength, out int rowLength)
        {
            rowLength = array.Length;
            colLength = array[0].Length;
        }
    }
}
