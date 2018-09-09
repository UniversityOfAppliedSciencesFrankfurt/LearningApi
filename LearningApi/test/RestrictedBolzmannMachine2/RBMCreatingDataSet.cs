using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NeuralNet.RestrictedBolzmannMachine2;
using ImageBinarizer;
using NeuralNet.Perceptron;
using LearningFoundation.DataProviders;
using LearningFoundation;
using Xunit;

namespace test.RestrictedBolzmannMachine2
{
    public class RBMCreatingDataSet
    {
        static RBMCreatingDataSet()
        {

        }

        //saving initializing Path to store binary data in CSV File
        //public static string DigitDatasetCSVPath = @"D:\thesis\DigitDataset.csv";
        //public static string DigitDatasetCSVPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\DigitDataset.csv");
        public static string DigitDatasetCSVPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\Smiley.csv");
        //public StreamWriter DigitDatasetCSVFile = File.CreateText(DigitDatasetCSVPath);
        public StreamWriter SmileyCSVFile = File.CreateText(DigitDatasetCSVPath);

        //path for images on local machine, should be changed for new data
        //private static string imagePath = @"C:\Program Files\MATLAB\R2017b\toolbox\nnet\nndemos\nndatasets\DigitDataset\";
        private static string imagePath = @"C:\Users\Jyoti\Desktop\RBM\Smiley\";

        private DataDescriptor getImgRecognitionDescriptor(int numOfFeatures)
        {
            DataDescriptor desc = new DataDescriptor();
            desc.Features = new LearningFoundation.DataMappers.Column[numOfFeatures];
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 0,
                Name = "X",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.LabelIndex = 1;

            return desc;
        }

        // Read image and convert to binary format
        private string readImageData(string imageName, int width, int height)
        {
            //string trainingImagesPath = Path.Combine(Path.Combine(AppContext.BaseDirectory, "RestrictedBolzmannMachine"), "TrainingImages");
            Binarizer bizer = new Binarizer(targetHeight: height, targetWidth: width);
            //return bizer.GetBinary(Path.Combine(trainingImagesPath, imageName));
            return bizer.GetBinary(imageName);
        }

        /// <summary>
        /// Test case for creating and storing binary data in csv file. 
        /// </summary>
        [Fact]
        public void RBMBinaryDataCreation()
        {
            //size of image
            int size = 40;

            var context = getImgRecognitionDescriptor(size * size);

            LearningApi api = new LearningApi(context);

            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                double[][] trainData = new double[22][];
                for (int j = 0; j < 1; j++)
                {
                    //Path of training images.
                    //return getImageData(size, $"{Directory.GetCurrentDirectory()}\\RestrictedBolzmannMachine2\\TrainingImages");
                    trainData = getImageData(size, imagePath +j);
                }
                return trainData;
            });

            IScore score = api.Run() as IScore;

            //DigitDatasetCSVFile.Close();
            SmileyCSVFile.Close();
        }

        private double[][] getImageData(int size, string imageFolder)
        {
            var trainingImages = Directory.GetFiles(imageFolder);

            double[][] data = new double[trainingImages.Length][];

            int indx = 0;

            foreach (var file in trainingImages)
            {
                string imgData = readImageData(file, size, size);

                List<double> imgTrainingRow = new List<double>();

                foreach (var rowInImg in imgData.Split('\n'))
                {
                    for (int k = 0; k < rowInImg.Length; k++)
                    {
                        if (rowInImg[k] == '1')
                            imgTrainingRow.Add(1);
                        else if (rowInImg[k] == '0')
                            imgTrainingRow.Add(0);
                        else if (rowInImg[k] == '\r' || rowInImg[k] == '\n')
                            continue;
                        else
                            throw new InvalidDataException($"'1' or '0' is supported only. We detected {rowInImg[k]}");
                    }
                }

                //WriteImageDataToCSVFile(DigitDatasetCSVFile, imgTrainingRow);
                WriteImageDataToCSVFile(SmileyCSVFile, imgTrainingRow);

                data[indx] = imgTrainingRow.ToArray();
                indx++;
            }



            return data;
        }


        //private void WriteImageDataToCSVFile(StreamWriter DigitDatasetCSVFile, List<double> imageData)
        private void WriteImageDataToCSVFile(StreamWriter SmileyCSVFile, List<double> imageData)
        {
            //Writing data in a csv file
            for (int i = 0; i < imageData.Count; i++)
            {
                //DigitDatasetCSVFile.Write(imageData[i]);
                //DigitDatasetCSVFile.Write(',');
                SmileyCSVFile.Write(imageData[i]);
                SmileyCSVFile.Write(',');
            }
            //DigitDatasetCSVFile.WriteLine();
            //DigitDatasetCSVFile.AutoFlush = true;
            SmileyCSVFile.WriteLine();
            SmileyCSVFile.AutoFlush = true;
        }

    }
}
