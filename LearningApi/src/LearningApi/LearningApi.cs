using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace LearningFoundation
{
    /// <summary>
    /// Main class for handling preprocessing, training, and prediction for data and ml algorithms.
    /// </summary>
    public class LearningApi
    {
        /// <summary>
        /// Gets/Sets DataProvider for loading of the data.
        /// </summary>
        public IDataProvider DataProvider { get; set; }
        /// <summary>
        /// Gets/Sets specifics ML algortim for training
        /// </summary>
        public IAlgorithm Algorithm { get; set; }
        /// <summary>
        /// Gets/Sets specifics normalization algoritm
        /// </summary>
        public IDataNormalizer Normilizer { get; set; }

        /// <summary>
        /// Used to map input columns to features.
        /// </summary>
        public IDataMapper DataMapper { get;set;}

        /// <summary>
        /// main constructor
        /// </summary>
        public LearningApi()
        {

        }

        public IScore GetScore()
        {
            return null;
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

                if (rawData != null)
                {
                    double[] data = this.DataMapper.MapInputVector(rawData);

                    double[] featureVector = new double[numOfFeatures];

                    for (int i = 0; i < numOfFeatures; i++)
                    {
                        featureVector[i] = data[this.DataMapper.GetFeatureIndex(i)];
                    }

                    var normFeatureVector = this.Normilizer.Normalize(featureVector);

                    await this.Algorithm.Train(normFeatureVector, data[labelIndx]);
                }
                else
                    break;//if the next item is null, we reached the end of the list
            }
        }
    }
}
