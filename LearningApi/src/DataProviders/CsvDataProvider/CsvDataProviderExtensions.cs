using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation.DataProviders
{
    public static class CsvDataProviderExtensions
    {
        public static LearningApi UseCsvDataProvider(this LearningApi api, string fileName, char delimiter, int skipRows = 0)       
        {
            api.DataProvider = new CsvDataProvider();
            return api;
        }
    }
}
