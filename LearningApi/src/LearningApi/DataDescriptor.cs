using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    public class DataDescriptor : IDataDescriptor
    {
        public int LabelIndex { get; set; }

        public int NumOfFeatures { get; set; }

        /// <summary>
        ///array of feature which play role in training 
        /// </summary>
        public DataMappers.Column[] Features { get; set; }

        //Used across data transformations
        public double[] m_Min { get; set; }
        public double[] m_Max { get; set; }

    }
}
