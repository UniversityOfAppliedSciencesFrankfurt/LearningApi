using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using LearningFoundation.DataMappers;
using LearningFoundation.PersistenceProviders;
using System.Runtime.Serialization;

namespace LearningFoundation
{
    /// <summary>
    /// Main class for handling preprocessing, training, and prediction for data and ml algorithms.
    /// </summary>
    public class LearningApi
    {
        /// <summary>
        /// Provider used for loading and saving of models.
        /// </summary>
        private IModelPersistenceProvider m_PersistenceProvider;

        public IContext Context { get; set; }

        [DataMember]
        public Dictionary<string, IPipelineModule> Modules { get; set; }

        /// <summary>
        /// QNames of registered modules.
        /// </summary>
        [DataMember]
        protected Dictionary<string, string> ModulesQulifiedNames { get; set; }

        /// <summary>
        /// Gets/Sets specifics ML algortim for training
        /// </summary>
        [IgnoreDataMember]
        public IAlgorithm Algorithm
        {
            get
            {
                foreach (var module in this.Modules)
                {
                    if (module.Value is IAlgorithm)
                        return module.Value as IAlgorithm;
                }

                return null;
            }
        }


        /// <summary>
        /// Main constructor.
        /// Initializes the API with specified internally managed context and persistence provider.
        /// </summary>
        /// <param name="desc">Describes the data and features.</param>
        public LearningApi(DataDescriptor desc = null, IModelPersistenceProvider persistenceProvider = null)
        {
            this.Context = new Context() { DataDescriptor = desc };

            this.Modules = new Dictionary<string, LearningFoundation.IPipelineModule>();

            if (persistenceProvider == null)
                this.m_PersistenceProvider = new JsonPersistenceProvider();
        }


        /// <summary>
        /// Gets the module of specified type and name.
        /// </summary>
        /// <typeparam name="T">Type of the module.</typeparam>
        /// <param name="name">[Optional]: Name of the module.</param>
        /// <returns>The module instance.</returns>
        public T GetModule<T>(string name = null) where T : IPipelineModule
        {

            if (name == null)
                return (T)this.Modules.FirstOrDefault(m => m.GetType().Name == typeof(T).Name).Value;
            else
                return (T)this.Modules.FirstOrDefault(m => m.Key == name).Value;
        }

        public LearningApi AddModule(IPipelineModule module, string name = null)
        {
            // TODO: Need few checks here. Dbl key name, module == null,..
            if (name == null)
                name = module.GetType().Name;

            this.Modules.Add(name, module);

            //if (module is IAlgorithm)
            //    this.Algorithm = module as IAlgorithm;

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

                var tmpData = module.Run(data, this.Context);
                if (tmpData == null)
                {
                    data = this.Context.Score;
                    break;
                }
                else
                    data = tmpData;
            }

            return data;
        }

        public object RunBatch()
        {
            dynamic data = null;

            do
            {
                data = Run();
            } while (this.Context.IsMoreDataAvailable);

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


        public static LearningApi Load(string modelName, IModelPersistenceProvider persistenceProvider = null)
        {
            LearningApi api = new LearningApi(persistenceProvider: persistenceProvider);
            return api.m_PersistenceProvider.Load(modelName);
        }

        /// <summary>
        /// Saves the current state of algorithm.This method saves all LearningApi instance.
        /// </summary>
        public void Save(string modelName)
        {
            m_PersistenceProvider.Save(modelName, this);
        }
    }
}
