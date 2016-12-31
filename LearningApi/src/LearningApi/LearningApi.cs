using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using LearningFoundation.DataMappers;

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
        /// Used to map input columns to features.
        /// </summary>
        //public IDataMapper DataMapper { get;set;}

        /// <summary>
        /// main constructor
        /// </summary>
        /// <param name="desc">Describes the data and features.</param>
        public LearningApi(DataDescriptor desc = null, IScore score = null)
        {
            this.Context = new Context() { DataDescriptor = desc, Score = score };

            this.Modules = new Dictionary<string, LearningFoundation.IPipelineModule>();
        }


        /// <summary>
        /// Gets the module of specified type and name.
        /// </summary>
        /// <typeparam name="T">Type of the module.</typeparam>
        /// <param name="name">[Optional]: Name opf the module.</param>
        /// <returns>The module instance.</returns>
        public T GetModule<T>(string name = null) where T : IPipelineModule
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


        /// <summary>
        /// Gets the score.
        /// </summary>
        /// <returns></returns>
        public IScore GetScore()
        {
            return Context.Score;
        }

        
        

        public object Run()
        {
            dynamic data = null;

            foreach (var item in this.Modules)
            {
                dynamic module = item.Value;

                data = module.Run(data, this.Context);
            }

            return data;
        }

        public async Task TrainAsync()
        {
            if (this.Modules.Count <= 1)
                throw new MLException("Uninitialised pipeline.");

            if (!(this.Modules.First().Value is IDataProvider<object[]>))
                throw new MLException("Uninitialised pipeline.");

            IDataProvider<object[]> dataProvider = (IDataProvider<object[]>)this.Modules.First().Value;


            /*
            
             Exit Criteria:
             1. Num of iterations
             2. err < some min err

            */

            while (dataProvider.MoveNext() /* ||  exitStrategy.ShouldExit(this.Modules.GetModule<IAlgorithm>().GetResult()) */)
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
    }
}
