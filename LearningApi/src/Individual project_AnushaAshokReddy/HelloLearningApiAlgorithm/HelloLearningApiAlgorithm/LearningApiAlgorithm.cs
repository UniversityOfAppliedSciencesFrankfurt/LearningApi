using LearningFoundation;
using System;
using System.Collections.Generic;

namespace HelloLearningApiAlgorithm
{
    public class LearningApiAlgorithm : IAlgorithm
    {
        public IResult Predict(double[][] data, IContext ctx)
        {
            LearningApiAlgorithmResult res = new LearningApiAlgorithmResult()
            {
                PredictedValue = new double[data.Length],
            }; ;

            for (int i = 0; i < data.GetLength(0); i++)
            {
                // logic for prediction
                res.PredictedValue[i] = ((LearningApiAlgorithmScore)ctx.Score).ScoreValue * data[i][0];
            }
            return res;
        }

        public IScore Run(double[][] data, IContext ctx)
        {
            return Train(data, ctx);
        }

        public IScore Train(double[][] data, IContext ctx)
        {
            if (ctx.Score as LearningApiAlgorithmScore == null)
                ctx.Score = new LearningApiAlgorithmScore();

            double sumTemp = 0;
            double avgTemp = 0;
            double sumChance = 0;
            double avgChance = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                // Initial first row and first column
                sumTemp += data[i][0];
                // Initial first row and second column
                sumChance += data[i][1];
            }
            //Calculating the average Temparature and Chance of preciitation
            avgTemp = sumTemp / data.GetLength(0);
            // calculating the chance of presipitation mean
            avgChance = sumChance / data.GetLength(0);

            LearningApiAlgorithmScore res = ctx.Score as LearningApiAlgorithmScore;

            // score value variabel conatains the averagechance divided by avg temp
            res.ScoreValue = avgChance / avgTemp;

            return ctx.Score;
        }
    }
}




