using AnomalyDetection.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    public static class Helpers
    {
        /// <summary>
        /// Crates sample data distributed arround specified clusters.
        /// </summary>
        /// <param name="clusterCentars"></param>
        /// <param name="numSkalars"></param>
        /// <param name="numDataSamples"></param>
        /// <param name="maxDistanceFromClusterCentar"></param>
        /// <returns></returns>
        public static double[][] CreateSampleData(double[][] clusterCentars, int numSkalars, int numDataSamples, double maxDistanceFromClusterCentar)
        {
            List<double[]> samples = new List<double[]>();

            Random rnd = new Random();

            int numClusters = clusterCentars.Length;

            double[] distances = calcMinClusterDistance(clusterCentars, numSkalars);

            double[] allowedDeltas = new double[distances.Length];
            for (int i = 0; i < allowedDeltas.Length; i++)
            {
                allowedDeltas[i] = distances[i] * maxDistanceFromClusterCentar;
            }

            for (int i = 0; i < numDataSamples; i++)
            {
                for (int cluster = 0; cluster < numClusters; cluster++)
                {
                    var clusterSample = new double[numSkalars];

                    for (int skalar = 0; skalar < numSkalars; skalar++)
                    {
                        double sampleVal = 1.0 * rnd.Next((int)(clusterCentars[cluster][skalar] - allowedDeltas[skalar]),
                            (int)(clusterCentars[cluster][skalar] + allowedDeltas[skalar]));
                        clusterSample[skalar] = sampleVal;
                    }

                    samples.Add(clusterSample);
                }
            }

            return samples.ToArray();
        }


        /// <summary>
        /// Calculates minimal distance between cluster centars for each dimension.
        /// </summary>
        /// <param name="clusterCentars">Array of cluster centars.</param>
        /// <param name="numAttributes"></param>
        /// nearest cluster centars. 0.5% would generate samples directly between two cluster boundaries.</param>
        /// <returns>Minimum distance between centars per dimension.</returns>
        private static double[] calcMinClusterDistance(double[][] clusterCentars, int numAttributes)
        {
            double[] distances = new double[numAttributes];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = double.MaxValue;
            }

            for (int i = 0; i < clusterCentars.Length - 1; i++)
            {
                for (int j = i + 1; j < clusterCentars.Length; j++)
                {
                    for (int k = 0; k < distances.Length; k++)
                    {
                        var d = Math.Abs(clusterCentars[i][k] - clusterCentars[j][k]);
                        if (d < distances[k])
                            distances[k] = d;
                    }
                }
            }

            return distances;
        }

        /// <summary>
        /// This is for converting csv file to double array
        /// </summary>
        /// <param name="FilePath">path of file</param>
        /// <returns>the data from file</returns>
        public static double[][] cSVtoDoubleJaggedArray(string FilePath)
        {
            if (FilePath.EndsWith(".csv"))
            {
                if (System.IO.File.Exists(FilePath))
                {
                    string CsvFile = "";
                    double[][] CsvData;
                    CsvFile = System.IO.File.ReadAllText(FilePath);

                    string RD = "\r\n";
                    if (CsvFile.EndsWith(",\r\n"))
                    {
                        CsvFile = CsvFile.Remove(CsvFile.Length - 3, 3);
                        RD = ",\r\n" ;
                    }
                    else if (CsvFile.EndsWith("\r\n"))
                    {
                        CsvFile = CsvFile.Remove(CsvFile.Length - 2, 2);
                    }

                    string[] RowDelimiter = { RD };
                    string[] CellDelimiter = { "," };

                    int CsvFileRowsNumber, CsvFileCellsNumber;
                    string[] Rows, Cells;

                    Rows = CsvFile.Split(RowDelimiter, StringSplitOptions.None);
                    CsvFileRowsNumber = Rows.Length;

                    CsvFileCellsNumber = Rows[0].Split(CellDelimiter, StringSplitOptions.None).Length;
                    CsvData = new double[CsvFileRowsNumber][];
                    for (int i = 0; i < CsvFileRowsNumber; i++)
                    {
                        CsvData[i] = new double[CsvFileCellsNumber];
                    }

                    for (int i = 0; i < CsvFileRowsNumber; i++)
                    {
                        Cells = Rows[i].Split(CellDelimiter, StringSplitOptions.None);

                        for (int j = 0; j < CsvFileCellsNumber; j++)
                        {
                            try
                            {
                                CsvData[i][j] = Convert.ToDouble(Cells[j]);
                            }
                            catch (FormatException)
                            {
                                return null;
                            }
                            catch (OverflowException)
                            {
                                return null;
                            }

                        }
                    }

                    return CsvData;
                }
            }
            return null;
        }

        public static void WriteToCSVFile(double[][] rawData, string path)
        {
            double[,] data = new double[rawData.Length,rawData.Length];
            using (StreamWriter outfile = new StreamWriter(File.Create($"{Directory.GetCurrentDirectory()}\\{path}")))
            {
                for (int x = 0; x < rawData.Length; x++)
                {
                    string content = "";
                    for (int y = 0; y < rawData[x].Length; y++)
                    {
                        content += rawData[x][y]+"," ;
                    }
                    outfile.WriteLine(content);
                }
            }
        }

        // saves data of type double[][] into CSV file with append option
        public static void Write2CSVFile(double[][] rawData, string path, bool Append=false)
        {
            double[,] data = new double[rawData.Length, rawData.Length];
            if (Append)
            {
                using (StreamWriter outfile = new StreamWriter(File.Open(path, FileMode.Append)))
                {
                    for (int x = 0; x < rawData.Length; x++)
                    {
                        string content = "";
                        for (int y = 0; y < rawData[x].Length; y++)
                        {
                            content += rawData[x][y] + ",";
                        }
                        outfile.WriteLine(content);
                    }
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (StreamWriter outfile = new StreamWriter(File.Open(path, FileMode.Create)))
                {
                    for (int x = 0; x < rawData.Length; x++)
                    {
                        string content = "";
                        for (int y = 0; y < rawData[x].Length; y++)
                        {
                            content += rawData[x][y] + ",";
                        }
                        outfile.WriteLine(content);
                    }
                }
            }
            
        }

        // saves data of type string[][] into CSV file with append option
        public static void Write2CSVFile(string[][] rawData, string path, bool Append = false)
        {
            string[,] data = new string[rawData.Length, rawData.Length];
            if (Append)
            {
                using (StreamWriter outfile = new StreamWriter(File.Open(path, FileMode.Append)))
                {
                    for (int x = 0; x < rawData.Length; x++)
                    {
                        string content = "";
                        for (int y = 0; y < rawData[x].Length; y++)
                        {
                            // seperate data by ',' to save in CSV
                            content += rawData[x][y] + ",";
                        }
                        outfile.WriteLine(content);
                    }
                }
            }
            else
            {
                using (StreamWriter outfile = new StreamWriter(File.Open(path, FileMode.Create)))
                {
                    for (int x = 0; x < rawData.Length; x++)
                    {
                        string content = "";
                        for (int y = 0; y < rawData[x].Length; y++)
                        {
                            // seperate data by ',' to save in CSV
                            content += rawData[x][y] + ",";
                        }
                        outfile.WriteLine(content);
                    }
                }
            }

        }

        public static AnomalyDetectionResponse UpdateMeans(double[][] rawData, int[] clustering, double[][] means, long previousSampleCount = 0, double previousMeanValue = 0)
        {
            try
            {
                if (rawData == null || rawData.Length < 1)
                {
                    return new AnomalyDetectionResponse(102, "Function <UpdateMeans>: RawData is empty");
                }

                if (means == null || means.Length < 1)
                {
                    return new AnomalyDetectionResponse(103, "Function <UpdateMeans>: Means is empty");
                }

                // assumes means[][] exists. consider making means[][] a ref parameter
                int numClusters = means.Length;

                //
                // Zero-out means[][]
                for (int k = 0; k < means.Length; ++k)
                {
                    for (int j = 0; j < means[0].Length; ++j)
                        means[k][j] = 0.0;
                }


                // Make an array to hold cluster counts
                int[] clusterCounts = new int[numClusters];

                //
                // walk through each tuple, accumulate sum for each attribute, update cluster count
                for (int i = 0; i < rawData.Length; ++i)
                {
                    int cluster = clustering[i];

                    // Increment number of samples inside of this cluster.
                    ++clusterCounts[cluster];

                    // Here we build a sum for minibatch.
                    for (int j = 0; j < rawData[i].Length; ++j)
                        means[cluster][j] += rawData[i][j];
                }

                //
                // Divide each attribute sum by cluster count to get average (mean)
                for (int k = 0; k < means.Length; ++k)
                {
                    if (clusterCounts[k] != 0)
                    {
                        for (int j = 0; j < means[k].Length; ++j)
                        {
                            means[k][j] /= clusterCounts[k];
                            double f = (double)1/(rawData.Length + previousSampleCount);
                            means[k][j] = f*(previousSampleCount*previousMeanValue+rawData.Length * means[k][j]);
                        }
                    }
                }

                return new AnomalyDetectionResponse(0, "OK");
            }
            catch (Exception Ex)
            {
                return new AnomalyDetectionResponse(400, "Function <UpdateMeans>: Unhandled exception:\t" + Ex.ToString());
            }
        }
    }
}
