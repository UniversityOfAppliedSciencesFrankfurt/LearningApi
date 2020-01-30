
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EuclideanFilter;
using System.Drawing;

namespace EuclideanColorFilterTests
{
    [TestClass]
    public class CalcDistanceTests
    {
        /// <summary>
        /// This Unit Test calculates the distance between White (RGB 255,255,255) and Black (RGB 0,0,0) and Returns the Max Value Sqrt((255-0)�(255-0)�+(255-0)�) = 441,672955...
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
        /// This Unit Test calculates the distance between Black (RGB 0,0,0) and Black (RGB 0,0,0) which should be 0 ((0-0)�+(0-0)�+(0-0)�) = 0
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
        /// This Unit Test calculates the distance between Red (RGB 255,0,0) and Blue (RGB 0,0,255) which should be 360.6244584... ((255-0)�+(0+0)�+(255-0)�) = 360.6244584...
        /// </summary>
        [TestMethod]
        public void CalcDistance_Color1IsBlueColor2IsRed_ReturnsMaxValue()
        {
            Color color1 = Color.Red;
            Color color2 = Color.Blue;

            float actual = CalcDistance.ComputeEuclideanDistance(color1, color2);
            Assert.AreEqual(360.6244584f, actual, 0.01f);
        }
    }
}
