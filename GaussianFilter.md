# Gaussian filter :
The Gaussian smoothing operator is a 2-D convolution operator that is used to 'blur' images and remove detail and noise. In this sense it is similar to the mean filter, but it uses a different kernel that represents the shape of a Gaussian ('bell-shaped') hump.

## How it works?
The effect of Gaussian smoothing is to blur an image, in a similar fashion to the mean filter. The degree of smoothing is determined by the standard deviation of the Gaussian. (Larger standard deviation Gaussians, of course, require larger convolution kernels in order to be accurately represented.)

The Gaussian outputs a 'weighted average' of each pixel's neighborhood, with the average weighted more towards the value of the central pixels. This is in contrast to the mean filter's uniformly weighted average. Because of this, a Gaussian provides gentler smoothing and preserves edges better than a similarly sized mean filter. 

The idea of Gaussian smoothing is to use this 2-D distribution as a 'point-spread' function, and this is achieved by convolution. Since the image is stored as a collection of discrete pixels we need to produce a discrete approximation to the Gaussian function before we can perform the convolution. 

~~~csharp

        /// <summary>
        /// Test to verify successful application of Gaussian filter on an image.
        /// </summary>
        [DataTestMethod]
        [DataRow("/TestPicture/test1.gif", "/TestPicture/test1.gif")]
        [DataRow("/TestPicture/test2.jpg", "/ExpectedPicture/Gaussian/test2.jpg")]
        [DataRow("/TestPicture/test3.png", "/ExpectedPicture/Gaussian/test3.png")]
        [DataRow("/TestPicture/test4.png", "/ExpectedPicture/Gaussian/test4.png")]
        [DataRow("/TestPicture/test5.jpg", "/ExpectedPicture/Gaussian/test5.jpg")]
        [DataRow("/TestPicture/test6.jpg", "/ExpectedPicture/Gaussian/test6.jpg")]
        [DataRow("/TestPicture/test7.jpg", "/ExpectedPicture/Gaussian/test7.jpg")]
        public void GaussianFilter_ImageBlur_FilterSuccessfullyApplied(string inputImageFileName, string expectedImageFileName)
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule((Func<double[,,], IContext, double[,,]>)((input, ctx) =>
                GetDataArrayFromImage(inputImageFileName)));

            lApi.AddModule(new GaussianFilter());

            double[,,] result = lApi.Run() as double[,,];

            ValidateBitmap(result, expectedImageFileName);
        }

~~~
 ### Link to test case: https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/test/LearningApiTests/GaussianAndMeanFilter/GaussianAndMeanFilterTest.cs 
