using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomalyDetectionK
{
    public class AnomalyDetectionAlgorithmResult : IResult
    {
        /// <summary>
        /// List of results. -1 means no result for slot.
        /// </summary>
        public int[] Results { get; set; }
    }
}