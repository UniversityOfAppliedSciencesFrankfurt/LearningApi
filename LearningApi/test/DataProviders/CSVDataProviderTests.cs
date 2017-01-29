using test;
using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using LearningFoundation;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;
using LearningFoundation.Normalizers;
using LearningFoundation.Statistics;
using NeuralNet.BackPropagation;

namespace test.csvdataprovider
{

    public class CSVDataProviderTast
    {
        string m_iris_data_path = @"SampleData\iris\iris.csv";
        string m_secom_data_path = @"SampleData\secom\SECOM_Dataset_AllFeatures.csv";
        string m_secom_data_mapper_path = @"SampleData\secom\SECOM_data_mapper.json";

        public CSVDataProviderTast()
        {


        }

        /// <summary>
        /// Demonstrates how to inject a data provider as an action.
        /// </summary>
        [Fact]
        public void CSVDataProviderTest_IrisData()
        {
            //
            //iris data file
            var isris_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), m_iris_data_path);

            LearningApi api = new LearningApi(TestHelpers.GetDescriptor());
            api.UseCsvDataProvider(isris_path, ',', 1);

            var result = api.Run() as object[][];

            //get expected result
            var expected = GetReal_Iris_DataSet();

            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < result[0].Length; j++)
                {
                    var col = api.Context.DataDescriptor.Features[j];
                    if (col.Type == ColumnType.STRING)
                        continue;
                    else if (col.Type == ColumnType.NUMERIC)//numeric column
                    {
                        var val1 = double.Parse(result[i][j].ToString());
                        var val2 = double.Parse(expected[i][j].ToString());

                        Assert.Equal(val1,val2);
                    }
                    else if (col.Type == ColumnType.BINARY)//binary column
                    {
                        Assert.Equal(result[i][j].ToString(), expected[i][j].ToString());
                    }
                    else if (col.Type == ColumnType.CLASS)//class column
                    {
                        Assert.Equal(result[i][j].ToString(), expected[i][j].ToString());
                    }

                   
                }
            }

            return;

        }


        /// <summary>
        /// Demonstrates how to inject a data provider as an action.
        /// </summary>
        [Fact]
        public void CSVDataProviderTest_SecomData()
        {
            //
            //iris data file
            var isris_path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), m_secom_data_path);

            LearningApi api = new LearningApi(TestHelpers.GetDescriptor(m_secom_data_mapper_path));
            api.UseCsvDataProvider(isris_path, ',', 1);

            var result = api.Run() as object[][];

            //get expected result
            var expected = GetReal_Secom_DataSet();

            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < result[0].Length; j++)
                {
                    var col = api.Context.DataDescriptor.Features[j];
                    if (col.Type == ColumnType.STRING)
                        Assert.Equal(result[i][j], expected[i][j]);
                    else if (col.Type == ColumnType.NUMERIC)//numeric column
                    {
                        //var val1 = double.Parse(result[i][j].ToString());
                        //var val2 = double.Parse(expected[i][j].ToString());

                        Assert.Equal(result[i][j], expected[i][j]);
                    }
                    else if (col.Type == ColumnType.BINARY)//binary column
                    {
                        Assert.Equal(result[i][j].ToString(), expected[i][j].ToString());
                    }
                    else if (col.Type == ColumnType.CLASS)//class column
                    {
                        Assert.Equal(result[i][j].ToString(), expected[i][j].ToString());
                    }


                }
            }

            return;

        }

        private object[][] GetReal_Secom_DataSet()
        {
            var secomPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(),m_secom_data_path);
            var lines = File.ReadAllLines(secomPath);

            var o = new List<object[]>();
            for (int ll=1; ll< lines.Length; ll++)
            {
                var c = lines[ll];
                var l = new object[592];
                var cols = c.Split(',');

                for(int i=0; i<592; i++)
                {
                    l[i] = cols[i];
                }
                o.Add(l);
            }

            return o.ToArray<object[]>();
        }


        private object[][] GetReal_Iris_DataSet()
        {
            var data = new object[150][] {
            new object[]{5.1,3.5,1.4,0.2,"setosa"},
            new object[]{4.9,3,1.4,0.2,"setosa"},
            new object[]{4.7,3.2,1.3,0.2,"setosa"},
            new object[]{4.6,3.1,1.5,0.2,"setosa"},
            new object[]{5,3.6,1.4,0.2,"setosa"},
            new object[]{5.4,3.9,1.7,0.4,"setosa"},
            new object[]{4.6,3.4,1.4,0.3,"setosa"},
            new object[]{5,3.4,1.5,0.2,"setosa"},
            new object[]{4.4,2.9,1.4,0.2,"setosa"},
            new object[]{4.9,3.1,1.5,0.1,"setosa"},
            new object[]{5.4,3.7,1.5,0.2,"setosa"},
            new object[]{4.8,3.4,1.6,0.2,"setosa"},
            new object[]{4.8,3,1.4,0.1,"setosa"},
            new object[]{4.3,3,1.1,0.1,"setosa"},
            new object[]{5.8,4,1.2,0.2,"setosa"},
            new object[]{5.7,4.4,1.5,0.4,"setosa"},
            new object[]{5.4,3.9,1.3,0.4,"setosa"},
            new object[]{5.1,3.5,1.4,0.3,"setosa"},
            new object[]{5.7,3.8,1.7,0.3,"setosa"},
            new object[]{5.1,3.8,1.5,0.3,"setosa"},
            new object[]{5.4,3.4,1.7,0.2,"setosa"},
            new object[]{5.1,3.7,1.5,0.4,"setosa"},
            new object[]{4.6,3.6,1,0.2,"setosa"},
            new object[]{5.1,3.3,1.7,0.5,"setosa"},
            new object[]{4.8,3.4,1.9,0.2,"setosa"},
            new object[]{5,3,1.6,0.2,"setosa"},
            new object[]{5,3.4,1.6,0.4,"setosa"},
            new object[]{5.2,3.5,1.5,0.2,"setosa"},
            new object[]{5.2,3.4,1.4,0.2,"setosa"},
            new object[]{4.7,3.2,1.6,0.2,"setosa"},
            new object[]{4.8,3.1,1.6,0.2,"setosa"},
            new object[]{5.4,3.4,1.5,0.4,"setosa"},
            new object[]{5.2,4.1,1.5,0.1,"setosa"},
            new object[]{5.5,4.2,1.4,0.2,"setosa"},
            new object[]{4.9,3.1,1.5,0.2,"setosa"},
            new object[]{5,3.2,1.2,0.2,"setosa"},
            new object[]{5.5,3.5,1.3,0.2,"setosa"},
            new object[]{4.9,3.6,1.4,0.1,"setosa"},
            new object[]{4.4,3,1.3,0.2,"setosa"},
            new object[]{5.1,3.4,1.5,0.2,"setosa"},
            new object[]{5,3.5,1.3,0.3,"setosa"},
            new object[]{4.5,2.3,1.3,0.3,"setosa"},
            new object[]{4.4,3.2,1.3,0.2,"setosa"},
            new object[]{5,3.5,1.6,0.6,"setosa"},
            new object[]{5.1,3.8,1.9,0.4,"setosa"},
            new object[]{4.8,3,1.4,0.3,"setosa"},
            new object[]{5.1,3.8,1.6,0.2,"setosa"},
            new object[]{4.6,3.2,1.4,0.2,"setosa"},
            new object[]{5.3,3.7,1.5,0.2,"setosa"},
            new object[]{5,3.3,1.4,0.2,"setosa"},
            new object[]{7,3.2,4.7,1.4,"versicolor"},
            new object[]{6.4,3.2,4.5,1.5,"versicolor"},
            new object[]{6.9,3.1,4.9,1.5,"versicolor"},
            new object[]{5.5,2.3,4,1.3,"versicolor"},
            new object[]{6.5,2.8,4.6,1.5,"versicolor"},
            new object[]{5.7,2.8,4.5,1.3,"versicolor"},
            new object[]{6.3,3.3,4.7,1.6,"versicolor"},
            new object[]{4.9,2.4,3.3,1,"versicolor"},
            new object[]{6.6,2.9,4.6,1.3,"versicolor"},
            new object[]{5.2,2.7,3.9,1.4,"versicolor"},
            new object[]{5,2,3.5,1,"versicolor"},
            new object[]{5.9,3,4.2,1.5,"versicolor"},
            new object[]{6,2.2,4,1,"versicolor"},
            new object[]{6.1,2.9,4.7,1.4,"versicolor"},
            new object[]{5.6,2.9,3.6,1.3,"versicolor"},
            new object[]{6.7,3.1,4.4,1.4,"versicolor"},
            new object[]{5.6,3,4.5,1.5,"versicolor"},
            new object[]{5.8,2.7,4.1,1,"versicolor"},
            new object[]{6.2,2.2,4.5,1.5,"versicolor"},
            new object[]{5.6,2.5,3.9,1.1,"versicolor"},
            new object[]{5.9,3.2,4.8,1.8,"versicolor"},
            new object[]{6.1,2.8,4,1.3,"versicolor"},
            new object[]{6.3,2.5,4.9,1.5,"versicolor"},
            new object[]{6.1,2.8,4.7,1.2,"versicolor"},
            new object[]{6.4,2.9,4.3,1.3,"versicolor"},
            new object[]{6.6,3,4.4,1.4,"versicolor"},
            new object[]{6.8,2.8,4.8,1.4,"versicolor"},
            new object[]{6.7,3,5,1.7,"versicolor"},
            new object[]{6,2.9,4.5,1.5,"versicolor"},
            new object[]{5.7,2.6,3.5,1,"versicolor"},
            new object[]{5.5,2.4,3.8,1.1,"versicolor"},
            new object[]{5.5,2.4,3.7,1,"versicolor"},
            new object[]{5.8,2.7,3.9,1.2,"versicolor"},
            new object[]{6,2.7,5.1,1.6,"versicolor"},
            new object[]{5.4,3,4.5,1.5,"versicolor"},
            new object[]{6,3.4,4.5,1.6,"versicolor"},
            new object[]{6.7,3.1,4.7,1.5,"versicolor"},
            new object[]{6.3,2.3,4.4,1.3,"versicolor"},
            new object[]{5.6,3,4.1,1.3,"versicolor"},
            new object[]{5.5,2.5,4,1.3,"versicolor"},
            new object[]{5.5,2.6,4.4,1.2,"versicolor"},
            new object[]{6.1,3,4.6,1.4,"versicolor"},
            new object[]{5.8,2.6,4,1.2,"versicolor"},
            new object[]{5,2.3,3.3,1,"versicolor"},
            new object[]{5.6,2.7,4.2,1.3,"versicolor"},
            new object[]{5.7,3,4.2,1.2,"versicolor"},
            new object[]{5.7,2.9,4.2,1.3,"versicolor"},
            new object[]{6.2,2.9,4.3,1.3,"versicolor"},
            new object[]{5.1,2.5,3,1.1,"versicolor"},
            new object[]{5.7,2.8,4.1,1.3,"versicolor"},
            new object[]{6.3,3.3,6,2.5,"virginica"},
            new object[]{5.8,2.7,5.1,1.9,"virginica"},
            new object[]{7.1,3,5.9,2.1,"virginica"},
            new object[]{6.3,2.9,5.6,1.8,"virginica"},
            new object[]{6.5,3,5.8,2.2,"virginica"},
            new object[]{7.6,3,6.6,2.1,"virginica"},
            new object[]{4.9,2.5,4.5,1.7,"virginica"},
            new object[]{7.3,2.9,6.3,1.8,"virginica"},
            new object[]{6.7,2.5,5.8,1.8,"virginica"},
            new object[]{7.2,3.6,6.1,2.5,"virginica"},
            new object[]{6.5,3.2,5.1,2,"virginica"},
            new object[]{6.4,2.7,5.3,1.9,"virginica"},
            new object[]{6.8,3,5.5,2.1,"virginica"},
            new object[]{5.7,2.5,5,2,"virginica"},
            new object[]{5.8,2.8,5.1,2.4,"virginica"},
            new object[]{6.4,3.2,5.3,2.3,"virginica"},
            new object[]{6.5,3,5.5,1.8,"virginica"},
            new object[]{7.7,3.8,6.7,2.2,"virginica"},
            new object[]{7.7,2.6,6.9,2.3,"virginica"},
            new object[]{6,2.2,5,1.5,"virginica"},
            new object[]{6.9,3.2,5.7,2.3,"virginica"},
            new object[]{5.6,2.8,4.9,2,"virginica"},
            new object[]{7.7,2.8,6.7,2,"virginica"},
            new object[]{6.3,2.7,4.9,1.8,"virginica"},
            new object[]{6.7,3.3,5.7,2.1,"virginica"},
            new object[]{7.2,3.2,6,1.8,"virginica"},
            new object[]{6.2,2.8,4.8,1.8,"virginica"},
            new object[]{6.1,3,4.9,1.8,"virginica"},
            new object[]{6.4,2.8,5.6,2.1,"virginica"},
            new object[]{7.2,3,5.8,1.6,"virginica"},
            new object[]{7.4,2.8,6.1,1.9,"virginica"},
            new object[]{7.9,3.8,6.4,2,"virginica"},
            new object[]{6.4,2.8,5.6,2.2,"virginica"},
            new object[]{6.3,2.8,5.1,1.5,"virginica"},
            new object[]{6.1,2.6,5.6,1.4,"virginica"},
            new object[]{7.7,3,6.1,2.3,"virginica"},
            new object[]{6.3,3.4,5.6,2.4,"virginica"},
            new object[]{6.4,3.1,5.5,1.8,"virginica"},
            new object[]{6,3,4.8,1.8,"virginica"},
            new object[]{6.9,3.1,5.4,2.1,"virginica"},
            new object[]{6.7,3.1,5.6,2.4,"virginica"},
            new object[]{6.9,3.1,5.1,2.3,"virginica"},
            new object[]{5.8,2.7,5.1,1.9,"virginica"},
            new object[]{6.8,3.2,5.9,2.3,"virginica"},
            new object[]{6.7,3.3,5.7,2.5,"virginica"},
            new object[]{6.7,3,5.2,2.3,"virginica"},
            new object[]{6.3,2.5,5,1.9,"virginica"},
            new object[]{6.5,3,5.2,2,"virginica"},
            new object[]{6.2,3.4,5.4,2.3,"virginica"},
            new object[]{5.9,3,5.1,1.8,"virginica"}
        };

            return data;
        }
    }
}
