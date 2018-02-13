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
    public class CsvDataProvider : IDataProvider<object[]>
    {
        string[] m_Header;
        IEnumerable<object[]> list = new List<object[]>();

        private StreamReader m_Reader;

        private string m_CurrentLine;

        private string m_FileName;

        private char m_Delimiter;

        private int m_SkipRows;

        private bool isHeaderIncluded;

        /// <summary>
        /// Mini-batch size.
        /// It must be define at the beginning of the iteration process
        /// In case it is not defined (LTE 0) mini-batching will not be happen.In fact will be performed full batching
        /// </summary>
        private int m_BatchSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="delimiter"></param>
        /// <param name="isHeader"></param>
        /// <param name="skipRows"></param>
        public CsvDataProvider(string fileName, char delimiter, bool isHeader, int batchSize ,int skipRows = 0)
        {
            m_BatchSize = batchSize;
            m_FileName = fileName;
            m_Delimiter = delimiter;
            isHeaderIncluded = isHeader;
            m_SkipRows = skipRows;
            // TODO.. check if file exists.
            m_Reader = File.OpenText(m_FileName);
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
        public CsvDataProvider()
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
            if(m_Reader!=null)
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
            m_Reader.BaseStream.Position = 0;
        }


        public object[][] Run(object data, IContext ctx)
        {
            //check if mini-batch is defined
            if (m_BatchSize > 0)
            {
                return RunBatch(data, ctx);
            }

            List<object[]> rawData = new List<object[]>();

            int linenum = 0;
            foreach (string line in readLineFromFile(m_Reader))
            {
                //split line in to column
                var strCols = line.Split(m_Delimiter);

                //skip first ... rows
                var headerLine = isHeaderIncluded ? 1 : 0;
                if (linenum < m_SkipRows + headerLine)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private object[][] RunBatch(object data, IContext ctx)
        {
            
            //
            int currentMBI = ctx.BatchIteration;

            List<object[]> rawData = new List<object[]>();

            using (StreamReader reader = File.OpenText(m_FileName))
            {
                int linenum = 0;
                foreach (string line in readLineFromFile(reader).Skip(currentMBI * m_BatchSize).Take(m_BatchSize))
                {
                    //split line in to column
                    var strCols = line.Split(m_Delimiter);

                    //skip first ... rows
                    var headerLine = isHeaderIncluded ? 1 : 0;
                    if (linenum < m_SkipRows + headerLine)
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

                //check if the miniBatch iteration reach to the end of file.
                ctx.IsMoreDataAvailable = !reader.EndOfStream;

                return rawData.ToArray();
            }
        }

        /// <summary>
        /// Reading stream reader line by line with IEnumerable collection.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IEnumerable<string> readLineFromFile(StreamReader reader)
        {
            string currentLine;
            while ((currentLine = reader.ReadLine()) != null)
            {
                yield return currentLine;
            }
        }

    }
}
