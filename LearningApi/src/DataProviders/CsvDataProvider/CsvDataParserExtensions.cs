using LearningFoundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.DataProviders
{
    /// <summary>
    /// Extension method for parsing data from CSV like string  
    /// </summary>
    public static class CsvDataParserExtensions
    {
        /// <summary>
        /// perform the loading data from SCV file
        /// </summary>
        /// <param name="api">instance of the LearningAPI</param>
        /// <param name="strContent">csv string</param>
        /// <param name="delimiter">csv delimiter</param>
        /// <param name="isHeader"> is header included in the data after skiped rows. </param>
        /// <param name="skipRows">firs several rows which should be skiped in parsing.</param>
        /// <returns></returns>
        public static LearningApi UseCsvDataParser(this LearningApi api, string strContent, char delimiter, bool isHeader, int skipRows = 0)       
        {
            var dp = new CsvDataParser(strContent, delimiter, isHeader, skipRows);
          
            api.AddModule(dp, "CsvDataParser");
            
            return api;
        }


        /// <summary>
        /// Creating dataset row by row
        /// </summary>
        /// <param name="strContent">csv string reade</param>
        /// <param name="delimeter">csv delimiter</param>
        /// <returns></returns>
        public static IEnumerable<object[]> LoadDataFromFile(string strContent, char delimeter)
        {
            using (StringReader reader = new StringReader(strContent))
            {
                //
                foreach (string line in readLineFromFile(reader))
                {

                    //split line in to column
                    var strCols = line.Split(delimeter);

                    //Transform data from row->col in to col->row
                    var rawData = new object[strCols.Length];

                    //define columns
                    for (int i = 0; i < strCols.Length; i++)
                    {
                        rawData[i] = strCols[i];
                    }

                    yield return rawData;
                }
            }                
        }

        /// <summary>
        /// Reading string reader line by line with IEnumerable collection.
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
