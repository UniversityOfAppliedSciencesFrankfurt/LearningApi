using Microsoft.VisualStudio.TestTools.UnitTesting;
using SelfOrganizingMap;
using System.Collections.Generic;
using System.IO;


namespace LearningFoundation.Test
{
    [TestClass]
    public class SelfOrganizingMapTest
    {
        string path = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));

        [TestMethod]
        public void TestMapWithLearningAPI()
        {
            List<string> labels = new List<string>();

            var api = new LearningApi();
            api.UseActionModule<List<double[]>, List<double[]>>((data, context) =>
            {
                List<double[]> patterns = new List<double[]>();
                var dimensions = 3;
                StreamReader reader = File.OpenText(path + "\\SelfOrganizingMap\\Food.csv");
                ///<Summary>Ignore first line.
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(',');
                    labels.Add(line[0]);
                    double[] inputs = new double[dimensions];

                    for (int i = 0; i < dimensions; i++)
                    {
                        inputs[i] = double.Parse(line[i + 1]);
                    }
                    patterns.Add(inputs);
                }
                reader.Dispose();

                return patterns;
            });
            api.AddModule(new Map(3, 10, 0.000001));
            var r = api.Run() as Neuron[];

            for (int i = 0; i < r.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine("{0},{1},{2}", labels[i], r[i].m_X, r[i].m_Y);
            }
        }
    }
}
