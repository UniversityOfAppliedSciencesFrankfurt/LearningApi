
using LearningFoundation;

namespace CenterModule
{
    public class CenterAlgorithm : IPipelineModule<double[][], double[][]>
    {
        private double average = 0;
        
       
        /// <summary>
        /// Method GetAverage build the Average over data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double GetAverage(double[][] data)
        {
            //building the average about all values
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    average += data[i][j];

                }
            }
            average /= data.GetLength(0) * data.Length;
            return average;
        }

        /// <summary>
        /// Method Binarization replace everything over the average with a one.
        /// The rest will be zero.
        /// 
        /// </summary>
        /// <param name="data"></param>
        public double[][] Binarization(double[][] data)
        {
            double average = GetAverage(data);

            //divide each value by teh average
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    if (data[i][j] > average)
                    {
                        data[i][j] = 1;
                    }
                    else { data[i][j] = 0; }
                }
            }
            return data;

        }
        
        /// <summary>
        /// method for Summation of all X-VAlues
        /// </summary>
        /// <param name="x"></param>
        /// <param name="countOnes"></param>
        /// <returns></returns>
        public float GetSumXValues(float[] x, int countOnes)
        {
            float xEnd = 0;
            for (int i = 0; i < countOnes; i++)
            {
                xEnd += x[i];
            }
            return xEnd;
        }
        /// <summary>
        /// Method for Initialisation of the Array for the Centered Image
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public double[][] InitiateCenteredArray(int width, int height)
        {
            double[][] centeredData = new double[height][];
            for (int i = 0; i < height; i++)
            {
                centeredData[i] = new double[width];
            }

            return centeredData;
        }

        /// <summary>
        /// method for Summation of all Y-VAlues
        /// </summary>
        
        public float GetSumYValues(float[] y, int countOnes)
        {
            float yEnd = 0;
            for (int i = 0; i < countOnes; i++)
            {
                yEnd += y[i];
            }
            return yEnd;
        }

        public double[][] Run(double[][] data, IContext ctx)
        {

            Binarization(data);
            // Get width and height of double [][]
            int width = data[0].Length;
            int height = data.Length;
            // Count amount of ones in Array
            int countOnes = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (data[i][j] == 1)
                    {
                        countOnes++;
                    }
                }
            }
            //Initiate Array for the x/y-coordinates of the ones in the Array
            float[] x = new float[countOnes];
            float[] y = new float[countOnes];
            // save position of ones with x/y- coordinates in Arrays
            int countPos = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (data[i][j] == 1)
                    {
                        y[countPos] = i;
                        x[countPos] = j;
                        countPos++;
                    }
                }
            }
            // Summation of all x and y values
            float xEnd = GetSumXValues(x, countOnes);
            float yEnd = GetSumYValues(y, countOnes);

            // Here I calculate the center of the origin (Center  by building the mean)

            int xReallyEnd = (int)(xEnd / countOnes);
            int yReallyEnd = (int)(yEnd / countOnes);

            // Get Half of the height and width
            int heightHalf = height / 2;
            int widthHalf = width / 2;
            // Calcute the shift of the ones in x and y direction
            float diffMiddleNumberToMiddleMatrixX = xReallyEnd - widthHalf;
            float diffMiddleNumberToMiddleMatrixY = yReallyEnd - heightHalf;

            // Initiate new Array in the same size as the Origin Array
            double[][] centeredData = InitiateCenteredArray(width, height);
            // put the ones in Array and shift them by the before Calculated Value 
            for (int i = 0; i < x.Length; i++)
            {
                x[i] += diffMiddleNumberToMiddleMatrixX;
                y[i] += diffMiddleNumberToMiddleMatrixY;
                centeredData[(int)y[i]][(int)x[i]] = 1;
            }
            //Fill the Rest of the Array with Zeros
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (centeredData[i][j] != 1
                    )
                        centeredData[i][j] = 0;
                }
            }

            return centeredData;
        }




    }


}


