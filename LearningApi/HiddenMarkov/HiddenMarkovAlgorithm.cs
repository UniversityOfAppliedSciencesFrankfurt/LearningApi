using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkov
{
    public class HiddenMarkovAlgorithm : IAlgorithm
    {
        public HiddenMarkovModel m_MyModel;
        public HiddenMarkovAlgorithm(int symbols, int states, HiddenMarkovModel.HiddenMarkovModelType type)      
        {
            m_MyModel = new HiddenMarkovModel(symbols,states,type);
        }

        public IResult Predict(double[][] data, IContext ctx)
        {
            HiddenMarkovResult Result = new HiddenMarkovResult();
            int numberOfRows = data.GetLength(0);
            Result.Probability = new List<double>();
            for (int i = 0; i < numberOfRows; ++i)
                Result.Probability.Add(m_MyModel.Evaluate(data[i])); // m_MyModel.Evaluate(data[i])
            return Result;
        }

        public IScore Run(double[][] data, IContext ctx)
        {
            throw new NotImplementedException();
        }

        public IScore Train(double[][] data, IContext ctx)
        {
            HiddenMarkovScore Score = new HiddenMarkovScore();
            HiddenMarkovContext MarkovCtx = new HiddenMarkovContext();
            MarkovCtx = (HiddenMarkovContext)ctx;
            Score.AvgLogLikelihood = m_MyModel.Learn(data,MarkovCtx.iterations,MarkovCtx.tolerance);
            return Score;
        }


    }
}
