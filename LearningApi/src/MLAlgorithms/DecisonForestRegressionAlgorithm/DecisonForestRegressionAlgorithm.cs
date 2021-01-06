using LearningFoundation;
using RandomForest.Lib.Numerical.Interfaces;
using RandomForest.Lib.Numerical.ItemSet;
using RandomForest.Lib.Numerical.ItemSet.Item;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;


namespace DecisonForestRegressionAlgorithm
{

    /// <summary>
    /// DecisonForestRegressionAlgorithm to predict regression or clasification data
    /// </summary>
    public class DecisonForestRegressionAlgorithm : IAlgorithm
    {
        #region mywork

        private Dictionary<string, Func<string>> validationDictionary = new Dictionary<string, Func<string>>();
        private Dictionary<string, bool> errorsDictionary = new Dictionary<string, bool>();
        private readonly ObservableCollection<NameValue> nameValueList = new ObservableCollection<NameValue>();
        private int numberOfTrees = 10;
        private int maxNumberOfTainingItemsInCategory = 5;
        private float trainingSubsetCountRatio = 0.6f;
        private string trainingSet = string.Empty;
        private string exportFolder = string.Empty;
        private string resolutionFeatureName = string.Empty;
        private IForest forest;
     
        private string result = string.Empty;
       
        /// <summary>
        /// Number of trees
        /// </summary>
        public int NumberOfTrees
        {
            get { return numberOfTrees; }
            set
            {
                if (numberOfTrees == value) return;
                numberOfTrees = value;
             
            }
        }
        /// <summary>
        /// Max Number Of Taining Items In Category
        /// </summary>
        public int MaxNumberOfTainingItemsInCategory
        {
            get { return maxNumberOfTainingItemsInCategory; }
            set
            {
                if (maxNumberOfTainingItemsInCategory == value) return;
                maxNumberOfTainingItemsInCategory = value;
              
            }
        }
        /// <summary>
        /// TrainingSubsetCountRatio 
        /// </summary>
        public float TrainingSubsetCountRatio
        {
            get { return trainingSubsetCountRatio; }
            set
            {
                if (trainingSubsetCountRatio == value) return;
                trainingSubsetCountRatio = value;
              
            }
        }
        /// <summary>
        /// Training Set
        /// </summary>
        public string TrainingSet
        {
            get { return trainingSet; }
            set
            {
                if (trainingSet == value) return;
                trainingSet = value;
             
            }
        }
        /// <summary>
        /// Export Folder
        /// </summary>
        public string ExportFolder
        {
            get { return exportFolder; }
            set
            {
                if (exportFolder == value) return;
                exportFolder = value;
             
            }
        }
        /// <summary>
        /// ResolutionFeatureName
        /// </summary>
        public string ResolutionFeatureName
        {
            get { return resolutionFeatureName; }
            set
            {
                if (resolutionFeatureName == value) return;
                resolutionFeatureName = value;
               
            }
        }
       
        /// <summary>
        /// Name Value List
        /// </summary>
        public ObservableCollection<NameValue> NameValueList
        {
            get { return nameValueList; }
        }
        /// <summary>
        /// Result
        /// </summary>
        public string Result
        {
            get { return result; }
            set
            {
                if (result == value) return;
                result = value;
               
            }
        }
        #endregion
      //  private readonly double LearningRate;
        /// <summary>
        /// AverageSpeed
        /// </summary>
        public Dictionary<int, float> AverageSpeed { get; set; } = new Dictionary<int, float>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="learningRate">The learning rate of algorithm. Typically 0.2 - 1.7.</param>
        /// <param name="anotherArg">Bla</param>
        public DecisonForestRegressionAlgorithm(double learningRate, double anotherArg = 2.5)
        {
            //this.LearningRate = learningRate;
            validationDictionary.Add("TrainingSet", ValidateTrainingSet);
            validationDictionary.Add("ExportFolder", ValidateExportFolder);
            validationDictionary.Add("ResolutionFeatureName", ValidateResolutionFeatureName);

        }

        /// <summary>
        /// 1
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public IResult Predict(double[][] data, IContext ctx)
        {
                                 
            #region predict

            NameValueList.Clear();
            //list
            ItemNumericalSet items = ExcelParser.ParseItemNumericalSet("", data);
            
            var results = new DecisonForestRegressionAlgorithmResult()
            {
                Results = new float[items.Count()],
            };
            //list end

            for (int i = 0; i < items.Count(); i++)

            {
                ItemNumerical test = items.GetItem(i);
                results.Results[i] = (float)forest.Resolve(test);
            }

            #endregion
            //results.Results=res
            return results;
        }


