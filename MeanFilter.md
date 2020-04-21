# Mean filter :
It is a method of smoothing images reducing the amount of intensity variation between one pixel and the next. It is often used to reduce noise in images

## How it works?
It replaces each pixel value in an image with the mean or average value of its neighbors, including itself. It eliminates the pixel values which are unrepresentative of their surroundings. Mean filtering is also as a convolution filter, which is based around a kernel representing the shape and size of the neighborhood to be sampled when calculating the mean.
 
Often a 3×3 square kernel is used, larger kernels can be used for severe smoothing. And also small kernel can be applied more than once in order to produce a similar but not identical effect as a single pass with a large kernel. 

~~~csharp

        /// <summary>
        /// Test to verify successful application of Mean filter on an image.
        /// </summary>
        [DataTestMethod]
        [DataRow("/TestPicture/test1.gif", "/TestPicture/test1.gif")]
        [DataRow("/TestPicture/test2.jpg", "/ExpectedPicture/Mean/test2.jpg")]
        [DataRow("/TestPicture/test3.png", "/ExpectedPicture/Mean/test3.png")]
        [DataRow("/TestPicture/test4.png", "/ExpectedPicture/Mean/test4.png")]
        [DataRow("/TestPicture/test5.jpg", "/ExpectedPicture/Mean/test5.jpg")]
        [DataRow("/TestPicture/test6.jpg", "/ExpectedPicture/Mean/test6.jpg")]
        [DataRow("/TestPicture/test7.jpg", "/ExpectedPicture/Mean/test7.jpg")]
        public void MeanFilter_ImageBlur_FilterSuccessfullyApplied(string inputImageFileName, string expectedImageFileName)
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule((Func<double[,,], IContext, double[,,]>)((input, ctx) => 
                GetDataArrayFromImage(inputImageFileName)));

            lApi.AddModule(new MeanFilter());

            double[,,] result = lApi.Run() as double[,,];

            ValidateBitmap(result, expectedImageFileName);
        }

~~~
 ### Link to test case: https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/test/LearningApiTests/GaussianAndMeanFilter/GaussianAndMeanFilterTest.cs 
