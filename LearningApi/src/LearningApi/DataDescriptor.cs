using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Implements Meta Data information about Training or Testing data. 
    /// The class also defined which problem type LearningAPI can solve.
    /// </summary>
    public class DataDescriptor : IDataDescriptor
    {
        public DataDescriptor()
        {
            //initial value of LabelIndex should be -1.
            //bay default DataDescriptor doesnt contain proper LabelIndex
            //in case LabelIndex is -1, this means no Label data contains which point to unsupervized learning.
            LabelIndex = -1;

        }

        /// <summary>
        /// Index of the Column in RealData pointing to the Labeled column.
        /// </summary>
        public int LabelIndex { get; set; }

       // public int NumOfFeatures { get; set; }

        /// <summary>
        ///array of feature which play role in training 
        /// </summary>
        public DataMappers.Column[] Features { get; set; }





        //Used statistics across data transformations
        public double[] Min { get; set; }
        public double[] Max { get; set; }
        public double[] Mean { get; set; }
        public double[] StDev { get; set; }



    }
}
