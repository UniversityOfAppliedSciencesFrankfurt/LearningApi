using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace test.RestrictedBolzmannMachine2
{
    public class RBMHammingDistance
    {
        static RBMHammingDistance()
        {

        }

        public void hammingDistance(double[][] test,double[][] predicted)
        {
            StreamWriter tw = new StreamWriter("hammingDistance.txt");
            double[][] hDistance = new double[test.Length][];
            double[] h = new double[test.Length];
            
            
            for (int i =0; i<test.Length; i++)
            {
                if (test[i].Length != predicted[i].Length)
                {
                    throw new Exception("Data must be equal length");
                }
                int s = 0;
                for (int j = 0; j<predicted[i].Length; j++)
                {
                    if(test[i][j] == predicted[i][j])
                    {
                        s = s + 1; 
                    }
                    else
                    {
                        s = s+0;
                    }
                }
                h[i] = s;

                float Accuracy = ((predicted[i].Length - s) / predicted[i].Length) * 100;
                tw.Write("Hamming Distance for Image"+i+" is:"+s );
                tw.Write("\t\t Accuracy: " + Accuracy);
                tw.WriteLine();
                
            }
            tw.Close();
            
            
        }
    }
}
