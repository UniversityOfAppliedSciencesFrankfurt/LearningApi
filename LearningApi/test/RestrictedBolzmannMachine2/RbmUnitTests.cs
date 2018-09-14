using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using NeuralNet.Perceptron;
using NeuralNet.RestrictedBolzmannMachine2;
using System.IO;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using ImageBinarizer;

namespace test.RestrictedBolzmannMachine
{

    public class RbmUnitTests
    {

        static RbmUnitTests()
        {
            
        }

        [Fact]
        public void TestBinarization()
        {
            var images = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "RestrictedBolzmannMachine2\\TrainingImages"));

            foreach (var item in images)
            {
                appendImageBinary(item, "binarized.txt");
            }

        }

        /// <summary>
        /// RBM is not supervised algorithm. This is why we do not have a label.
        /// </summary>
        /// <returns></returns>
        private DataDescriptor getDescriptorForRbm_sample1()
        {
            DataDescriptor des = new DataDescriptor();
            des.Features = new LearningFoundation.DataMappers.Column[6];

            // Label not used.
            des.LabelIndex = -1;

            des.Features = new Column[6];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[2] = new Column { Id = 3, Name = "col3", Index = 2, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[3] = new Column { Id = 4, Name = "col4", Index = 3, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[4] = new Column { Id = 5, Name = "col5", Index = 4, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[5] = new Column { Id = 6, Name = "col6", Index = 5, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };

            return des;
        }
 
        /// <summary>
        /// RBM is not supervised algorithm. This is why we do not have a label.
        /// </summary>
        /// <returns></returns>
        private DataDescriptor getDescriptorForRbmTwoClassesClassifier()
        {
            DataDescriptor des = new DataDescriptor();
            des.Features = new LearningFoundation.DataMappers.Column[10];

            // Label not used.
            des.LabelIndex = -1;

            des.Features = new Column[10];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[2] = new Column { Id = 3, Name = "col3", Index = 2, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[3] = new Column { Id = 4, Name = "col4", Index = 3, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[4] = new Column { Id = 5, Name = "col5", Index = 4, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[5] = new Column { Id = 6, Name = "col6", Index = 5, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[6] = new Column { Id = 7, Name = "col7", Index = 6, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[7] = new Column { Id = 8, Name = "col8", Index = 7, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[8] = new Column { Id = 9, Name = "col9", Index = 8, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[9] = new Column { Id = 10, Name = "col10", Index = 9, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
           
            return des;
        }

        /// <summary>
        /// Movies:
        /// 
        /// </summary>
        [Fact]
        public void RBMRecoomendationTest()
        {

        }


        [Fact]
        public void SimpleRBMTest()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 12;
                ctx.DataDescriptor = getDescriptorForRbm_sample1();
                double[][] data = new double[maxSamples][];

                data[0] = new double[] { 1, 1, 0, 0, 0, 0 };  // A
                data[1] = new double[] { 0, 0, 1, 1, 0, 0 };  // B
                data[2] = new double[] { 0, 0, 0, 0, 1, 1 };  // C
                
                data[3] = new double[] { 1, 1, 0, 0, 0, 1 };  // noisy A
                data[4] = new double[] { 0, 0, 1, 1, 0, 0 };  // BRt
                data[5] = new double[] { 0, 0, 0, 0, 1, 1 };  // C
                
                data[6] = new double[] { 1, 0, 0, 0, 0, 0 };  // weak A
                data[7] = new double[] { 0, 0, 1, 0, 0, 0 };  // weak B
                data[8] = new double[] { 0, 0, 0, 0, 1, 0 };  // weak C
                
                data[9] = new double[] { 1, 1, 0, 1, 0, 0 };  // noisy A
                data[10] = new double[] { 1, 0, 1, 1, 0, 0 };  // noisy B
                data[11] = new double[] { 0, 0, 1, 0, 1, 1 };  // noisy C
                return data;
            });

            
            api.UseRbm(0.01, 1000, 6,3);

            RbmScore score = api.Run() as RbmScore;

            double[][] testData = new double[4][];

            Assert.True(score.Loss < 1.0);

            testData[0] = new double[] { 1, 1, 0, 0, 0, 0 };
            testData[1] = new double[] { 0, 0, 0, 0, 1, 1 };
            testData[2] = new double[] { 0, 1, 0, 0, 0, 0 };
            testData[3] = new double[] { 0, 0, 0, 0, 1, 0 };

            var result = api.Algorithm.Predict(testData, api.Context);

            // NOT FINISHED.
            //Assert.True(result[0] == 1);
            //Assert.True(result[1] == 0);
            //Assert.True(result[2] == 0);
            //Assert.True(result[3] == 0);
            //Assert.True(result[4] == 1);
            //Assert.True(result[5] == 0);
        }

        [Fact]
        public void SimpleRBMDeepTest()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 12;
                ctx.DataDescriptor = getDescriptorForRbm_sample1();
                double[][] data = new double[maxSamples][];

                data[0] = new double[] { 1, 1, 0, 0, 0, 0 };  // A
                data[1] = new double[] { 0, 0, 1, 1, 0, 0 };  // B
                data[2] = new double[] { 0, 0, 0, 0, 1, 1 };  // C

                data[3] = new double[] { 1, 1, 0, 0, 0, 1 };  // noisy A
                data[4] = new double[] { 0, 0, 1, 1, 0, 0 };  // BRt
                data[5] = new double[] { 0, 0, 0, 0, 1, 1 };  // C

                data[6] = new double[] { 1, 0, 0, 0, 0, 0 };  // weak A
                data[7] = new double[] { 0, 0, 1, 0, 0, 0 };  // weak B
                data[8] = new double[] { 0, 0, 0, 0, 1, 0 };  // weak C

                data[9] = new double[] { 1, 1, 0, 1, 0, 0 };  // noisy A
                data[10] = new double[] { 1, 0, 1, 1, 0, 0 };  // noisy B
                data[11] = new double[] { 0, 0, 1, 0, 1, 1 };  // noisy C
                return data;
            });


            api.UseRbm(0.01, 1000, 6, 4);
           
            RbmScore score = api.Run() as RbmScore;

            double[][] testData = new double[4][];

            Assert.True(score.Loss < 1.0);

            testData[0] = new double[] { 1, 1, 0, 0, 0, 0 };
            testData[1] = new double[] { 0, 0, 0, 0, 1, 1 };
            testData[2] = new double[] { 0, 1, 0, 0, 0, 0 };
            testData[3] = new double[] { 0, 0, 0, 0, 1, 0 };

            var result = api.Algorithm.Predict(testData, api.Context);

            // NOT FINISHED.
            //Assert.True(result[0] == 1);
            //Assert.True(result[1] == 0);
            //Assert.True(result[2] == 0);
            //Assert.True(result[3] == 0);
            //Assert.True(result[4] == 1);
            //Assert.True(result[5] == 0);
        }

        [Fact]
        public void RBMDataSample1Test()
        {
            var dataPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\rbm_sample1.csv");

            LearningApi api = new LearningApi(this.getDescriptorForRbm_sample1());
           
            // Initialize data provider
            api.UseCsvDataProvider(dataPath, ',', false, 1);
            api.UseDefaultDataMapper();
            api.UseRbm(0.2, 1000, 6, 3);

            RbmResult score = api.Run() as RbmResult;
           
            double[][] testData = new double[4][];

            testData[0] = new double[] { 1, 1, 0, 0, 0, 0 };
            testData[1] = new double[] { 0, 0, 0, 0, 1, 1 };
            testData[2] = new double[] { 0, 1, 0, 0, 0, 0 };
            testData[3] = new double[] { 0, 0, 0, 0, 1, 1 };

            var result = api.Algorithm.Predict(testData, api.Context);

            // NOT FINISHED.
            //Assert.True(result[0] == 1);
            //Assert.True(result[1] == 0);
            //Assert.True(result[2] == 0);
            //Assert.True(result[3] == 0);
            //Assert.True(result[4] == 1);
            //Assert.True(result[5] == 0);
        }

        /// <summary>
        /// NOT USED.
        /// </summary>
        [Fact]
        public void LinearEquationSolver()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 12;
                ctx.DataDescriptor = getDescriptorForRbm_sample1();
                double[][] data = new double[maxSamples][];

                data[0] = new double[] { 1, 1, 0, 0, 0, 0 };  // A
                data[1] = new double[] { 0, 0, 1, 1, 0, 0 };  // B
                data[2] = new double[] { 0, 0, 0, 0, 1, 1 };  // C

                data[3] = new double[] { 1, 1, 0, 0, 0, 1 };  // noisy A
                data[4] = new double[] { 0, 0, 1, 1, 0, 0 };  // B
                data[5] = new double[] { 0, 0, 0, 0, 1, 1 };  // C

                data[6] = new double[] { 1, 0, 0, 0, 0, 0 };  // weak A
                data[7] = new double[] { 0, 0, 1, 0, 0, 0 };  // weak B
                data[8] = new double[] { 0, 0, 0, 0, 1, 0 };  // weak C

                data[9] = new double[] { 1, 1, 0, 1, 0, 0 };  // noisy A
                data[10] = new double[] { 1, 0, 1, 1, 0, 0 };  // noisy B
                data[11] = new double[] { 0, 0, 1, 0, 1, 1 };  // noisy C
                return data;
            });


            api.UseRbm(0.2, 1000, 6, 3);

            IScore score = api.Run() as IScore;

            double[][] testData = new double[4][];

            testData[0] = new double[] { 1, 1, 0, 0, 0, 0 };
            testData[1] = new double[] { 0, 0, 0, 0, 1, 1 };
            testData[2] = new double[] { 0, 1, 0, 0, 0, 0 };
            testData[3] = new double[] { 0, 0, 0, 0, 1, 0 };

            var result = api.Algorithm.Predict(testData, api.Context);

            // NOT FINISHED.
            //Assert.True(result[0] == 1);
            //Assert.True(result[1] == 0);
            //Assert.True(result[2] == 0);
            //Assert.True(result[3] == 0);
            //Assert.True(result[4] == 1);
            //Assert.True(result[5] == 0);
        }


        /// <summary>
        /// Gives full dataset.
        /// </summary>
        [Fact]
        public void FullDataSetRBMTest()
        {
            const int bits = 10; 

            LearningApi api = new LearningApi();

            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                var maxSamples = (int)Math.Pow(2, bits);
                double[][] data = new double[maxSamples][];

                for (int i = 0; i < maxSamples; i++)
                {
                    data[i] = new double[bits];
                    
                    var val = 1;
                    for (int j = 0; j < bits; j++)
                    {
                        if ((val & i) >= 1)
                        {
                            data[i][j] = 1;
                        }
                        val = val << 1;                       
                    }
                }
                                
                ctx.DataDescriptor = getDescriptorForRbm_sample1();
              
                return data;
            });
             
            api.UseRbm(0.01, 1000, bits, 7);

            RbmScore score = api.Run() as RbmScore;

            double[][] testData = new double[4][];

            testData[0] = new double[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 };
            testData[1] = new double[] { 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 };
            testData[2] = new double[] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 };
            testData[3] = new double[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };

            var result = api.Algorithm.Predict(testData, api.Context);

            // NOT FINISHED.
            //Assert.True(result[0] == 1);
            //Assert.True(result[1] == 0);
            //Assert.True(result[2] == 0);
            //Assert.True(result[3] == 0);
            //Assert.True(result[4] == 1);
            //Assert.True(result[5] == 0);
        }


        /// <summary>
        /// This test provides data, which contains two patterns.
        /// First pattern is concentrated on left and second pattern is concentrated on right.
        /// Sample data is stored in 'rbm_twoclass_sample.csv'.
        /// Data looks like:
        /// 011111000000
        /// 000000001110
        /// It is concentrated on left or on right.
        /// </summary>
        [Fact]
        public void Rbm_ClassifierTest()
        {
            var dataPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\rbm_twoclass_sample.csv");

            LearningApi api = new LearningApi(this.getDescriptorForRbmTwoClassesClassifier());

            // Initialize data provider
            api.UseCsvDataProvider(dataPath, ';', false, 1);
            api.UseDefaultDataMapper();
            api.UseRbm(0.01, 1000, 10, 2);

            RbmScore score = api.Run() as RbmScore;

            double[][] testData = new double[5][];

            //
            // This test data contains two patterns. One is grouped at left and one at almost right.
            testData[0] = new double[] { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
            testData[1] = new double[] { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
            testData[2] = new double[] { 0, 0, 0, 0, 0, 1, 1, 1, 0, 0 };
            testData[3] = new double[] { 0, 0, 0, 0, 0, 1, 0, 1, 0, 0 };

            // This will be classified as third class.
            testData[4] = new double[] { 1, 1, 1, 0, 0, 1, 1, 1, 0, 0 };

            var result = api.Algorithm.Predict(testData, api.Context) as RbmResult;

            //
            // 2 * BIT1 + BIT2 of [0] and [1] should be same.
            // We don't know how RBM will classiffy data. We only expect that
            // same or similar pattern of data will be assigned to the same class.
            // Note, we have here two classes (two hiddne nodes).
            // First and second data sample are of same class. Third and fourth are also of same class.

            // Here we check first classs.
            Assert.True(2 * result.HiddenNodesPredictions[0][0] + result.HiddenNodesPredictions[0][1] ==
                2 * result.HiddenNodesPredictions[1][0] + result.HiddenNodesPredictions[1][1]);

            // Here is test for second class.
            Assert.True(2 * result.HiddenNodesPredictions[2][0] + result.HiddenNodesPredictions[2][1] ==
                2 * result.HiddenNodesPredictions[3][0] + result.HiddenNodesPredictions[3][1]);

            printVector("Weights", result.Weights);
        }


        private void printVector(string name, double[][] vector)
        {
            Debug.WriteLine("");
            Debug.WriteLine(name);
            for (int row = 0; row < vector.Length; ++row)
            {
                Debug.WriteLine("");
                for (int col = 0; col < vector[row].Length; ++col)
                    Debug.Write($"{vector[row][col]:F3}\t");
            }

            Debug.WriteLine("");
        }

        
        private void appendImageBinary(string image, string binaryFilePath)
        {
            //saving images in Bitmap format
            var names = image.Split('\\');
            string filename = string.Empty;
            foreach (var item in names)
            {
                // if (item.Contains("."))
                filename = " " + item.Substring(0, 1);
            }
        
            //Loading Bmp images        
            Bitmap Imgbmp = new Bitmap(64, 64);
        
            using (StreamWriter writer = File.AppendText(binaryFilePath))
            {
                Bitmap img = new Bitmap(image);
                StringBuilder t = new StringBuilder();
                int hg = img.Height;
                int wg = img.Width;
                for (int i = 0; i < hg; i++)
                {
                    for (int j = 0; j < wg; j++)
                    {
                        // t = 0 .299R + 0 .587G + 0 .144B
                        t.Append((img.GetPixel(j, i).R > 100 && img.GetPixel(j, i).G > 100 &&
                           img.GetPixel(j, i).B > 100) ? 1 : 0);
                        //  t.Append(img.GetPixel(i,j).R);
                    }
                    t.AppendLine();
                }
                string text = t.ToString();
                writer.Write(text);
                writer.WriteLine(filename);
            }

        }


    }




}
