using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LearningApiTests.GaussianAndMeanFilter
{
    public static class BitmapExtensions
    {
        public static byte[] GetBytes(this Bitmap bitmap)
        {
            var bytes = new byte[bitmap.Height * bitmap.Width * 3];
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            Marshal.Copy(bitmapData.Scan0, bytes, 0, bytes.Length);
            bitmap.UnlockBits(bitmapData);
            return bytes;
        }
    }
}
