using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProjectCenterModule
{
    class LoadDigitImage
    {

        public string PixelFile { get; set; }
        public string LabelFile { get; set; }
        public LoadDigitImage(String pixelFile, String labelFile)
        {
            PixelFile = pixelFile;
           LabelFile = labelFile;

        }


        // Method Change Big-Endian to Little-Endian format
        public static int ReverseBytes(int v)
        {
            byte[] intAsBytes = BitConverter.GetBytes(v);
            Array.Reverse(intAsBytes);
            return BitConverter.ToInt32(intAsBytes, 0);
        }
        //Show the Image as Matrix (String) With Column = 28 and Row= 28
        public static string PixelValues(DigitImage dImage)
        {
            string s = "";
            for (int i = 0; i < dImage.height; ++i)
            {
                for (int j = 0; j < dImage.width; ++j)
                {
                    s += dImage.pixels[i][j].ToString("X2") + " ";
                }
                s += Environment.NewLine;
            }
            return s;
        }
        /*The Method open the Pixel-(image) and the Label-Data and load it.
         * It begins with implementing a 28x28 Matrix of the Pixel-(Image) Values.
         * For this it uses the binaryreader- class from .NET.
          
         */
        public static DigitImage[] LoadData(string pixelFile, string labelFile)
        {
            int numImages = 60000;
            DigitImage[] result = new DigitImage[numImages];
            byte[][] pixels = new byte[28][];
            for (int i = 0; i < pixels.Length; ++i)
                pixels[i] = new byte[28];
            FileStream ifsPixels = new FileStream(pixelFile, FileMode.Open);
            FileStream ifsLabels = new FileStream(labelFile, FileMode.Open);
            BinaryReader brImages = new BinaryReader(ifsPixels);
            BinaryReader brLabels = new BinaryReader(ifsLabels);
            int magic1 = brImages.ReadInt32(); // stored as big endian
            magic1 = ReverseBytes(magic1); // convert to little endian
            int imageCount = brImages.ReadInt32();
            imageCount = ReverseBytes(imageCount);
            int numRows = brImages.ReadInt32();
            numRows = ReverseBytes(numRows);
            int numCols = brImages.ReadInt32();
            numCols = ReverseBytes(numCols);
            int magic2 = brLabels.ReadInt32();
            magic2 = ReverseBytes(magic2);
            int numLabels = brLabels.ReadInt32();
            numLabels = ReverseBytes(numLabels);

            // Reduce the Data to 28x28 pixel pictures of values
            for (int di = 0; di < numImages; ++di)
            {
                for (int i = 0; i < 28; ++i) // get 28x28 pixel values
                {
                    for (int j = 0; j < 28; ++j)
                    {
                        byte b = brImages.ReadByte();
                        pixels[i][j] = b;
                    }
                }
                byte lbl = brLabels.ReadByte(); // get the label

                //here u can reduce the size of Digit
                DigitImage dImage = new DigitImage(28, 28, pixels, lbl);
                result[di] = dImage;
            } // Each image
            ifsPixels.Close();
            brImages.Close();
            ifsLabels.Close();
            brLabels.Close();
            return result;
        }
        /*public String LoadCenteredData(string pixelFile, String labelFile, int nextIndex)
        {
            String pixelVals;
            //Load Data
            DigitImage[] trainImages = null;

            trainImages = LoadData(pixelFile, labelFile);
            DigitImage currImage = trainImages[nextIndex];
            pixelVals = PixelValues(currImage);
            return pixelVals;

        }*/
    }
}

