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
    public class MinMaxNormalizer : IDataNormalizer
    {
        private double[] m_Min;
        private double[] m_Max;

        /// <summary>
        /// /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="mapper">data mapper for the related data</param>
        /// <param name="min">min for each column in the dataset</param>
        /// <param name="max">max for each column in the dataset</param>
        public MinMaxNormalizer(double[] min, double[] max)
        {
            m_Min = min;
            m_Max = max;
        }


        /// <summary>
        /// perform process of denormalization where normalized data  is being transformed in to natural format
        /// </summary>
        /// <param name="normalizedData"></param>
        /// <returns></returns>
        //public double[] DeNormalize(double[] normalizedData)
        //{
        //    //
        //    var rawData = new List<double>();
        //    for (int i = 0; i < normalizedData.Length; i++)
        //    {
        //        //get feature index
        //        var fi = m_DataMapper.GetFeatureIndex(i);

        //        //numeric column
        //        if (m_DataMapper.Features[i].Type == ColumnType.NUMERIC)
        //        {
        //            var value = m_Min[fi] + normalizedData[i] * (m_Max[fi] - m_Min[fi]);
        //            rawData.Add(value);
        //        }
        //        //binary column
        //        else if (m_DataMapper.Features[i].Type == ColumnType.BINARY)
        //        {
        //            //in case of binary column type real and normalized value are the same
        //            rawData.Add(rawData[i]);

        //        }
        //        //category column
        //        else if (m_DataMapper.Features[i].Type == ColumnType.CLASS)
        //        {
        //            // COnverts set of binary values in to one category 
        //            // Normalized values for Blues category:
        //            //          Blue  =  (0,0,1)  - three values which sum is 1,
        //            //          Red   =  (1,0,0)
        //            //          Green =  (0,1,0)
        //            // Example: Red, Gree, Blue - 3 categories  - real values
        //            //             0,  1,  2    - 3 numbers     - numeric values
        //            //             

        //            var count = m_DataMapper.Features[i].Values.Length;
        //            for (int j = 0; j < count; j++)
        //            {
        //                if (rawData[i + j] == 1)
        //                    rawData.Add(j);
        //            }
        //            //
        //            i += count;
        //        }
        //    }
        //    //
        //    return rawData.ToArray();
        //}


        /// <summary>
        /// perform process of normalization where natural data is being transformd in to normalized format
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public double[] Run(double[] rawData, IContext ctx)
        {
            //
            var normData = new List<double>();
            for (int i = 0; i < rawData.Length; i++)
            {
                //get feature index
                var fi = ctx.DataDescriptor.Features[i].Index;

                //numeric column
                if (ctx.DataDescriptor.Features[i].Type ==  ColumnType.NUMERIC)
                {
                    var value = (rawData[i] - m_Min[fi]) / (m_Max[fi] - m_Min[fi]);
                    normData.Add(value);
                }
                //binary column
                else if (ctx.DataDescriptor.Features[i].Type == ColumnType.BINARY)
                {
                    //in case of binary column type real and normalized value are the same
                    normData.Add(rawData[i]);

                }
                //category column
                else if (ctx.DataDescriptor.Features[i].Type ==  ColumnType.CLASS)
                {
                    //TODO:
                    // Converts category numeric values in to binary values
                    // it creates array which has length of categories count.
                    // Example: Red, Gree, Blue - 3 categories  - real values
                    //             0,  1,  2    - 3 numbers     - numeric values
                    //             
                    // Normalized values for Blues category:
                    //          Blue  =  (0,0,1)  - three values which sum is 1,
                    //          Red   =  (1,0,0)
                    //          Green =  (0,1,0)
                    var count = ctx.DataDescriptor.Features[i].Values.Length;
                    for (int j = 0; j < count; j++)
                    {
                        if (j == rawData[i])
                            normData.Add(1);
                        else
                            normData.Add(0);
                    }
                }
            }
            //
            return normData.ToArray();
        }

    }
}
