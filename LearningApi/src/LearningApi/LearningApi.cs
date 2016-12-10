using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;


namespace LearningFoundation
{
    /// <summary>
    /// Main class for handling preprocessing, training, and prediction for data and ml algorithms.
    /// </summary>
    public class LearningApi
    {
        public IContext Context { get; set; }

        public Dictionary<string, IPipelineModule> Modules { get; internal set; }

        /// <summary>
        /// Gets/Sets DataProvider for loading of the data.
        /// </summary>
        //public IDataProvider DataProvider { get; set; }
        
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
        //public IDataMapper DataMapper { get;set;}

        /// <summary>
        /// main constructor
        /// </summary>
        /// <param name="desc">Describes the data and features.</param>
        public LearningApi(DataDescriptor desc)
        {
            this.Context = new Context() { DataDescriptor = desc };

            this.Modules = new Dictionary<string, LearningFoundation.IPipelineModule>();
        }


        /// <summary>
        /// Gets the module of specified type and name.
        /// </summary>
        /// <typeparam name="T">Type of the module.</typeparam>
        /// <param name="name">[Optional]: Name opf the module.</param>
        /// <returns>The module instance.</returns>
        public T GetModule<T>(string name = null)  where T : IPipelineModule
        {
            if (name == null)
                return (T)this.Modules.FirstOrDefault(m => m.GetType().Name == typeof(T).Name).Value;
            else
                return (T)this.Modules.FirstOrDefault(m => m.GetType().Name == name).Value;
        }

        public LearningApi AddModule(IPipelineModule module, string name = null)
        {
            // TODO: Need few checks here. Dbl key name, module == null,..
            if (name == null)
                name = module.GetType().Name;

            this.Modules.Add(name, module);

            return this;
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
            var Normalizer = GetModule<IDataNormalizer>();
            if (Normalizer == null)
                return featureVector;
            else
                return Normalizer.Run(featureVector, this.Context);
        }

        /// <summary>
        /// Transform normalized in to numeric row 
        /// </summary>
        /// <param name="featureVector"></param>
        /// <returns></returns>
        private double[] DeNormalize(double[] normVector)
        {
            IDataDeNormalizer Normalizer = GetModule<IDataDeNormalizer>() as IDataDeNormalizer;
            if (Normalizer == null)
                throw new MLException("There is no module registered of type 'IDataDeNormalizer'");
            else
                return Normalizer.DeNormalize(normVector, this.Context);
        }

        public async Task TrainAsync()
        {
            if (this.Modules.Count <= 1)
                throw new MLException("Uninitialised pipeline.");

            if(!(this.Modules.First().Value is IDataProvider<object[]>))
                throw new MLException("Uninitialised pipeline.");

            IDataProvider<object[]> dataProvider = (IDataProvider<object[]>)this.Modules.First().Value;
            
           
            /*
            
             Exit Criteria:
             1. Num of iterations
             2. err < some min err

            */

            while(dataProvider.MoveNext() /* ||  exitStrategy.ShouldExit(this.Modules.GetModule<IAlgorithm>().GetResult()) */)
            {  
                int k = 0;

                dynamic vector = dataProvider.Current;
             
                foreach (var item in this.Modules)
                {
                    if (k++ == 0) continue;                    
                   
                    dynamic module = item.Value;
                
                    vector = module.Run(vector, this.Context);
                }              
            }
        }

        /*
        /// <summary>
        /// Enumerates all data and runs a single training epoch. 
        /// </summary>
        /// <returns></returns>
        public async Task TrainAsyncOLD()
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
        }*/
    }
}
