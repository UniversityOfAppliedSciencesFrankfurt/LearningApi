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
        //private double[] m_Mean;
        //private double[] m_Var;

        /// <summary>
        /// Main Constructor
        /// </summary>
        public GaussNormalizer()
        {
           
        }

        /// <summary>
        /// perform process of normalization where natural data is being transformd in to normalized format
        /// x_nor=(val-average)/variance
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public double[][] Run(double[][] data, IContext ctx)
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

                    if (ctx.DataDescriptor.Features[i].Type == LearningFoundation.DataMappers.ColumnType.STRING)
                        continue;
                    //numeric column
                    else if (ctx.DataDescriptor.Features[i].Type == LearningFoundation.DataMappers.ColumnType.NUMERIC)
                    {
                        var value = 12;// (rawData[i] - m_Mean[fi]) / m_Var[fi];
                        normalizedRow.Add(value);
                    }
                    //binary column
                    else if (ctx.DataDescriptor.Features[i].Type == LearningFoundation.DataMappers.ColumnType.BINARY)
                    {
                        //in case of binary column type real and normalized value are the same
                        normalizedRow.Add(rawData[i]);

                    }
                    //category column
                    else if (ctx.DataDescriptor.Features[i].Type == LearningFoundation.DataMappers.ColumnType.CLASS)
                    {
                        //in case of class column type real and normalized value are the same
                        normalizedRow.Add(rawData[i]);
                    }
                }
            }

            return normData.Select(r => r.ToArray()).ToArray();
        }
    }
}
