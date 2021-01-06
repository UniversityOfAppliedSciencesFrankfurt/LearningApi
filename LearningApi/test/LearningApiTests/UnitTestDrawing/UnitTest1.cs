//using LearningFoundation;
//using LearningFoundation.DataMappers;
//using LearningFoundation.DataProviders;
//using LearningFoundation.Normalizers;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using MyDrawingModule;
//using System;
//using System.Collections.Generic;
//using System.IO;

//namespace UnitTestDrawing
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        [TestMethod]
        
//            public void TestMethod1()
//            {
//                var file_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"HTML folder\data.csv");

//                var desc = (DataDescriptor)LoadMetaData();
//                LearningApi api = new LearningApi(desc);

//                //Using CSV Data Provider Module
//                api.UseCsvDataProvider(file_path, ',', false, 1);

//                // Use mapper for data, which will extract (map) required columns 
//                api.UseDefaultDataMapper();

//                //Using some Normalizer Module
//                api.UseMinMaxNormalizer();

//                //Using LearningApi Algorithm
//                api.UseLearningApiAlgorithm();

//                //Training
//                LearningApiAlgorithm score = api.Run() as LearningApiAlgorithm;

//                //Prediction            
//                var predictingData = new double[2][] {
//                    new double[]{ 25, 18 },
//                    new double[]{ 32, 23 }
//                    };

//                LearningApiAlgorithmResult result = api.Algorithm.Predict(predictingData, api.Context) as LearningApiAlgorithmResult;

//                Assert.AreEqual(Math.Round(result.PredictedValue[0]), 18);
//                Assert.AreEqual(Math.Round(result.PredictedValue[1]), 23);
//            }

//        private object LoadMetaData()
//        {
//            throw new NotImplementedException();
//        }
//    }

        
      
//    }
//todo
