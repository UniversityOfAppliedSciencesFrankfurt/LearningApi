using LearningFoundation.EuclideanColorFilter;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace LearningFoundation.EuclideanColorFilterTests
{

        [TestClass]
        public class TestDistance
        {
        

        [TestMethod]
            public void Test_Euclidean()
            {
                var p1 = new Point(5, 4);
                var p2 = new Point(4, 1);

                Assert.AreEqual(3.1622776601683795, Distance.Euclidean(p1, p2));

            }
            [TestMethod]
            public void Test_EuclideanSimilarity()
            {
                var p1 = new Point(5, 4);
                var p2 = new Point(4, 1);

                Assert.AreEqual(0.2402530733520421, Distance.EuclideanSimilarity(p1, p2));

            }
        [TestMethod]
        public void Test_GetDistance()
        {
            var current = new Color() ;
            var match = new Color();

            Assert.AreEqual(0.0f, Distance.GetDistance(current, match));
        }
        [TestMethod]
        public void Test_FindNearestColor()
        {
            var map = new Color();
            var current = new Color();

            Assert.AreEqual(0.0f, Distance.FindNearestColor(map, current));
        }
    }
}