        /// <summary>
        /// Run method get data and context
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public IScore Run(double[][] data, IContext ctx)
        {
            float treesResult = 0;
            //List<double[]> Trainingdata = new List<double[]>();
            double[][] Trainingdata = new double[3000][];
            // We want to have 80%  from our data for training.
            int Trainingpercent = 80;
            int total = data.Length;
            int Traininggdata = (total * Trainingpercent) / 100;
            int Testpercent = 20;
            int Testingdata = (total * Testpercent) / 100;
            var TrainingData = data.Skip(Testingdata).Take(Traininggdata).ToArray();

            ResolutionFeatureName = "Z";
            TrainingSubsetCountRatio = 0.6f;
            MaxNumberOfTainingItemsInCategory = 7;
            NumberOfTrees = 30;
            ForestGrowParameters p = new ForestGrowParameters
            {
                ExportDirectoryPath = ExportFolder,
                ExportToJson = false,
                ResolutionFeatureName = ResolutionFeatureName,
                ItemSubsetCountRatio = TrainingSubsetCountRatio,
                TrainingDataPath = TrainingSet,
                MaxItemCountInCategory = MaxNumberOfTainingItemsInCategory,
                TreeCount = NumberOfTrees,
                SplitMode = SplitMode.RSS,
                data = TrainingData,
            };

            if (forest == null)
            {
                forest = ForestFactory.Create();
            
            }

            try
            {

                treesResult = forest.Grow(p);
            }
            catch (Exception)
            {

            }
            
            IScore score = new DecisonForestRegressionAlgorithmScore();
            (score as DecisonForestRegressionAlgorithmScore).ScoreResult = treesResult;
            
            return score;
        }

        /// <summary>
        /// Train method take data and context
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public IScore Train(double[][] data, IContext ctx)
        {
            return Run(data, ctx);
        }

        #region myregion
        private void ForestGrowComplete(object sender, EventArgs e)
        {

            IItemNumerical item = forest.CreateItem();
            var names = forest.GetFeatureNames();
            foreach (var name in names)
            {
                if (name == ResolutionFeatureName)
                    continue;

                NameValueList.Add(new NameValue { Name = name, Value = 0 });

            }
        }

        private void TreeBuildComplete(object sender, EventArgs e)
        {
            float treesCount = forest.TreeCount() * 100f / NumberOfTrees;
          

        }
        #region IDataErrorInfo implementation
        /// <summary>
        /// Error
        /// </summary>
        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// propertyName
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string this[string propertyName]
        {
            get
            {
                string error = validationDictionary[propertyName]();
               
                return error;
            }
        }

        #endregion

        #region Validation

        private string ValidateTrainingSet()
        {
            if (string.IsNullOrWhiteSpace(TrainingSet))
            {
                if (!errorsDictionary.ContainsKey("TrainingSet"))
                    errorsDictionary.Add("TrainingSet", true);
                return "Trainig set path is not defined";
            }

            FileInfo fi = new FileInfo(TrainingSet);
            if (!fi.Exists)
            {
                if (!errorsDictionary.ContainsKey("TrainingSet"))
                    errorsDictionary.Add("TrainingSet", true);
                return "Trainig set path does not exist";
            }

            errorsDictionary.Remove("TrainingSet");
            return string.Empty;
        }

        private string ValidateExportFolder()
        {
            if (string.IsNullOrWhiteSpace(ExportFolder))
            {
                if (!errorsDictionary.ContainsKey("ExportFolder"))
                    errorsDictionary.Add("ExportFolder", true);
                return "Export folder path is not defined";
            }

            DirectoryInfo di = new DirectoryInfo(ExportFolder);
            if (!di.Exists)
            {
                if (!errorsDictionary.ContainsKey("ExportFolder"))
                    errorsDictionary.Add("ExportFolder", true);
                return "Export folder path does not exist";
            }

            errorsDictionary.Remove("ExportFolder");
            return string.Empty;
        }

        private string ValidateResolutionFeatureName()
        {
            if (string.IsNullOrWhiteSpace(ResolutionFeatureName))
            {
                if (!errorsDictionary.ContainsKey("ResolutionFeatureName"))
                    errorsDictionary.Add("ResolutionFeatureName", true);
                return "Resolution feature name is not defined";
            }

            errorsDictionary.Remove("ResolutionFeatureName");
            return string.Empty;
        }


        #endregion

        #endregion


    }
}
