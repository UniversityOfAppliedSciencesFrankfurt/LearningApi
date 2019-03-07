using System;
using System.IO;


namespace CenterModuleUnitTests
{   /// <summary>
    /// This class saves the results of my Unitest to a Text-File
    /// </summary>
    public class Save
    {
        public void SaveFile(double[][] output)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CenteredImages\\result.txt");
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (var value in output)
                {
                    foreach (var saveValue in value)
                    {
                        writer.Write(saveValue);
                    }
                    writer.WriteLine();
                }
            }


        }
    }
}
