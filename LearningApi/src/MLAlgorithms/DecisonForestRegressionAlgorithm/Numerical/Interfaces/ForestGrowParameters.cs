using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest.Lib.Numerical.Interfaces
{
    /// <summary>
    /// ForestGrowParameters
    /// </summary>
    public class ForestGrowParameters
    {
        private SplitMode _splitMode = SplitMode.RSS;

        /// <summary>
        /// Path to Microsoft Excel file (*.xlsx) contained data to learn and build trees 
        /// </summary>
        public string TrainingDataPath { get; set; }

        /// <summary>
        /// Number of trees to grow
        /// </summary>
        public int TreeCount { get; set; }

        /// <summary>
        /// Name of the feature to make classification by
        /// </summary>
        public string ResolutionFeatureName { get; set; }

        /// <summary>
        /// Max number of items in a category while classifying
        /// </summary>
        public int MaxItemCountInCategory { get; set; }

        /// <summary>
        /// Ratio to define number of items from learn data subset for each tree
        /// </summary>
        public float ItemSubsetCountRatio { get; set; }

        /// <summary>
        /// Export trees to directory in Json format
        /// </summary>
        public bool ExportToJson { get; set; }

        /// <summary>
        /// Path to directory for Json files
        /// </summary>
        public string ExportDirectoryPath { get; set; }
        /// <summary>
        /// /SplitMode
        /// </summary>
        public SplitMode SplitMode
        {
            get { return _splitMode; }
            set { _splitMode = value; }
        }
        /// <summary>
        /// data of type double[][]
        /// </summary>
        public double[][] data { get; set; }
        
    }
}
