using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    public class DataMapper : IDataMapper
    {
        private Column[] m_Features;

        private Column m_Label;

        //private doub
        public DataMapper()
        {

        }

        private int m_LabelIndex;

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

        public int GetFeatureIndex(int feature)
        {
            return 1;
        }

        public double[] MapInputVector(object[] rawData)
        {
            return new double[NumOfFeatures]; //TODO..
        }
    }

    public class Column
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Index { get; set; }

        public double DefaultMissingValue { get; set; }
    }


    /// <summary>
    /// Defines how to map categorical values to numerical values. 
    /// </summary>
    public class ResultMapping
    {
        public Dictionary<string, double> Mappings { get; set; }
    }
}
