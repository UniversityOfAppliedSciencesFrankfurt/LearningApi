using LearningFoundation;
using System;

namespace SurvivalAnalysis
{
    public class SurvivalAnalysis : IAlgorithm
    {
        private Random m_Rnd;

        internal int Iterations { get; set; }

        public SurvivalAnalysis()
        {
           
            m_Rnd = new Random((int)DateTime.Now.Ticks);
        }

        public double[] Predict(double[][] data, IContext ctx)
        {
            throw new NotImplementedException();
        }

        public IScore Run(double[][] data, IContext ctx)
        {
            throw new NotImplementedException();
        }

        public IScore Train(double[][] data, IContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
