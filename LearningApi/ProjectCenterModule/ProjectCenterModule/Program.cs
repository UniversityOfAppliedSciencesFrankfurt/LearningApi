using System;
using System.IO;
using CenterModule;

namespace ProjectCenterModule
{
    class Program
    {
        static void Main(string[] args)
        {
            int digitNumber = 59999;
            double average = 0;
            //Three Input Arguments(String,String,int)
            // Relative Paths to Dataset of images and their labels

            string pixelFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "train-images.idx3-ubyte");
            string labelFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "train-labels.idx1-ubyte");


            // Object of the Class LoadDigitImage

            DigitImage[] images = LoadDigitImage.LoadData(pixelFile, labelFile);


            //Output of the first 30 centered MNISTS
            for (digitNumber = 0; digitNumber <= 30; digitNumber++)
            {
                //Make Mnist pictures in Double Array
                double[][] data = new double[images[digitNumber].pixels.Length][];
                for (int i = 0; i < data.Length; i++)
                {

                    data[i] = new double[images[digitNumber].pixels.Length];
                }
                for (int i = 0; i < images[digitNumber].pixels.Length; i++)
                {
                    for (int j = 0; j < images[digitNumber].pixels.Length; j++)
                    {
                        data[i][j] = ((double)images[digitNumber].pixels[i][j]);
                    }
                }

                //Output Original Digit
                Console.WriteLine("Original Digit:");
                for (int i = 0; i < data.Length; i++)
                {
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        Byte.TryParse(data[i][j].ToString(), out byte result);
                        Console.Write("{0} ", result.ToString("X2"));

                    }
                    Console.WriteLine("");
                }

                CenterAlgorithm center = new CenterAlgorithm();

                double[][] centeredData = center.Run(data, null);
                


                //Output Centered Digit
                Console.WriteLine("Centered Digit:");
                for (int i = 0; i < centeredData.Length; i++)
                {
                    for (int j = 0; j < centeredData[i].Length; j++)
                        Console.Write("{0}", centeredData[i][j]);
                    Console.WriteLine("");
                }
                Console.WriteLine("----------------------------------------------------------");



                Console.ReadKey();
            }

        }
    }
}
    

