// Copyright (c) daenet GmbH / Frankfurt University of Applied Sciences. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LearningFoundation.EuclideanColorFilter;
using System.Drawing;

namespace LearningFoundation.EuclideanColorFilterTests
{
    [TestClass]
    public class CalcDistanceTests
    {
        /// <summary>
        /// This Unit Test calculates the distance between White (RGB 255,255,255) and Black (RGB 0,0,0) and Returns the Max Value Sqrt((255-0)²(255-0)²+(255-0)²) = 441,672955...
        ///  We have to use a delta because the float might make some trouble...
        /// </summary>
        [TestMethod]
        public void CalcDistance_Color1IsBlackColor2IsWhite_ReturnsMaxValue()
        {
            Color color1 = Color.Black;
            Color color2 = Color.White;

            float actual = CalcDistance.ComputeEuclideanDistance(color1, color2);
            Assert.AreEqual(441.672955930063709849498817084f, actual, 0.01f);
        }
        /// <summary>
        /// This Unit Test calculates the distance between White (RGB 255,255,255) and White (RGB 255,255,255) and Returns the Max Value Sqrt((255-0)²(255-0)²+(255-0)²) = 441,672955...
        ///  We have to use a delta because the float might make some trouble...
        /// </summary>
        [TestMethod]
        public void CalcDistance_Color1IsWhiteColor2IsWhite_ReturnsMaxValue()
        {
            Color color1 = Color.White;
            Color color2 = Color.White;

            float actual = CalcDistance.ComputeEuclideanDistance(color1, color2);
            Assert.AreEqual(0f, actual, 0.01f);
        }

        /// <summary>
        /// This Unit Test calculates the distance between Black (RGB 0,0,0) and Black (RGB 0,0,0) which should be 0 ((0-0)²+(0-0)²+(0-0)²) = 0
        /// </summary>
        [TestMethod]
        public void CalcDistance_Color1IsBlackColor2IsBlack_ReturnZero()
        {
            Color color1 = Color.Black;
            Color color2 = Color.Black;

            float actual = CalcDistance.ComputeEuclideanDistance(color1, color2);
            Assert.AreEqual(0f, actual, 0.01f);
        }

        /// <summary>
        /// This Unit Test calculates the distance between Red (RGB 255,0,0) and Blue (RGB 0,0,255) which should be 360.6244584... ((255-0)²+(0+0)²+(255-0)²) = 360.6244584...
        /// </summary>
        [TestMethod]
        public void CalcDistance_Color1IsBlueColor2IsRed_ReturnsMaxValue()
        {
            Color color1 = Color.Red;
            Color color2 = Color.Blue;

            float actual = CalcDistance.ComputeEuclideanDistance(color1, color2);
            Assert.AreEqual(360.6244584f, actual, 0.01f);
        }
        /// <summary>
        /// This Unit Test calculates the distance between Green (RGB 0,255,0) and Green (RGB 0,255,0) which should be 360.6244584... ((255-0)²+(0+0)²+(255-0)²) = 360.6244584...
        /// </summary>
        [TestMethod]
        public void CalcDistance_Color1IsGreenColor2IsGreen_ReturnsMaxValue()
        {
            Color color1 = Color.Green;
            Color color2 = Color.Green;

            float actual = CalcDistance.ComputeEuclideanDistance(color1, color2);
            Assert.AreEqual(0f, actual, 0.01f);
        }
        /// <summary>
        /// This Unit Test calculates the distance between Blue (RGB 0,0,255) and blue (RGB 0,0,255) which should be 360.6244584... ((255-0)²+(0+0)²+(255-0)²) = 360.6244584...
        /// </summary>
        [TestMethod]
        public void CalcDistance_Color1IsBlueColor2IsBlue_ReturnsMaxValue()
        {
            Color color1 = Color.Blue;
            Color color2 = Color.Blue;

            float actual = CalcDistance.ComputeEuclideanDistance(color1, color2);
            Assert.AreEqual(0f, actual, 0.01f);
        }
    }
}
