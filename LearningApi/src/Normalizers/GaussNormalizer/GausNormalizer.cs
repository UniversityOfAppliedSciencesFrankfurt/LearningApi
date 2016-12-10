using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using LearningFoundation.DataMappers;

namespace LearningFoundation.Normalizers
{
    /// <summary>
    /// Implementation Gauss normalization
    /// x_nor=(val-average)/variance
    /// val= average + m_nor*variance;
    /// Type of normalizations can be found at: https://en.wikipedia.org/wiki/Feature_scaling
    /// </summary>
    public class GaussNormalizer : IDataNormalizer
    {
        private double[] m_Mean;
        private double[] m_Var;

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="mean">mean for each column in the dataset</param>
        /// <param name="var">variance for each column in the dataset</param>
        public GaussNormalizer(double[] mean, double[] var)
        {
            m_Mean = mean;
            m_Var = var;
        }    

        /// <summary>
        /// perform process of normalization where natural data is being transformd in to normalized format
        /// x_nor=(val-average)/variance
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

                if (ctx.DataDescriptor.Features[i].Type == LearningFoundation.DataMappers.ColumnType.STRING)
                    continue;
                //numeric column
                else if (ctx.DataDescriptor.Features[i].Type == LearningFoundation.DataMappers.ColumnType.NUMERIC)
                {
                    var value = (rawData[i] - m_Mean[fi]) / m_Var[fi];
                    normData.Add(value);
                }
                //binary column
                else if (ctx.DataDescriptor.Features[i].Type == LearningFoundation.DataMappers.ColumnType.BINARY)
                {
                    //in case of binary column type real and normalized value are the same
                    normData.Add(rawData[i]);

                }
                //category column
                else if (ctx.DataDescriptor.Features[i].Type == LearningFoundation.DataMappers.ColumnType.CLASS)
                {
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
