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
    /// Implementation of MinMax normalization
    /// nor=(val - min)/(max-min)
    /// Type of normalizations can be found at: https://en.wikipedia.org/wiki/Feature_scaling
    /// </summary>
    public class MinMaxNormalizer : IDataNormalizer
    {
        /// <summary>
        /// /// <summary>
        /// Main constructor
        /// </summary>
        public MinMaxNormalizer()
        {
        }


        

        /// <summary>
        /// perform process of normalization where natural data is being transformd 
        /// in to normalized format
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[][] Run(double[][] data, IContext ctx)
        {
            var normData = new List<List<double>>();

            // calculate min and max value for each column vector
            var tuple = data.calculateMinMax();
            var desc = ctx.DataDescriptor as DataDescriptor;

            //store values for denormalizer
            desc.Min = tuple.Item1;
            desc.Max = tuple.Item2;

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
                        if (desc.Max[dataIndex] == desc.Min[dataIndex])
                        {
                            if (desc.Max[dataIndex] == 0) //zero column 
                                normalizedRow.Add(rawData[dataIndex]);
                            else if (desc.Max[dataIndex] > 1 || desc.Max[dataIndex] < -1)//nonzero column
                                normalizedRow.Add(rawData[dataIndex] * desc.Max[dataIndex]);
                            else
                                normalizedRow.Add(rawData[dataIndex]);
                        }
                        else
                        {
                            var value = (rawData[dataIndex] - desc.Min[dataIndex]) / (desc.Max[dataIndex] - desc.Min[dataIndex]);
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
                        for(int i=0; i < column.Values.Length; i++)
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
