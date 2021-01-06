using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DecisonForestRegressionAlgorithm
{
    static class DirectoryInfoExtensions
    {
        internal static void Clear(this DirectoryInfo di)
        {
            foreach (var fi in di.EnumerateFiles())
                fi.Delete();
        }
    }
}
