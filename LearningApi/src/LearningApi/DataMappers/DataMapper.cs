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
    public class DataMapper : IDataMapper
    {
        /// <summary>
        ///array of feature which play role in training 
        /// </summary>
        public Column[] Features { get; set; }

        /// <summary>
        /// main constructor
        /// </summary>
        public DataMapper()
        {

        }

        private int m_LabelIndex;
        /// <summary>
        /// Position/Index of the Label column in data row
        /// </summary>
        public int LabelIndex
        {
            get
            {
                return m_LabelIndex;
            }

            set
            {
                m_LabelIndex = value;
            }
        }

        private int m_NumOfFeatures;

        /// <summary>
        /// Number of feature used in training of label
        /// </summary>
        public int NumOfFeatures
        {
            get
            {
                return m_NumOfFeatures;
            }

            set
            {
                this.m_NumOfFeatures = value;
            }
        }

        /// <summary>
        /// returns the rawData index of specific feature
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public int GetFeatureIndex(int feature)
        {
            return Features[feature].Index;
        }

        /// <summary>
        /// Transform the featureVector from natural format in to double format.
        /// If the data is loaded from the file, all data are in string format regadless of the type of the column.
        /// THis method transform string values in to numeric value. Also in case of the binary or the category type
        /// it transforms binary or category value in to number representation of the class. Eg. 0 or 1 in case of binary, 
        /// and 0,1,2,3...n, whre n is number of categories, in case of category type.
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public double[] MapInputVector(object[] rawData)
        {
            List<double> raw = new List<double>();

            //transform rawData in to raw of Fetures with proper type, normalization value, and coresct binary and catogery type 
            for(int i=0; i< rawData.Length; i++)
            {
               
                var col= Features[i];
                if (col.Type == 0)
                    continue;
                else if(col.Type == 1)//numeric column
                {
                    var val = rawData[col.Index];
                    double value = double.NaN;
                    if (!double.TryParse(val.ToString(), out value))
                        value = double.NaN;
                    //
                    raw.Add(value);
                }
                else if(col.Type == 2)//binary column
                {
                    if (col.Values[0].Equals(rawData[col.Index]))
                        raw.Add(0);
                    else
                        raw.Add(1);
                }
                else if(col.Type == 3)//multiclass column
                {
                    //add as many columns as number of categories
                    //eg. red, greeen,blue -  categories
                    // for red -> 0
                    // for green -> 1
                    // for blue -> 2
                    var numClass = col.Values.Length;
                    for (int j=0; j<numClass; j++)
                    {
                        if(col.Values[j].Equals(rawData[col.Index]))
                        {
                            raw.Add(j);
                            break;
                        }
                        
                    }
                }

            }
            //callculate number of features
            NumOfFeatures = raw.Count;
            //return double value feture vector
            return raw.ToArray();
        }

        /// <summary>
        /// Initialize mapper from file
        /// </summary>
        /// <param name="filePath">path of the file contining mapper configuration</param>
        /// <returns>.Net data mapper object</returns>
        public static DataMapper Load(string filePath)
        {
           string strContent = System.IO.File.ReadAllText(filePath);
           //
            var dm = JsonConvert.DeserializeObject(strContent, typeof(DataMapper));
            //
            if (dm is DataMapper)
                return (DataMapper)dm;
            else
                return null;
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
        /// The Type of the feature
        /// 0 - string (avoided in training and testing)
        /// 1 - numeric
        /// 2 - binary
        /// 3 - multiclass with x categories
        /// 
        /// </summary>
        public int Type { get; set; }

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
        /// Type of normalizations can be found at: https://en.wikipedia.org/wiki/Feature_scaling
        /// 0 - none
        /// 1 - minmax
        /// 2 - gaus
        /// 3 - custom
        /// </summary>
        public int Normalization { get; set; }

        /// <summary>
        /// Replaces the null value in the cell
        /// </summary>
        public double DefaultMissingValue { get; set; }
    }


    /// <summary>
    /// todo
    /// </summary>
    public class ResultMapping
    {
        public Dictionary<string, double> Mappings { get; set; }
    }
}
