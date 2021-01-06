using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest.Lib.Numerical.Interfaces
{
    /// <summary>
    /// ForestFactory
    /// </summary>
    public static class ForestFactory
    {
        /// <summary>
        /// create forest
        /// </summary>
        /// <returns></returns>
        public static IForest Create()
        {
            return new Forest();
        }
    }
}
