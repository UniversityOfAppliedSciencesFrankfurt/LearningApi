

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Xunit;
using LearningFoundation;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;
using LearningFoundation.Statistics;
using LearningFoundation.Normalizers;
using System.Globalization;
using LearningFoundation.Statistics;
namespace test.datamapper
{
    /// <summary>
    /// Test class for testing data mapper - transformation of the RealData in to numeric ML ready data.
    /// </summary>
    public class CorrelationMatrixGenerator
    {
        
        /// <summary>
        /// COnstructor which initializes all necessary data for test
        /// </summary>
        public CorrelationMatrixGenerator()
        {
            
        }

        [Fact]
        public void calculateCorrelation_test1()
        {

            //
            LearningApi api = new LearningApi(null);

            // Initialize data provider
            api.UseCsvDataProvider(@"CorrelationMatrix/corellation_data.csv", ',',true,0);


            //Custom action of dataset 
            api.UseActionModule<object[][], double[][]>((input,ctx) =>
            {
                return toColumnVector(input);
            });


            // api.UseMinMaxNormalizer();


            var data = api.Run() as double[][];

            var prov = api.GetModule<CsvDataProvider>("CsvDataProvider");

            var strData = new List<string>();
            var hed = prov.Header.ToList();
            hed.Insert(0, "");
            strData.Add(string.Join(",", hed.ToArray()));
            for (int i=0; i< data.Length; i++)
            {
                var lst = new List<string>();
                lst.Add(prov.Header[i]);
                for(int  k=0; k < i;k++)
                    lst.Add(" ");

                for (int j = i; j < data.Length; j++)
                {
                    var corValue = data[i].CorrCoeffOf(data[j]);
                    if (double.IsNaN(corValue))
                        continue;
                    lst.Add(corValue.ToString("n5", CultureInfo.InvariantCulture));
                }


                strData.Add(string.Join(",", lst));
            }

            Assert.True("Col1,1.00000,0.16892,0.99111,0.75077,-0.82354,-0.85164"==strData[1]);

            System.IO.File.WriteAllLines(@"CorrelationMatrix/strCorrlation.txt", strData);
            //
            return;
        }

        private double[][] toColumnVector(object[][] input)
        {
            var csvData = new double[input.Length][];
            for (int i=0; i< input.Length; i++)
            {
                csvData[i] = new double[input[i].Length];

                for (int j = 0; j < input[i].Length; j++)
                {
                    if (float.TryParse(input[i][j].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out float val))
                    {
                        csvData[i][j]= val;
                    }
                    else if (DateTime.TryParse(input[i][j].ToString(), out DateTime dat))
                    {
                        csvData[i][j] = dat.Ticks;
                    }
                }
            }

            var colVecData = new List<double[]>();
            for (int j = 0; j < csvData[0].Length; j++)
            {
                var col = new double[csvData.Length];

                for (int i = 0; i < csvData.Length; i++)
                {
                    col[i] = csvData[i][j];
                    
                }

                colVecData.Add(col);
            }
            return colVecData.ToArray();
               
        }

        private DataDescriptor loadMetaData()
        {
            var des = new DataDescriptor();

            des.Features = new Column[5];
            des.Features[0] = new Column { Id = 1, Name = "percent", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0.5, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "color", Index = 1, Type = ColumnType.CLASS, DefaultMissingValue = 0, Values = new string[3] { "red", "green", "blue" } };
            des.Features[2] = new Column { Id = 3, Name = "gender", Index = 2, Type = ColumnType.BINARY, DefaultMissingValue = 1, Values = new string[2] { "male", "female" } };
            des.Features[3] = new Column { Id = 4, Name = "year", Index = 3, Type = ColumnType.NUMERIC, DefaultMissingValue = 15, Values = null };
            des.Features[4] = new Column { Id = 5, Name = "y", Index = 4, Type = ColumnType.BINARY, DefaultMissingValue = 1, Values = new string[2] { "no", "yes" } };

            des.LabelIndex = 4;
            return des;
        }

    }
}
