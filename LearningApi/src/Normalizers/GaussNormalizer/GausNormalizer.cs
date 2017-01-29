using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using LearningFoundation.DataMappers;
using LearningFoundation.Statistics;

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

            // calculate min and max value for each column vector
            var tuple = data.calculateMeanStDev();
            var desc = ctx.DataDescriptor as DataDescriptor;

            //store values for denormalizer
            desc.Mean = tuple.Item1;
            desc.StDev = tuple.Item2;

            //
            for (int k = 0; k < data.Length; k++)
            {
                var normalizedRow = new List<double>();

                double[] rawData = data[k];

                //get all columns including labelcolumn
                var features = ctx.DataDescriptor.Features;

                //index of numericDataSet
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
                            normalizedRow.Add(rawData[dataIndex]);
                        }
                        else
                        {
                            var value = (rawData[dataIndex] - desc.Mean[dataIndex]) / desc.StDev[dataIndex];
                            normalizedRow.Add(value);
                        }
                        //change the index
                        dataIndex++;
                    }
                    //binary column
                    else if (column.Type == ColumnType.BINARY)
                    {
                        //in case of binary column type real and normalized value are the same
                        normalizedRow.Add(rawData[dataIndex]);

                        //change the index
                        dataIndex++;
                    }
                    //category column
                    else if (column.Type == ColumnType.CLASS)
                    {
                        //in case of class column type real and normalized value are the same
                        for (int i = 0; i < column.Values.Length; i++)
                        {
                            //change the index
                            dataIndex += i;
                            normalizedRow.Add(rawData[dataIndex]);
                        }

                    }
                }

                normData.Add(normalizedRow);
            }

            return normData.Select(r => r.ToArray()).ToArray();
        }
    }
}
