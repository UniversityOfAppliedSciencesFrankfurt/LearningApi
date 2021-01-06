using RandomForest.Lib.Numerical.ItemSet.Feature;
using RandomForest.Lib.Numerical.ItemSet.Splitters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomForest.Lib.Numerical.ItemSet
{
    static class ExcelParser
    {
        public static ItemNumericalSet ParseItemNumericalSet(string path,  double[][] data,int sheetNo = 1)
        {
           
            int cols = 10;
            string[] header = new string[10];
            header[0] = "A";
            header[1] = "B";
            header[2] = "C";
            header[3] = "D";
            header[4] = "E";
            header[5] = "F";
            header[6] = "G";
            header[7] = "H";
            header[8] = "I";
            header[9] = "Z";
           
            //double[][] datacopy = data.Take(100).ToArray();
            int rows = data.Count();
            List<string> featureNames = new List<string>();
            for (int j = 0; j <= header.Length-1; j++)
            {
                var fn = header[j];
                if (fn == null)
                    throw new Exception();
                string str = fn.ToString();
                if (string.IsNullOrEmpty(str))
                    throw new Exception();
                featureNames.Add(str);
            }

            ItemNumericalSet set = new ItemNumericalSet(featureNames);

            for (int i = 0; i <= rows-1; i++)
            {
                FeatureNumericalValue[] arr = new FeatureNumericalValue[cols];
                for (int j = 0; j < cols; j++)
                {
                    var v = data[i][j];
                   // var v = sheet.Cells[i, j].Value;
                    arr[j] = new FeatureNumericalValue { FeatureName = featureNames[j], FeatureValue = Convert.ToDouble(v) };
                }
                set.AddItem(arr);
            }

            return set;
        }
    }
}
