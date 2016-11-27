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
        public IDataNormalizer Normalizer { get; set; }

        /// <summary>
        /// Gets/Sets specifics normalization algoritm
        /// </summary>
        public IStatistics Statistics { get; set; }

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
        /// Transform numeric to normalized row 
        /// </summary>
        /// <param name="featureVector"></param>
        /// <returns></returns>
        private double[] Normalize(double[] featureVector)
        {
            if (Normalizer == null)
                return featureVector;
            else
                return Normalizer.Normalize(featureVector);
        }

        /// <summary>
        /// Transform normalized in to numeric row 
        /// </summary>
        /// <param name="featureVector"></param>
        /// <returns></returns>
        private double[] DeNormalize(double[] normVector)
        {
            if (Normalizer == null)
                return normVector;
            else
                return Normalizer.DeNormalize(normVector);
        }

        public async Task TrainAsync()
        {
        }

        /// <summary>
        /// Enumerates all data and runs a single training epoch. 
        /// </summary>
        /// <returns></returns>
        public async Task TrainAsync2()
        {
            int numOfFeatures = this.DataMapper.NumOfFeatures;
            int labelIndx = this.DataMapper.LabelIndex;

            do
            {
                var rawData = this.DataProvider.Current;

                if (rawData != null)
                {
                    object[] data = this.DataMapper.RunAsync(rawData);

                    double[] featureVector = new double[numOfFeatures];

                    for (int i = 0; i < numOfFeatures; i++)
                    {
                        featureVector[i] = (double)data[this.DataMapper.GetFeatureIndex(i)];
                    }

                    var normFeatureVector = Normalize(featureVector);

                    ///
                    await this.Algorithm.Train(normFeatureVector, (double)data[labelIndx]);
                }
                else
                    break;//if the next item is null, we reached the end of the list
            } while (this.DataProvider.MoveNext());
        }
    }
}
