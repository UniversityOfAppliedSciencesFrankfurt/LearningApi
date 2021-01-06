using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LearningFoundation;

namespace MyDrawingModule
{
    public class DrawingModule : IPipelineModule
    {
        // The name of axis
        public string Name { get; set; }

        // Indices of column to be outputed in diagram
        public int ColumnIndex { get; set; }

        // RGB (or some other format) Color of the value.
        public int Color { get; set; }



            private double[][] CreateDataSample()
            {
                string pathprovided = Path.GetDirectoryName(System.IO.Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
                List<double[]> customData = new List<double[]>();
                StreamReader reader = File.OpenText(pathprovided + @"\HTML folder\drawing.html");
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    double[] line = reader.ReadLine().Split(',').Select(Double.Parse).ToArray();
                    customData.Add(line);
                }
                reader.Dispose();
                var R = customData.Count;
                var C = customData[0].Length;
                double[,] res = new double[R, C];
                for (int r = 0; r != R; r++)
                {
                    for (int c = 0; c != C; c++)
                    {
                        res[r, c] = customData[r][c];
                    }
                }
                double[][] a = customData.ToArray();
                return a;
            }

            public double[][] LoadDataSample()
            {

                double[][] dt = CreateDataSample();
                return dt;
            }
        }
    }

