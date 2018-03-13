using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.Arrays
{
    public static class OccurencesExt
    {
        public static object[] FindAllOcurrences(this object[] items)
        {
            List<object> founds = new List<object>();

            foreach (var item in items)
            {
                if (!founds.Contains(item))
                {
                    founds.Add(item);
                }
            }

            return founds.ToArray();
        }

        public static object[] FindOcurrencesOf(this object[] items, object[] targets)
        {
            if (targets == null)
                throw new NullReferenceException();
            
            List<object> founds = new List<object>();

            foreach (var item in items)
            {
                if (!founds.Contains(item))
                {
                    founds.Add(item);
                }
            }

            return founds.ToArray();
        }
    }
}
