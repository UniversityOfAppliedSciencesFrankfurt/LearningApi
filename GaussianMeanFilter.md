# Gaussian mean filter :
It is a method of smoothing images reducing the amount of intensity variation between one pixel and the next. It is often used to reduce noise in images
## How it works?
It replaces each pixel value in an image with the mean or average value of its neighbors, including itself. It eliminates the pixel values which are unrepresentative of their surroundings. Mean filtering is also as a convolution filter, which is based around a kernel representing the shape and size of the neighborhood to be sampled when calculating the mean.
 
Often a 3×3 square kernel is used, larger kernels can be used for severe smoothing. And also small kernel can be applied more than once in order to produce a similar but not identical effect as a single pass with a large kernel. 

~~~csharp
//code is vomitted fpr simplicitz
```/// <summary>
        /// Unit test for Mean Filter
        /// </summary>
        [Fact]
        public void MeanF()
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                Bitmap myBitmap = new Bitmap($"{Directory.GetCurrentDirectory()}/TestPicture/test.gif");

                double[,,] data = new double[myBitmap.Width, myBitmap.Height, 3];

                for (int x = 0; x < myBitmap.Width; x++)
                {
                    for (int y = 0; y < myBitmap.Height; y++)
                    {
                        Color pixelColor = myBitmap.GetPixel(x, y);

                        data[x, y, 0] = pixelColor.R;
                        data[x, y, 1] = pixelColor.G;
                        data[x, y, 2] = pixelColor.B;
                    }
                }
                return data;
            });

            lApi.AddModule(new MeanFilter());

            double[,,] result = lApi.Run() as double[,,];

            Assert.True(result != null);

            Bitmap blurBitmap = new Bitmap(result.GetLength(0), result.GetLength(1));

            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int y = 0; y < result.GetLength(1); y++)
                {
                    Color pixelColor = Color.FromArgb((int)result[x, y, 0], (int)result[x, y, 1], (int)result[x, y, 2]);

                    blurBitmap.SetPixel(x, y, pixelColor);
                }
            }
~~~
 ### Link to test case: https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/test/GaussianAndMeanFilter
