using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest.Lib.Numerical.Interfaces
{
    /// <summary>
    /// IItemNumerical for data
    /// </summary>
    public interface IItemNumerical
    {
        /// <summary>
        /// SetValue take name and value
        /// </summary>
        /// <param name="featureName"></param>
        /// <param name="featureValue"></param>
        void SetValue(string featureName, double featureValue);

        /// <summary>
        /// featureName
        /// </summary>
        /// <param name="featureName"></param>
        /// <returns></returns>
        double GetValue(string featureName);
        /// <summary>
        /// HasValue
        /// </summary>
        /// <param name="featureName"></param>
        /// <returns></returns>
        bool HasValue(string featureName);
    }
}
