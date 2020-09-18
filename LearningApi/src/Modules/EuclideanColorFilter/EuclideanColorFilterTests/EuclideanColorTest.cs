using LearningFoundation.EuclideanColorFilter;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LearningFoundation.EuclideanColorFilterTests
{

    [TestClass]
    public class TestDistance
    {
        /// <summary>
        /// This Unit Test calculates the distance between assumed data point(pixel)
        /// This test returns the Max Value Sqrt((5-4)²+(4-1)²) = 3.1622776601683
        ///  We have to use a delta because the float might make some trouble...
        /// Point = any pixel from image
        /// </summary>
        [TestMethod]
        public void EuclideanFormulaTest()
        {
            var p1 = new Point(5, 4);
            var p2 = new Point(4, 1);

            Assert.AreEqual(3.1622776601683795, Distance.EuclideanFormula(p1, p2));

        }

        /// <summary>
        /// This Unit Test calculates the distance between assumed data point(pixel) using Euclidean Distance similaity
        /// This test returns the (1/(1+EuclideanDistance(P1,P2)) = 1/(1+3.1622...) = 0.2402530733520
        ///  We have to use a delta because the float might make some trouble...
        ///  Point = any pixel from image
        /// </summary>
        [TestMethod]
        public void EuclideanSimilarityTest()
        {
            var p1 = new Point(5, 4);
            var p2 = new Point(4, 1);

            Assert.AreEqual(0.2402530733520421, Distance.EuclideanSimilarity(p1, p2));
        }

        /// <summary>
        /// This Unit Test calculates the distance between current color and match color
        /// If code retruns the 0.0f value then as per Euclidean Distance formula test will passed to show that both current and match color are same.
        ///  We have to use a delta because the float might make some trouble...
        ///  Color = Any color of pixel from image
        ///  </summary>

        [TestMethod]
        public void ColorMatchingTest()
        {
            var current = new Color();
            var match = new Color();

            Assert.AreEqual(0.0f, Distance.ColorMatching(current, match));
        }

        /// <summary>
        /// This Unit Test calculates the nearest color to the current color from various match colors.
        /// If code retruns the 0.0f value then as per Euclidean Distance formula test will passed to show that selected match color is nearest color of current color.
        ///  We have to use a delta because the float might make some trouble...
        /// Color = Any color of pixel from image 
        /// </summary>

        [TestMethod]
        public void FindNearestColorTest()
        {
            var map = new Color[1];
            var current = new Color(); 

            Assert.AreEqual(0.0f, Distance.FindNearestColor(map, current));
        }

    }
}

