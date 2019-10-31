using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NeuralNet.RestrictedBolzmannMachine2;
using NeuralNet.Perceptron;
using LearningFoundation.DataProviders;
using LearningFoundation;
using LearningFoundation.DataMappers;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LearningFoundation.Test.RestrictedBolzmannMachine2
{
    [TestClass]
    public class SoftmaxNormalizer
    {
        static SoftmaxNormalizer()
        {

        }

        //saving initializing Path to read and write data in CSV File

        //Path to read the file  
        string PathToSourcefile = @"D:\LearningApi\LearningApi\test\RestrictedBolzmannMachine2\Data\rbm_sample1.csv"; // Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, @"RestrictedBolzmannMachine2\Data\rbm_Sample1.csv");
        //Path to write the file.
        string PathToDestinationFile = @"D:\LearningApi\LearningApi\test\RestrictedBolzmannMachine2\Data\rbm_sample1_result.csv";//Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\rbm_Sample1_result.csv");


        [TestMethod]
        public void softMax()
        {
            List<string> termsList = new List<string>();
            int lengthOfValues = 0;
            using (var reader = new StreamReader(PathToSourcefile))
            {
                //skip the first row
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    lengthOfValues = values.Length;
                    foreach (var value in values)
                    {
                        termsList.Add(value);
                    }
                }
            }

            double[] z = Array.ConvertAll(termsList.ToArray(), new Converter<string, double>(Double.Parse));
            //calculating exponential
            var z_exp = z.Select(Math.Exp);
            // sum of unique values
            var sum_z_exp = z_exp.Distinct().Sum();
            // softmax formula implication
            var softmax = z_exp.Select(i => i / sum_z_exp).ToArray();
            // writing data
            WriteData(softmax, PathToDestinationFile, lengthOfValues);

        }

        // funtion to write data
        private void WriteData(double[] softmax, string pathToDestinationFile, int lengthOfValues)
        {
            int countColumn = 0;
            using (var file = new StreamWriter(pathToDestinationFile))
            {
                for(int i= 0; i < softmax.Length; i++)
                {
                    countColumn++;
                    file.Write(softmax[i] + ",");
                    if (countColumn == lengthOfValues)
                    {
                        file.WriteLine();
                        countColumn = 0; //reset column values back to '0' in order to write the values in the next line.
                    }
                }
            }
        }

        // Read data
        internal static double[][] ReadData(string path)
        {
            List<double[]> data = new List<double[]>();

            var reader = new StreamReader(File.OpenRead(path));

            StreamReader sr = new StreamReader(path);
            String line;

            while ((line = sr.ReadLine()) != null)
            {
                List<double> row = new List<double>();
                var tokens = line.Split(',');
                foreach (var item in tokens)
                {
                    if (item != "")
                        row.Add(double.Parse(item, CultureInfo.InvariantCulture));
                }

                data.Add(row.ToArray());
            }

            return data.ToArray();
        }
    }
}

