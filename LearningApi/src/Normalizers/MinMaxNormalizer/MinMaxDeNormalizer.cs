using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using LearningFoundation.DataMappers;

namespace LearningFoundation.Normalizers
{
    /// <summary>
    /// Implementation of MinMax normalization
    /// nor=(val - min)/(max-min)
    /// Type of normalizations can be found at: https://en.wikipedia.org/wiki/Feature_scaling
    /// </summary>
    public class MinMaxDeNormalizer : IDataDeNormalizer
    {
        private double[] m_Min;
        private double[] m_Max;

        /// <summary>
        /// /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="min">min for each column in the dataset</param>
        /// <param name="max">max for each column in the dataset</param>
        public MinMaxDeNormalizer(double[] min, double[] max)
        {
            m_Min = min;
            m_Max = max;
        }


        /// <summary>
        /// perform process of denormalization where normalized data  is being transformed in to natural format
        /// </summary>
        /// <param name="normalizedData"></param>
        /// <returns></returns>
        public double[][] DeNormalize(double[][] data, IContext ctx)
        {
            var normData = new List<List<double>>();

            for (int k = 0; k < data.Length; k++)
            {
                var normalizedRow = new List<double>();

                double[] rawData = data[k];

                for (int i = 0; i < rawData.Length; i++)
                {
                    //get feature index
                    var fi = ctx.DataDescriptor.Features[i].Index;

                    //numeric column
                    if (ctx.DataDescriptor.Features[i].Type == ColumnType.NUMERIC)
                    {
                        var value = m_Min[fi] + normalizedRow[i] * (m_Max[fi] - m_Min[fi]);
                        normalizedRow.Add(value);
                    }
                    //binary column
                    else if (ctx.DataDescriptor.Features[i].Type == ColumnType.BINARY)
                    {
                        //in case of binary column type real and normalized value are the same
                        normalizedRow.Add(normalizedRow[i]);
                    }
                    //category column
                    else if (ctx.DataDescriptor.Features[i].Type == ColumnType.CLASS)
                    {
                        // COnverts set of binary values in to one category 
                        // Normalized values for Blues category:
                        //          Blue  =  (0,0,1)  - three values which sum is 1,
                        //          Red   =  (1,0,0)
                        //          Green =  (0,1,0)
                        // Example: Red, Gree, Blue - 3 categories  - real values
                        //             0,  1,  2    - 3 numbers     - numeric values
                        //             

                        var count = ctx.DataDescriptor.Features[i].Values.Length;
                        for (int j = 0; j < count; j++)
                        {
                            if (normalizedRow[i + j] == 1)
                                normalizedRow.Add(j);
                        }
                        //
                        i += count;
                    }
                }
            }
            
            return normData.Select(r => r.ToArray()).ToArray();
        }

        public double[][] Run(double[][] rawData, IContext ctx)
        {
            return DeNormalize(rawData, ctx);
        }

    }
}
