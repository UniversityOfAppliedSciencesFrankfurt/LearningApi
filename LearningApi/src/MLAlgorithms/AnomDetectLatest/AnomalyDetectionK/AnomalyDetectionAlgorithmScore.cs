using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomalyDetectionK
{
    
    public class  AnomalyDetectionAlgorithmScore : IScore
    {
        /// <summary>
        /// store train data
        /// </summary>

        public double[][] OldData { get; set; }
        /// <summary>
        /// store clusters
        /// </summary>
        public int[] cl { get; set; }

    }
}