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
            desc.m_Min = tuple.Item1;
            desc.m_Max = tuple.Item2;

            //
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
                        var value = (rawData[i] - tuple.Item1[fi]) / (tuple.Item2[fi] - tuple.Item1[fi]);
                        normalizedRow.Add(value);
                    }
                    //binary column
                    else if (ctx.DataDescriptor.Features[i].Type == ColumnType.BINARY)
                    {
                        //in case of binary column type real and normalized value are the same
                        normalizedRow.Add(rawData[i]);

                    }
                    //category column
                    else if (ctx.DataDescriptor.Features[i].Type == ColumnType.CLASS)
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
                                normalizedRow.Add(1);
                            else
                                normalizedRow.Add(0);
                        }
                    }
                }

                normData.Add(normalizedRow);
            }
          
            return normData.Select(r => r.ToArray()).ToArray();
        }
    }
}
