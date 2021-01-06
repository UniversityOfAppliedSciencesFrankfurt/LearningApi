using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DimensionalityReduction.PCA.Tests.Const
{
    /// <summary>
    /// Store some constanse for the test method
    /// </summary>
    class TestConstants
    {
        /// <summary>
        /// epsilon we defined as 10E-5
        /// </summary>
        public const double Epsilon = 0.00001;

        /// <summary>
        /// Input data used for Unit test cases
        /// </summary>
        public static string INPUT_DATA_DIR = Path.Combine(AppContext.BaseDirectory, "InputData");
        
        /// <summary>
        /// Output data for comparing test result
        /// </summary>
        public static string OUTPUT_RESULT_DIR = Path.Combine(AppContext.BaseDirectory, "OutputResult");
    }
}
