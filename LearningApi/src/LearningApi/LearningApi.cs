using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace LearningFoundation
{
    public class LearningApi
    {
        /// <summary>
        /// Gets/Sets DataProvider for loading of the data.
        /// </summary>
        public IDataProvider DataProvider { get; set; }

        public IAlgorithm Algorithm { get; set; }

        public IDataNormilizer Normilizer { get; set; }

        /// <summary>
        /// Used to map input columns to features.
        /// </summary>
        public IDataMapper DataMapper { get;set;}

        public LearningApi()
        {

        }

        public IScore GetScore()
        {
            
        }
        
        /// <summary>
        /// Enumerates all data and runs a single training epoch. 
        /// </summary>
        /// <returns></returns>
        public async Task TrainAsync()
        {
            int numOfFeatures = this.DataMapper.NumOfFeatures;
            int labelIndx = this.DataMapper.LabelIndex;

            while (this.DataProvider.MoveNext())
            {
                var rawData = this.DataProvider.Current;

                double[] data = this.DataMapper.MapInputVector(rawData);

                double[] featureVector = new double[numOfFeatures];

                for (int i = 0; i < numOfFeatures; i++)
                {
                    featureVector[i] = data[this.DataMapper.GetFeatureIndex(i)];
                }

                var normFeatureVector = this.Normilizer.Normilize(featureVector);

                await this.Algorithm.Train(normFeatureVector, data[labelIndx]);
            }
        }
    }
}
