using LearningFoundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.DataProviders
{
    /// <summary>
    /// Extension method for loading and parsing data from SCV file 
    /// </summary>
    public static class CsvDataProviderExtensions
    {
        /// <summary>
        /// perform the loading data from SCV file
        /// </summary>
        /// <param name="api">instance of the LearningAPI</param>
        /// <param name="fileName">csv file path</param>
        /// <param name="delimiter">csv delimiter</param>
        /// <param name="skipRows">firs several rows which should be skiped in parsing.</param>
        /// <returns></returns>
        public static LearningApi UseCsvDataProvider(this LearningApi api, string fileName, char delimiter, int skipRows = 0)       
        {
            var dp = new CsvDataProvider(fileName, delimiter, skipRows);
          
            api.AddModule(dp, "DataProvider");
            
            return api;
        }


        /// <summary>
        /// Creating dataset row by row
        /// </summary>
        /// <param name="fileName">csv file reade</param>
        /// <param name="delimeter">csvdelimiter</param>
        /// <returns></returns>
        public static IEnumerable<object[]> LoadDataFromFile(string fileName, char delimeter)
        {
            using (StreamReader reader = File.OpenText(fileName))
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
