using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using System.IO;
using System.Collections;


namespace LearningFoundation.DataProviders
{
    /// <summary>
    /// DataProvider implementation in case data is coming from CSV file
    /// </summary>
    public class CsvDataParser : IDataProvider<object[]>
    {
        string[] m_Header;
        IEnumerable<object[]> list = new List<object[]>();

        private StringReader m_Reader;

        private string m_CurrentLine;

        private string m_StrContent;

        private char m_Delimiter;

        private int m_SkipRows;

        private bool isHeaderIncluded;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="delimiter"></param>
        /// <param name="isHeader"></param>
        /// <param name="skipRows"></param>
        public CsvDataParser(string strContent, char delimiter, bool isHeader, int skipRows = 0)
        {
            m_StrContent = strContent;
            m_Delimiter = delimiter;
            isHeaderIncluded = isHeader;
            m_SkipRows = skipRows;
            // TODO.. check if file exists.
            m_Reader = new StringReader(m_StrContent);
            for (int i = 0; i < m_SkipRows; i++)
            {
                m_CurrentLine = m_Reader.ReadLine();
            }
            //define header
            if (isHeaderIncluded)
            {
                m_CurrentLine = m_Reader.ReadLine();
                if (!string.IsNullOrEmpty(m_CurrentLine))
                {
                    m_Header = m_CurrentLine.Split(new char[] { m_Delimiter });
                }
            }

        }


        /// <summary>
        /// Represent the loaded data
        /// </summary>
        public IEnumerable<object[]> DataSet
        {
            get
            {
                return list;
            }
            set
            {
                if (list != value)
                    list = value;
            }
        }

        int m_Current = 0;

        /// <summary>
        /// main constructor
        /// </summary>
        public CsvDataParser()
        {
        }

        /// <summary>
        /// Header of the data set
        /// </summary>
        public string[] Header => m_Header;
        
        /// <summary>
        /// Current object of the enumerator
        /// </summary>
        public object[] Current
        {
            get
            {
                var strCols = m_CurrentLine.Split(m_Delimiter);

                //
                // Transform data from row->col in to col->row
                var rawData = new object[strCols.Length];

                // Read columns
                for (int i = 0; i < strCols.Length; i++)
                {
                    rawData[i] = strCols[i];
                }

                return rawData;               
            }
        }


        /// <summary>
        /// Current item of the enumerator
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return m_CurrentLine;
            }
        }

        /// <summary>
        /// Disposing the enumerator
        /// </summary>
        public void Dispose()
        {
            m_Reader.Dispose();
        }

        /// <summary>
        /// enumerator move one index forward
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            m_CurrentLine = m_Reader.ReadLine();
            if (m_CurrentLine != null)
                return true;
            else
                return false;
        }

        
        /// <summary>
        /// reset index of the enumerator
        /// </summary>
        public void Reset()
        {
            m_CurrentLine = null;
            //m_Reader.BaseStream.Position = 0;
        }


        public object[][] Run(object data, IContext ctx)
        {
            List<object[]> rawData = new List<object[]>();

            using (StringReader reader =  new StringReader(m_StrContent))
            {
                int linenum = 0;
                foreach (string line in readLineFromFile(reader))
                {
                    //split line in to column
                    var strCols = line.Split(m_Delimiter);

                    //skip first ... rows
                    var headerLine = isHeaderIncluded ? 1 : 0;
                    if(linenum < m_SkipRows+ headerLine)
                    {
                        linenum++;
                        continue;
                    }

                    //Transform data from row->col in to col->row
                    var singleRow = new object[strCols.Length];

                    //define columns
                    for (int i = 0; i < strCols.Length; i++)
                    {
                        singleRow[i] = strCols[i];
                    }

                    rawData.Add(singleRow);
                }

                return rawData.ToArray();
            }
        }

        /// <summary>
        /// Reading stream reader line by line with IEnumerable collection.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IEnumerable<string> readLineFromFile(StringReader reader)
        {
            string currentLine;
            while ((currentLine = reader.ReadLine()) != null)
            {
                yield return currentLine;
            }
        }

    }
}
