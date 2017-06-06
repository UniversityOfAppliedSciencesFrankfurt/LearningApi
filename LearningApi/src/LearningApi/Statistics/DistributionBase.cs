using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.Statistics
{
    /// 
    [Serializable]
    public abstract class DistributionBase : IFormattable
    {
             
        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }
               
        public string ToString(string format)
        {
            return ToString(format, null);
        }
                
        public abstract string ToString(string format, IFormatProvider formatProvider);

        public abstract object Clone();

    }
}
