using LearningFoundation;
using MLPerceptron;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Globalization;
using NeuralNet.MLPerceptron;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageBinarizerLib;

namespace LearningFoundation.Test.MLPerceptron
{
    /// <summary>
    /// Class MLPerceptronUnitTests contains the unit test cases to test the ML Perceptron algorithm
    /// </summary>
    [TestClass]
    public class ImageBinarizationTests
    {
        
        /// <summary>
        /// Loads a single JPG and create a binarized version with zeros and ones.
        /// Look after executing of test for file binary.txt.
        /// </summary>
        [TestMethod]
        public void BinarizerTest()
        {
            //todo: fix
            //ImageBinarizer bizer = new ImageBinarizer();

            //foreach (var item in new string[] { "face.jpg", "daenet.png", "gab.png"})
            //{
            //    string trainingImagesPath = Path.Combine(Path.Combine(AppContext.BaseDirectory, "ImageBinarization"), "TestImages");
             
            //    bizer.CreateBinary(Path.Combine(trainingImagesPath, item), $"binary_{item}.txt");
            //}
        }


        /// <summary>
        /// Loads a single JPG and create a binarized version with zeros and ones in specified size, which is not image original size.
        /// </summary>
        [TestMethod]
        public void BinarizerWithTargetSizeTest()
        {
            //tod: fix
            //ImageBinarizer bizer = new ImageBinarizer(targetHeight: 256, targetWidth: 256 );

            //foreach (var item in new string[] { "face.jpg", "daenet.png", "gab.png" })
            //{
            //    string trainingImagesPath = Path.Combine(Path.Combine(AppContext.BaseDirectory, "ImageBinarization"), "TestImages");

            //    //bizer.CreateBinary(Path.Combine(trainingImagesPath, item), $"binary__256x256_{item}.txt");
            //}
        }
    }

}




