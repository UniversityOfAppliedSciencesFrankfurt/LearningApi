using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.DataNormalizers
{
    /// <summary>
    /// Implementation main type of the column normalization
    /// Type of normalizations can be found at: https://en.wikipedia.org/wiki/Feature_scaling
    /// 0 - none
    /// 1 - minmax
    /// 2 - gaus
    /// 3 - custom
    /// </summary>
    public class DataNormalizer : IDataNormalizer
    {
        IDataMapper m_dataMapper;
        public DataNormalizer(IDataMapper mapper)
        {
            m_dataMapper = mapper;
        }

        /// <summary>
        /// perform process of denormalization where normalized data  is being transformed in to natural format
        /// </summary>
        /// <param name="normilizedData"></param>
        /// <returns></returns>
        public double[] DeNormalize(double[] normilizedData)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// perform process of normalization where natural data is being transformd in to normalized format
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public double[] Normalize(double[] rawData)
        {
            throw new NotImplementedException();
        }
    }
}
