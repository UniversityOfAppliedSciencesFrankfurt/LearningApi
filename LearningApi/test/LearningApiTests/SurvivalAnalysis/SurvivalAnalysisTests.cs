using test;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using LearningFoundation;
using LogisticRegression;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;
using LearningFoundation.Normalizers;
using LearningFoundation.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LearningFoundation.Test.SurvivalAnalysis
{
    [TestClass]
    public class SurvivalAnalysisTests
    {

        public SurvivalAnalysisTests()
        {

        }

        /// <summary>
        /// Performs the SurvivalAnalysis on specified dataset with 10 iteration and 0.15 learning rate.
        /// </summary>
        [TestMethod]
        public void SurvivalAnalysis_Tests_iterations_10_learningrate_013()
        {
            
           // Assert.Equal(Math.Round(score.Weights[6], 5), -0.85624);
        }

      
    }
}
