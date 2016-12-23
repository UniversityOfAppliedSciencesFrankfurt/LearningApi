using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.DataMappers
{
    /// <summary>
    /// Class for asigning set of properties for each feature (data column)
    /// </summary>
    public class DataMapper : IDataMapper<object[][], double[][]>
    {
       // IDataDescriptor m_DataContext = new DataDescriptor();

        /// <summary>
        /// main constructor
        /// </summary>
        public DataMapper()
        {

        }

        ///// <summary>
        /////array of feature which play role in training 
        ///// </summary>
        //public Column[] Features { get; set; }

                
        //private int m_LabelIndex;

        ///// <summary>
        ///// Position/Index of the Label column in data row
        ///// </summary>
        //public int LabelIndex
        //{
        //    get
        //    {
        //        return m_LabelIndex;
        //    }

        //    set
        //    {
        //        m_LabelIndex = value;
        //    }
        //}

        //private int m_NumOfFeatures;

        ///// <summary>
        ///// Number of feature used in training of label
        ///// </summary>
        //public int NumOfFeatures
        //{
        //    get
        //    {
        //        return m_NumOfFeatures;
        //    }

        //    set
        //    {
        //        this.m_NumOfFeatures = value;
        //    }
        //}

        ///// <summary>
        ///// returns the rawData index of specific feature
        ///// </summary>
        ///// <param name="feature"></param>
        ///// <returns></returns>
        //public int GetFeatureIndex(int feature)
        //{
        //    return Features[feature].Index;
        //}


        /// <summary>
        /// Transform the featureVector from natural format in to double format.
        /// If the data is loaded from the file, all data are in string format regadless of the type of the column.
        /// THis method transform string values in to numeric value. Also in case of the binary or the category type
        /// it transforms binary or category value in to number representation of the class. Eg. 0 or 1 in case of binary, 
        /// and 0,1,2,3...n, whre n is number of categories, in case of category type.
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>


        public double[][] Run(object[][] data, IContext ctx)
        {
            List<List<double>> rows = new List<List<double>>();

            for (int i = 0; i < data.Length; i++)
            {
                List<double> raw = new List<double>();

                object[] vector = ((object[])data[i]);

                //
                // Transform rawData in to raw of Fetures with proper type, normalization value, and coresct binary and catogery type 
                for (int featureIndx = 0; featureIndx < data[0].Length; featureIndx++)
                {
                    var col = ctx.DataDescriptor.Features[featureIndx];
                    if (col.Type == ColumnType.STRING)
                        continue;
                    else if (col.Type == ColumnType.NUMERIC)//numeric column
                    {
                        var val = vector[col.Index];
                        double value = double.NaN;

                        //in case of invalid (missing) value, value must be replaced with defaultMIssing value
                        if (!double.TryParse(val.ToString(), out value))
                            value = col.DefaultMissingValue;

                        //
                        raw.Add(value);
                    }
                    else if (col.Type == ColumnType.BINARY)//binary column
                    {
                        if (col.Values[0].Equals(data[col.Index]))
                            raw.Add(0);
                        else if (col.Values[1].Equals(data[col.Index]))
                            raw.Add(1);
                        else//in case of invalid (missing) value, value must be replaced with defaultMIssing value
                            raw.Add(col.DefaultMissingValue);
                    }
                    else if (col.Type == ColumnType.CLASS)//multiclass column
                    {
                        //add as many columns as number of categories
                        //eg. red, greeen,blue -  categories
                        // for red -> 0
                        // for green -> 1
                        // for blue -> 2
                        var numClass = col.Values.Length;
                        bool isMissigValue = true;
                        for (int j = 0; j < numClass; j++)
                        {
                            if (col.Values[j].Equals(data[col.Index]))
                            {
                                raw.Add(j);
                                isMissigValue = false;
                                break;
                            }
                        }

                        //in case of missing value
                        if (isMissigValue)
                            raw.Add(col.DefaultMissingValue);
                    }                   
                }

                rows.Add(raw);
            }

          
            ctx.DataDescriptor.NumOfFeatures = rows.FirstOrDefault().Count;

            // Returns rows of double value feture vectors
            return rows.Select(r => r.ToArray()).ToArray();
        }
    }

    /// <summary>
    /// Implementation of the data column used in Data Mapper 
    /// </summary>
    public class Column
    {
        /// <summary>
        /// Feature Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Feature (Column) name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Feature position in raw data
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The Type of the column (feature)
        /// </summary>
        public ColumnType Type { get; set; }

        /// <summary>
        /// In case of binary and Category type, values are enumerated in ascedenting order
        /// binary:
        /// {false,true} - mean: 0->false, 1->true
        /// {no, yes}; - mean: 0->no, 1->yes  
        /// {0, 1}; - mean: 0->0, 1->1
        /// 
        /// multiclass: 1->n representation 
        /// {Red, Green, Blue}; - mean: (Red=0, Green=1, Blue=2) normalized values: Red-> (1,0,0), Green ->(0,1,0), Blue ->(0,0,1) 
        /// </summary>
        public string[] Values { get; set; }

        /// <summary>
        /// Replaces the null value in the cell
        /// </summary>
        public double DefaultMissingValue { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ColumnType
    {
        STRING,// 0 - string (avoided in training and testing)
        NUMERIC,// 1 - numeric
        BINARY,// 2 - binary
        CLASS// 3 - multiclass with x categories
    }

    /// <summary>
    /// 
    /// </summary>
    public class ResultMapping
    {
        public Dictionary<string, double> Mappings { get; set; }
    }
}
