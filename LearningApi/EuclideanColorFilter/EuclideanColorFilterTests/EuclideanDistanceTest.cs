using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation.EuclideanColorFilter;
using System.Drawing;
using System.Linq;
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
        }
}

