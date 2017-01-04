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
    public class GaussDeNormalizer : IDataDeNormalizer
    {
        //DataMapper m_DataMapper;
        //private double[] m_Mean;
        //private double[] m_Var;

        /// <summary>
        /// Main Constructor
        /// </summary>
        public GaussDeNormalizer()
        {
           
        }

        /// <summary>
        /// perform process of denormalization where normalized data  is being transformed in to natural format
        /// val= average + m_nor*variance;
        /// </summary>
        /// <param name="normalizedData"></param>
        /// <returns></returns>
        public double[][] DeNormalize(double[][] data, IContext ctx)
        {
            var deNormData = new List<List<double>>();

            //get data desrptior
            var desc = ctx.DataDescriptor as DataDescriptor;

            for (int k = 0; k < data.Length; k++)
            {
                var deNormRow = new List<double>();

                double[] rawData = data[k];

                //get all columns 
                var features = ctx.DataDescriptor.Features;

                //index of numericDataSet.In case of Class column type vector element count is greater than Feature coune
                var dataIndex = 0;

                //enumerate all feature columns
                foreach (var column in features)
                {
                    //numeric column
                    if (column.Type == ColumnType.NUMERIC)
                    {
                        //in case the colum is constant
                        if (desc.StDev[dataIndex] == 0)
                        {
                           deNormRow.Add(rawData[dataIndex]);
                        }
                        else
                        {
                            var value = desc.Mean[dataIndex] + rawData[dataIndex] * desc.StDev[dataIndex];
                            deNormRow.Add(value);
                        }

                        //change the index
                        dataIndex++;

                    }
                    //binary column
                    else if (column.Type == ColumnType.BINARY)
                    {
                        //in case of binary column type real and normalized value are the same
                        deNormRow.Add(rawData[dataIndex]);

                        //change the index
                        dataIndex++;

                    }
                    //category column
                    else if (column.Type == ColumnType.CLASS)
                    {
                        //in case of class column type real and normalized value are the same
                        for (int i = 0; i < column.Values.Length; i++)
                        {
                            dataIndex += i;
                            deNormRow.Add(rawData[dataIndex]);
                        }
                    }
                }

                //
                deNormData.Add(deNormRow);
            }

            return deNormData.Select(r => r.ToArray()).ToArray();
        }


        public double[][] Run(double[][] data, IContext ctx)
        {
            return DeNormalize(data, ctx);
        }
    }
}
