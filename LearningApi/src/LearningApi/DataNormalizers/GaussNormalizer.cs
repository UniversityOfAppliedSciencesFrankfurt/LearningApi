using LearningFoundation.DataMappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.DataNormalizers
{
    /// <summary>
    /// Implementation Gauss normalization
    /// x_nor=(val-average)/variance
    /// val= average + m_nor*variance;
    /// Type of normalizations can be found at: https://en.wikipedia.org/wiki/Feature_scaling
    /// </summary>
    public class GaussNormalizer : IDataNormalizer
    {
        double m_avg;
        double m_variance;
        DataMapper m_dataMapper;
        public GaussNormalizer(DataMapper mapper)
        {
            m_dataMapper = mapper;
        }

        /// <summary>
        /// perform process of denormalization where normalized data  is being transformed in to natural format
        /// val= average + m_nor*variance;
        /// </summary>
        /// <param name="normalizedData"></param>
        /// <returns></returns>
        public double[] DeNormalize(double[] normalizedData)
        {
            
            var rawData = new List<double>();
            for (int i = 0; i < normalizedData.Length; i++)
            {
                //numeric column
                if (m_dataMapper.Features[i].Type == 1)
                {
                    var value = m_avg + normalizedData[i] * m_variance;
                    rawData.Add(value);
                }
                //binary column
                else if (m_dataMapper.Features[i].Type == 2)
                {
                    //in case of binary column type real and normalized value are the same
                    rawData.Add(rawData[i]);

                }
                //category column
                else if (m_dataMapper.Features[i].Type == 3)
                {
                    // COnverts set of binary values in to one category 
                    // Normalized values for Blues category:
                    //          Blue  =  (0,0,1)  - three values which sum is 1,
                    //          Red   =  (1,0,0)
                    //          Green =  (0,1,0)
                    // Example: Red, Gree, Blue - 3 categories  - real values
                    //             0,  1,  2    - 3 numbers     - numeric values
                    //             

                    var count = m_dataMapper.Features[i].Values.Length;
                    for (int j = 0; j < count; j++)
                    {
                        if (rawData[i+j]==1)
                            rawData.Add(j);
                    }
                    //
                    i += count;
                }
            }
            //
            return rawData.ToArray();
        }
        /// <summary>
        /// perform process of normalization where natural data is being transformd in to normalized format
        /// x_nor=(val-average)/variance
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public double[] Normalize(double[] rawData)
        {
            var normData = new List<double>();
            for (int i = 0; i < rawData.Length; i++)
            {
                //numeric column
                if (m_dataMapper.Features[i].Type == 1)
                {
                    var value = (rawData[i] - m_avg) / m_variance;
                    normData.Add(value);
                }
                //binary column
                else if (m_dataMapper.Features[i].Type == 1)
                {
                    //in case of binary column type real and normalized value are the same
                    normData.Add(rawData[i]);

                }
                //category column
                else if (m_dataMapper.Features[i].Type == 1)
                {
                    // COnverts category numeric values in to binary values
                    // it creates array which has length of categories count.
                    // Example: Red, Gree, Blue - 3 categories  - real values
                    //             0,  1,  2    - 3 numbers     - numeric values
                    //             
                    // Normalized values for Blues category:
                    //          Blue  =  (0,0,1)  - three values which sum is 1,
                    //          Red   =  (1,0,0)
                    //          Green =  (0,1,0)
                    var count = m_dataMapper.Features[i].Values.Length;
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
