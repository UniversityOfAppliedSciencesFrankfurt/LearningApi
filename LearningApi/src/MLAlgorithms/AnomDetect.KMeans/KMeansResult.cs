using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomDetect.KMeans
{
    public class KMeansResult : IResult
    {
        public int[] PredictedClusters { get; set; }
    }
}
