using LearningFoundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sobel_Detection;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;


namespace MysobelUnitTest
{

    [TestClass]
    public class Test1
    {   
        /// <summary>
        /// creating objet of converion helper class
        /// </summary>
        ConversionHelper helper = new ConversionHelper();


        /// <summary>
        /// This method is used to Test the Algorithm by taking Input image as grayscale image(InputGray-scale.jpg) by setting boolean parameter Grayscale=false in Algorithm. 
        /// Bitmap image(InputGray-scale.jpg) will be loaded from TestInputimages folder and converted to double[,,]. 
        /// After that sobel  algorithm will be executed.Then the result image will be converted back to Bitmap and saved  in TestOutputImages folder as grayscaleoutput1.jpg.
        /// </summary>
        [TestMethod]
        public void SobelFilterTestForGrayscaleImage()
        {

            LearningApi api = new LearningApi();

            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
             {

                 string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                 string path = Path.Combine(baseDirectory, "TestInputimages\\InputGray-scale.jpg");//path to bin directory of project
                 Bitmap bitmap = new Bitmap(path);   
                 double[,,] data = helper.ConvertBitmapToDouble(bitmap);// convert bitmap to double
                 return data;

             });
            api.AddModule(new SobelConvolutionFilter());
            double[,,] result = api.Run() as double[,,];

            Bitmap bitresult = helper.ConvertDoubleToBitmap(result);// convert double to bitmap

            string baseDirectory2 = AppDomain.CurrentDomain.BaseDirectory;
            string outpath = baseDirectory2 + "\\TestOutputImages\\";
            if (!Directory.Exists(outpath))
            {
                Directory.CreateDirectory(outpath);
            }

            bitresult.Save(outpath + "grayscaleoutput1.jpg");

        }


        /// <summary>
        /// This method is used to Test the Algorithm by taking Input Image as colored Image without converting to Grayscale Image (setting boolean parameter Grayscale=false in Algorithm.) .
        /// Bitmap image(TIM2.jpg) will be loaded from TestInputimages folder and converted to double[,,]. After that sobel  algorithm will be executed.
        /// Then the result image will be converted back to Bitmap and saved  in TestOutputImages folder as output2.jpg.
        /// </summary>
        [TestMethod]
        public void SobelFilterTestForColoredImage()
        {

            LearningApi api = new LearningApi();

            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(baseDirectory, "TestInputimages\\TIM2.jpg");//path to bin directory of project
                Bitmap bitmap = new Bitmap(path);
                double[,,] data = helper.ConvertBitmapToDouble(bitmap);// convert bitmap to double
                return data;

            });
            api.AddModule(new SobelConvolutionFilter());
            double[,,] result = api.Run() as double[,,];

            Bitmap bitresult = helper.ConvertDoubleToBitmap(result);// convert double to bitmap

            string baseDirectory2 = AppDomain.CurrentDomain.BaseDirectory;
            string outpath = baseDirectory2 + "\\TestOutputImages\\";
            if (!Directory.Exists(outpath))
            {
                Directory.CreateDirectory(outpath);
            }

            bitresult.Save(outpath + "Output2.jpg");

        }

        /// <summary>
        /// This method is used to Test the Algorithm by taking InputImage(TIM3.jpg) as colored image and converting it to Grayscale (setting boolean parameter Grayscale=true in Algorithm.). 
        /// Bitmap image will be loaded from TestInputimages folder and converted to double[,,]. After that sobel  algorithm will be executed.
        /// Then the result image will be converted back to Bitmap and saved  in TestOutputImages folder as Output3.jpg.
        /// </summary>
        [TestMethod]
        public void SobelFilterTestForGrayscaleConversionImage()
        {

            LearningApi api = new LearningApi();

            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(baseDirectory, "TestInputimages\\TIM3.jpg");//path to bin directory of project
                Bitmap bitmap = new Bitmap(path);
                double[,,] data = helper.ConvertBitmapToDouble(bitmap);// convert bitmap to double
                return data;

            });
            api.AddModule(new SobelConvolutionFilter());
            double[,,] result = api.Run() as double[,,];

            Bitmap bitresult = helper.ConvertDoubleToBitmap(result);// convert double to bitmap

            string baseDirectory2 = AppDomain.CurrentDomain.BaseDirectory;
            string outpath = baseDirectory2 + "\\TestOutputImages\\";
            if (!Directory.Exists(outpath))
            {
                Directory.CreateDirectory(outpath);
            }

            bitresult.Save(outpath + "Output3.jpg");

        }

        /// <summary>
        /// This method is used to Test the Algorithm by taking InputImage file type as .Png(TIM4.png)and Grayscale =true in Algorithm. Bitmap image will be loaded from TestInputimages folder and converted to double[,,]. After that sobel  algorithm will be executed.
        /// Then the result image will be converted back to Bitmap and saved  in TestOutputImages folder as output4.png.
        /// </summary>
        [TestMethod]
        public void SobelFilterTestForPngImageFileType()
        {

            LearningApi api = new LearningApi();

            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(baseDirectory, "TestInputimages\\TIM4.png");//path to bin directory of project
                Bitmap bitmap = new Bitmap(path);
                double[,,] data = helper.ConvertBitmapToDouble(bitmap);// convert bitmap to double
                return data;

            });
            api.AddModule(new SobelConvolutionFilter());
            double[,,] result = api.Run() as double[,,];

            Bitmap bitresult = helper.ConvertDoubleToBitmap(result);// convert double to bitmap

            string baseDirectory2 = AppDomain.CurrentDomain.BaseDirectory;
            string outpath = baseDirectory2 + "\\TestOutputImages\\";
            if (!Directory.Exists(outpath))
            {
                Directory.CreateDirectory(outpath);
            }

            bitresult.Save(outpath + "Output4.png");

        }

        /// <summary>
        /// This method is used to Test the Algorithm by taking InputImage as Jpeg File type(TIM5.jpeg) and Grayscale=true in Alorithm. Bitmap image will be loaded from TestInputimages folder and converted to double[,,]. After that sobel  algorithm will be executed.
        /// Then the result image will be converted back to Bitmap and saved  in TestOutputImages folder as output5.jpeg.
        /// </summary>
        [TestMethod]
        public void SobelFilterTestForJpegImagefileType()
        {

            LearningApi api = new LearningApi();

            api.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(baseDirectory, "TestInputimages\\TIM5.jpeg");//path to bin directory of project
                Bitmap bitmap = new Bitmap(path);
                double[,,] data = helper.ConvertBitmapToDouble(bitmap);// convert bitmap to double
                return data;

            });
            api.AddModule(new SobelConvolutionFilter());
            double[,,] result = api.Run() as double[,,];

            Bitmap bitresult = helper.ConvertDoubleToBitmap(result);// convert double to bitmap

            string baseDirectory2 = AppDomain.CurrentDomain.BaseDirectory;
            string outpath = baseDirectory2 + "\\TestOutputImages\\";
            if (!Directory.Exists(outpath))
            {
                Directory.CreateDirectory(outpath);
            }

            bitresult.Save(outpath + "Output5.jpeg");

        }
    }
}


