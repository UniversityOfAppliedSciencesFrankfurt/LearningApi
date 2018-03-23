using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    public static class Helpers
    {
        private const string cCR = "\r\n";

        private const string cSeparator = ";";

        /// <summary>
        /// CreateSampleData creates sample data distributed arround specified clusters.
        /// </summary>
        /// <param name="clusterCenters">center of clusters</param>
        /// <param name="numSkalars">number of attributes</param>
        /// <param name="numDataSamples">number of samples</param>
        /// <param name="maxDistanceFromClusterCenter">maximum allowed distance from the cluster center</param>
        /// <returns></returns>
        public static double[][] CreateSampleData(double[][] clusterCenters, int numSkalars, int numDataSamples, double maxDistanceFromClusterCenter)
        {
            List<double[]> samples = new List<double[]>();

            Random rnd = new Random();

            int numClusters = clusterCenters.Length;

            double[] distances = calcMinClusterDistance(clusterCenters, numSkalars);

            double[] allowedDeltas = new double[distances.Length];
            for (int i = 0; i < allowedDeltas.Length; i++)
            {
                allowedDeltas[i] = distances[i] * maxDistanceFromClusterCenter;
            }

            for (int i = 0; i < numDataSamples; i++)
            {
                for (int cluster = 0; cluster < numClusters; cluster++)
                {
                    var clusterSample = new double[numSkalars];

                    for (int skalar = 0; skalar < numSkalars; skalar++)
                    {
                        double sampleVal = 1.0 * rnd.Next((int)(clusterCenters[cluster][skalar] - allowedDeltas[skalar]),
                            (int)(clusterCenters[cluster][skalar] + allowedDeltas[skalar]));
                        clusterSample[skalar] = sampleVal;
                    }

                    samples.Add(clusterSample);
                }
            }

            return samples.ToArray();
        }


        /// <summary>
        /// calcMinClusterDistance calculates minimal distance between cluster centars for each dimension.
        /// </summary>
        /// <param name="clusterCentars">Array of cluster centars.</param>
        /// <param name="numAttributes">number of attributes</param>
        /// <returns>Minimum distance between centers per dimension.</returns>
        private static double[] calcMinClusterDistance(double[][] clusterCenters, int numAttributes)
        {
            double[] distances = new double[numAttributes];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = double.MaxValue;
            }

            for (int i = 0; i < clusterCenters.Length - 1; i++)
            {
                for (int j = i + 1; j < clusterCenters.Length; j++)
                {
                    for (int k = 0; k < distances.Length; k++)
                    {
                        var d = Math.Abs(clusterCenters[i][k] - clusterCenters[j][k]);
                        if (d < distances[k])
                            distances[k] = d;
                    }
                }
            }

            return distances;
        }

        public static bool CheckOrCreateDefaultFunction(string rootPath, string file, int points, int numOfDims)
        {
            if (File.Exists(file))
                return true;

            if (File.Exists(Path.Combine(rootPath, file)))
                return true;

            var delta = 2 * Math.PI / points;

            List<double[]> rows = new List<double[]>();

            double[] rowX = new double[points];
            for (int i = 0; i < points; i++)
            {
                rowX[i] = i * delta;
            }

            rows.Add(rowX);

            for (int y = 0; y < numOfDims-1; y++)
            {
                double[] rowY = new double[points];

                for (int i = 0; i < points; i++)
                {
                    rowY[i] = Math.Sin(i * delta);
                }

                rows.Add(rowY);
            }

            Write2CSVFile(rows.ToArray(), Path.Combine(rootPath, file), false);

            return false;
        }

        /// <summary>
        /// Loads a csv file into double array
        /// </summary>
        /// <param name="filePath">path of file</param>
        /// <returns>the data from file</returns>
        public static double[][] LoadFunctionData(string filePath)
        {
            if (filePath.EndsWith(".csv"))
            {
                if (System.IO.File.Exists(filePath))
                {
                    string CsvFile = "";
                    double[][] CsvData;
                    CsvFile = System.IO.File.ReadAllText(filePath);

                    string RD = cCR;
                    if (CsvFile.EndsWith($"{cSeparator}{cCR}"))
                    {
                        CsvFile = CsvFile.Remove(CsvFile.Length - 3, 3);
                        RD = $"{cSeparator}{cCR}";
                    }
                    else if (CsvFile.EndsWith(cCR))
                    {
                        CsvFile = CsvFile.Remove(CsvFile.Length - 2, 2);
                    }

                    string[] RowDelimiter = { RD };
                    string[] CellDelimiter = { cSeparator };

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
                                CsvData[i][j] = Convert.ToDouble(Cells[j], CultureInfo.InvariantCulture);
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
        
        /// <summary>
        /// Write2CSVFile saves data of type double[][] into CSV file with append option
        /// </summary>
        /// <param name="rawData">the data to be saved</param>
        /// <param name="path">path to file</param>
        /// <param name="Append">bool, if false the saving will overwrite the existing file else i will append to file</param>
        public static void Write2CSVFile(double[][] rawData, string path, bool Append=false)
        {
            if (Append)
            {
                using (StreamWriter outfile = new StreamWriter(File.Open(path, FileMode.Append)))
                {
                    FormatData(outfile, rawData);
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (StreamWriter outfile = new StreamWriter(File.Open(path, FileMode.Create)))
                {
                    FormatData(outfile, rawData);
                }
            }
            
        }

        /// <summary>
        /// FormatData is a function that formats the data which will be saved in CSV file and uses "Write2CSV"'s StreamWriter to save the data.
        /// </summary>
        /// <param name="outfile">streamWriter initialized in "Write2CSVFile"</param>
        /// <param name="rawData">the data to be saved</param>
        private static void FormatData(StreamWriter outfile, double[][] rawData)
        {
            string content;
            for (int x = 0; x < rawData.Length; x++)
            {
                content = "";
                for (int y = 0; y < rawData[x].Length; y++)
                {
                    content += (rawData[x][y]).ToString(CultureInfo.InvariantCulture) + cSeparator;
                }
                content = content.Remove(content.Length - cSeparator.Length);
                outfile.WriteLine(content);
            }
        }
    }
}
