using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningFoundation;
using System.IO;
using System.Collections;

namespace LearningFoundation.DataProviders
{
    public class CsvDataProvider : IDataProvider
    {
        List<object[]> list = new List<object[]>();

        int m_Current = 0;

        public CsvDataProvider()
        {
        }

        public object[] Current
        {
            get
            {
                return list[m_Current];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
         
        }

        public bool MoveNext()
        {
            return m_Current < list.Count;
        }

        public void Reset()
        {
        }
    }
}
