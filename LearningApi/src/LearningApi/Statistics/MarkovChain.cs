using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LearningFoundation.Arrays;

namespace LearningFoundation.Statistics
{
    public class MarkovChain
    {
        public static double[] CalculateFirstOrder(object[][] sequences, object[] targets = null)
        {
            if (targets == null)
            {
                throw new NotImplementedException();
                targets = new object[0].FindAllOcurrences();
            }

            Task[] tasks = new Task[targets.Length];
            double[] sums = new double[targets.Length];

            var task = new TaskFactory().StartNew(new Action<object>((test) =>
            {
                Console.WriteLine(test);
            }), sums);

            int k = 0;

            foreach (var target in targets)
            {                
                tasks[k] = new TaskFactory().StartNew(new Action<object>((state)=>
                {
                    long denominator = 0;

                    int safeIndex = ((State)state).Index;
                    object safeTarget = ((State)state).Target;

                    sums[safeIndex] = 1;

                    foreach (var sequence in sequences)
                    {
                        for (int i = 0; i < sequence.Length; i++)
                        {
                            if (sequence[i].Equals(safeTarget))
                            {
                                sums[safeIndex] = sums[safeIndex]+1;
                                System.Diagnostics.Debug.WriteLine($"target={safeTarget}, sq(i)={sequence[i]}, sum={sums[safeIndex]}");
                            }
                        }

                        denominator += sequence.Length;
                    }

                    sums[safeIndex] = sums[safeIndex] / (targets.Length + denominator);
                }), new State {  Index = k, Target = target});

                k++;
            }

            Task.WaitAll(tasks);

            return sums;
        }


        private class State
        {
            public int Index { get; set; }

            public object Target { get; set; }
        }
    }
}
