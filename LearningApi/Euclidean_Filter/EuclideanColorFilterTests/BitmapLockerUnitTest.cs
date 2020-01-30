using System.Drawing;
using EuclideanFilter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LearningFoundation;
using System.IO;
using System;
using Microsoft.VisualStudio.Imaging;

namespace EuclideanColorFilterTests
{
    [TestClass]
    public class BitmapLockerUnitTest
    {
        Bitmap bitmap = (Bitmap)Bitmap.FromFile(@"C:\lena.png");
        BitmapLocker locker;

        public BitmapLockerUnitTest()
        {
            locker = new BitmapLocker(bitmap);
        }

        [TestMethod]
        public void SetPixel__Test()
        {
            Bitmap copy = (Bitmap)bitmap.Clone();
            int xx = copy.Width / 2;
            int yy = copy.Height / 2;

            Color c = Color.Black;

            locker.Lock();
            locker.SetPixel(xx, yy, c);
            Assert.AreEqual(c, locker.GetPixel(xx, yy));
        }
    }
}
