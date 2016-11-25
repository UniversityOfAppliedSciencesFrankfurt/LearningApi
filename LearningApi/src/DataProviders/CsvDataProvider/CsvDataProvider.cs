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
    /// DataProvider implementation in case data is comming from CSV file
    /// </summary>
    public class CsvDataProvider : IDataProvider
    {
        //
        IEnumerable<object[]> list = new List<object[]>();

        /// <summary>
        /// Respesent the loaded data
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
        /// Current object of the enumerator
        /// </summary>
        public object[] Current
        {
            get
            {
                var val= list.ElementAtOrDefault(m_Current);
                if (val == null)
                    Reset();
                return val;
            }
        }
        /// <summary>
        /// Current item of the enumerator
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Disposing the enumerator
        /// </summary>
        public void Dispose()
        {
         
        }

        /// <summary>
        /// enumerator move one index forward
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            m_Current++;
            return true;//m_Current < list.Count();
        }
        /// <summary>
        /// reset index of the enumerator
        /// </summary>
        public void Reset()
        {
            m_Current = 0;
        }
    }
}
