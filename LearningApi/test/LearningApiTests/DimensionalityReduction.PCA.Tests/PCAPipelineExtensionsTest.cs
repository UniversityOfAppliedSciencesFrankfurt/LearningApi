using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DimensionalityReduction.PCA.Tests.Const;
using LearningFoundation;
using LearningFoundation.DimensionalityReduction.PCA;
using LearningFoundation.DimensionalityReduction.PCA.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DimensionalityReduction.PCA.Tests
{
    /// <summary>
    /// Test the PCAPipelineModule when intergrated with LearningApi
    /// </summary>
    [TestClass]
    public class PCAPipelineExtensionsTest
    {
        private static GeneralLinearAlgebraUtils Linag = GeneralLinearAlgebraUtils.getImpl();

        /// <summary>
        /// Test the PCAPipelineModule when intergrated with LearningApi
        /// </summary>
        [TestMethod]
        public void Test_PCAPipelineModuleIntergratedWithLearningAPI() {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                string filePath = Path.Combine(TestConstants.INPUT_DATA_DIR, "face1.pgm.csv");
                double[][] data = CsvUtils.ReadCSVToDouble2DArray(filePath, ',');
                return data;
            });

            api.UsePCAPipelineModule(moduleName: "PCAPipelineModule", newDimensionSize: 10);
            api.Run();

            PCAPipelineModule moduleInstance = api.GetModule<PCAPipelineModule>("PCAPipelineModule");
            double[][] explainedVariance = new double[2][];
            explainedVariance[0] = moduleInstance.ExplainedVariance;
            explainedVariance[1] = moduleInstance.CummulativeExplainedVariance;

            CsvUtils.WriteCSVFromDouble2DArray(Path.Combine(TestConstants.OUTPUT_RESULT_DIR, "ExplainedVarianceResult_face1.csv"), ',', Linag.Transpose(explainedVariance));
        }
    }
}
