[![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000)](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/master/LICENSE)

# Welcome To LearningApi Tutorial 

# Index 

Part 1  - <a href="#LearningApi_Introduction">LearningApi Introduction</a>

Part 2  - <a href="#Supported_Algorithms&Modules_List">Supported Algorithms and Modules</a>

Part 3  - <a href="#LearningApi_Concept">The LearningApi concept</a>

Part 4a - <a href="#What_is_Pipeline_Module">What is a LearningApi Pipeline Module?</a>

Part 4b - <a href="#Example_Custom_Module">How to build the custom module?</a>

Part 5a - <a href="#What_is_Algorithm">What is a LearningApi Algorithm?</a>

Part 5b - <a href="#Example_Custom_Algorithm">How to build the custom algorithm?</a>

Part 6  - <a href="#Your_Contribution">Contribution to Learning API?</a>


# LearningApi Introduction <a id="LearningApi_Introduction"></a>

_LearningApi_ is a Machine Learning Foundation of a set of ML algorithms implemented in .NET Core/C#. It provides a unique pipeline processing API for Machine Learning solutions. Because it is implemented fully in .NET, developers do not have to bridge .NET and Python or other popular ML frameworks. It has been developed in cooperation with daenet GmbH and Frankfurt University of Applied Sciences.

<!--
![Image 1](https://user-images.githubusercontent.com/44580961/98464210-a5dc1200-21c1-11eb-95ef-e1a0d7942382.png)--> 

<!-- Fig. 1 : daenet GmbH and Frankfurt University of Applied Sciences --> 

LearningAPI already has interfaces that are pre declared which we can easily access, understand and use in our project.

Before you start with the *LearningAPI*, you should get familiar with several interfaces: IPipeline Module, IAlgorithm, IScore, IResult. These interfaces are shared across all algorithms inside of the *LearningAPI*.

LearningAPI is a foundation of Machine Learning algorithms, which can run in the pipeline of modules compatible to each other. This concept allows using of conceptually different algorithms in the same API, which consists of a chain of modules. Typically in Machine Learning applications, developers need to combine multiple algorithms or tasks to achieve a final task or result.

For example, imagine you want to train a supervised algorithm from historical power consumption data to be able to predict the power consumtion. The training data is stored in a csv file which can be read into the program for training the model. This csv file needs to be described in DataDescriptor interface. It contains features like outside temperature, the wind and power consumtion (where power consumption is the output prediction value which is called as label). To solve the problem, you first have to read the data from CSV, then to normalize features and then to train the algorithm.

You could think about these tasks as follows:

1. Read CSV
2. Normalize the data
3. Train the data - After the above 3 processes, you will have a trained instance of the algorithm *algInst*, which can be used for prediction,
4. Use *algInst* to predict the power consumption based on the given temperature and wind.

To solve this problem with the *LearningAPI* the following pseudo can be used:

```
var api = new LearningApi(config)
api.UseCsvDataProvider('csvFileName.csv')
api.UseNormilizerModule();
api.Train();
// Prediction
var predictedPower = api.Predict(108W, 45 wind force);
```

We can implement the solution for the above discussed model using _pipeline_ method. A pipeline is defined as the list of pipeline modules. One pipeline module is defined as implementation of interface _IPipeline_.

The IPipeline Interface is defined as follows:

```csharp
 public interface IPipelineModule
 {
 }
 public interface IPipelineModule<TIN, TOUT> : IPipelineModule
 {
        TOUT Run(TIN data, IContext ctx);
 }
```

With this definition the developer can run the implementation of the module with the following code:

```csharp
using LearningFoundation;
public interface IAlgorithm : IPipelineModule<double[][], IScore>, IPipelineModule
{
	IScore Train(double[][] data, IContext ctx)
  	IResult Predict(double[][] data, IContext ctx)
}
```

To define the pipline of modules you woudld typically do describe the following modules differnetly in the same file and call them wherever needed to do the respective functions:

api.UseDataProviderModule(“DataProvider”, DataProviderModule)
api.UseDataNormalizerModule(“DataNormalizer”, DataNormalizerModule);
 
To make the code more readable, developers of modules typically provide helper extension methods using the following ,	
 
api.UseDataProvider(args1)
api.UseDataNormalizer(args2);

-------------------------------

A real time example model is explained in the below section -
<a href="#Example_Custom_Algorithm">'Please click here to understand 'How to build a LearningAPI algorithm?'</a>

-------------------------------

# Supported Algorithms and Modules <a id="Supported_Algorithms&Modules_List"></a>

All the supported Modules and Algorithms are listed in an excel sheet. Also, the information about the documentation and coding source files availabiliy in the LearningApi repository can be found here.

[Click here to find the list in Git Repository..](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/master/LearningApi/src/AlgorithmsModules%20_list_Final.xlsx)

## Machine Learning Algorithms

| Algorithm | LearningApi Repository | .md file available | Documentation available? |
|:--- |:--- |:--- |:--- |
| RingProjection | [Github_RingProjection Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/RingProjection) | Available | [Github_RingProjection Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/RingProjection/Documentation) |
| SVM - SupportVectorMachine | [Github_SVM Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/SupportVectorMachine)    | Available | [Github_SVM Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/SupportVectorMachine/Documentation) |
| Scalar encoder - ScalarEncoder in HTM  | [Github_ScalarEncoder Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/ScalarEncoder%20in%20HTM) | Not Available yet | [Github_ScalarEncoder Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/ScalarEncoder%20in%20HTM/Documentation) |
| Anamoly latest - AnomDetectLatest | [Github_Anamoly Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/AnomDetectLatest) | Available | [Github_Anamoly Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/AnomDetectLatest/Documentation) |
| Delta Learning | [Github_DeltaLearning Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/DeltaRuleLearning) | Available | [Github_DeltaLearning Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/DeltaRuleLearning/Documentation) |
| GaussianMean Filter | [Github_GaussianMeanFilter Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/GaussianMeanFilter) | Available | [Github_GaussianMeanFilter Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/GaussianMeanFilter/Documentation) |
| Image Edge detection    | [Github_ImageEdgeDetection Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/ImageDetection) | Available  | [Github_ImageEdgeDetection Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/ImageDetection/Documentation) |
| Logistic Regression | [Github_Logistic Regression Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/LogisticRegression) | Available | [Github_LogisticRegression Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/LogisticRegression/Documentation) |
| Neural Network Perceptron | [Github_NeuralNetworkPerceptron Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/NeuralNetworks) | Not Available yet | [Github_NeuralNetworkPerceptron Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/NeuralNetworks/NeuralNet.MLPerceptron/Documentation)    |
| Self Organizing Map | [Github_SelfOrganizingMap Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/SelfOrganizingMap) | Available | [Github_SelfOrganizingMap Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/SelfOrganizingMap/Documentation) |
| Survival Analysis | [Github_SurvivalAnalysis Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/MLAlgorithms/SurvivalAnalysis) | Not Available yet | Not Available yet |

## Data Modules

| Modules | LearningApi Repository | .md file available | Documentation available? |
|:--- |:--- |:--- |:--- |
| Image binarizer Latest | [Github_ImageBinarizer Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Modules/ImageBinarizerLatest) |  Available | [Github_ImageBinarizer Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Modules/ImageBinarizerLatest/Documentation) |
| Euclidian color filter | [Github_EuclidianColorFilter Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Modules/EuclideanColorFilter) | Available | [Github_EuclidianColorFilter Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Modules/EuclideanColorFilter/Documentation) |
| Image Binarizer | [Github_ImageBinarizer Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Modules/ImageBinarizer) | Available  | [Github_ImageBinarizer Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Modules/ImageBinarizer/Documentation) |
| Center Module | [Github_CenterModule Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Modules/Center%20Module) | Available | [Github_CenterModule Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Modules/Center%20Module/Documentation) |
| Canny edge detector | [Github_CannyEdgeDetector Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Modules/CannyEdgeDetector) | Available    | Not Available yet |
| SDR Classifier | [Github_SDR Classifier Algorithm](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Modules/SDR%20Classifier) | Available | [Github_SDR Classifier Documentation](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Modules/SDR%20Classifier/Documentation) |

# The LearningApi Concept <a id="LearningApi_Concept"></a>

LearningAPI is a framework for developing software machine learning applications. This includes predefined classes and functions that can be used to process input, train the system and give an accurate predicted answer.

  In order to use LearningApi, we should install the Nuget package called **_LearningApi_** into our project (this will be demonstarted in <a href="#Example_Custom_Algorithm">Example custom algorithm section</a>)
  
  Basically a NuGet package is a single ZIP file with the *.nupkg* extension that contains compiled code (DLLs), other files related to that code, and a descriptive manifest that includes information like the package's version number.
  
  Initially open the class ‘.cs’ and implement the IAlgorithm in the code which is taken from LearningApi NuGet package. IAlgorithm is in the library and it has a separate structure which we have to use in the project. 
  
More information can be found on [Click here for more information on NuGet packages..](https://docs.microsoft.com/en-us/visualstudio/mac/nuget-walkthrough?view=vsmac-2019)

<!--To find out more details, click on [Information..](https://docs.microsoft.com/en-us/nuget/what-is-nuget)-->

  **Inputs** to the TRAIN i.e to the algorithm is the set of data with expected outputs for few number of inputs, we train the system and then expect the predicted value to be accurate when other input is given.
  
  **Output** is the predicted value from PREDICT method which gives the accuracy of the True or False statements.
  
  For example, if we take HOUSE PRICE prediction scenario (Explained in Linear Regression using LeraningApi section), the features SIZE, ROOM and PRICE  are the real time _input data_ given to the model to get trained based on these existing data. Whereas , PRICE is the predicted value which is expected to be the output of the model based on the training given to the model. 

**IAlgorithm** - The _IAlgorithm_ interface has 2 phases:

1. _**IResult**_ – IResult is used to set and get the final result of the algorithm and store it. We use IResult interface for the PREDICT phase - This is the final phase where we get the accurate predicted output for the input provided by the user on the basis of the trained model. IResult is returned by PREDICT. 

2. _**IScore**_ – Iscore is used to set and get the values of all the features used in the project (which ar given in csv file/input data). IScore is returned by RUN / TRAIN methods.

**RUN/TRAIN** – This is the training (learning) part where the random/real time data will be given to our system to test whether the correct output is being displayed after the training phase. The description of features, data inputs, logic for learning are defined in this interface. Here we will train the system with our specific set of data i.e input and the output as in how to function/ predict the output with higher accuracy.

**PREDICT** –  This is the prediction part where the model has the trained logic as input and gives the high accuracy prediction for the label we described to be as output. 
  
**The Pipeline module** receives an input TIN and context information. Usually TIN is set of data, which results as output of th eprevious module. Typically, first module in the pipeline is responsibe to provide learning data and last module in the pipeline is usually algorithm.

# What is a LearningApi Pipeline Module? <a id="What_is_Pipeline_Module"></a>

A module in Machine Learning represents a set of code that can run independently and perform a machine learning task, given the required inputs. A module might contain a particular algorithm, or perform a task that is important in machine learning, such as missing value replacement, or statistical analysis.
Both algorithms and modules are independent of each other. 

While implementing an algorithm, it is initially trained using various number of data available already to make the algorithm learn how to predict the results for an unknown input in the later stages. Thus the set of data is very important. This data is supposed to be clean with all details. Sometimes in algorithms when we don't get clean data, pipeline modules are used for pre-processing of the data. 

For example some pipeline modules as MinMaxNormalisers have the function of normalising the data for the larger algorithms.

Following example illustrates how to setup the learning pipeline modules:

```csharp
public void SimpleSequenceTest()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 10000;
                ctx.DataDescriptor = getDescriptor();
                double[][] data = new double[maxSamples][];

                //
                // We generate following input vectors: 
                // IN Val - Expected OUT Val 
                // 1 - 0
                // 2 - 0,
                // ...
                // maxSamples / 2     - 1,
                // maxSamples / 2 + 1 - 1,
                // maxSamples / 2 + 2 - 1,


                for (int i = 0; i < maxSamples; i++)
                {
                    data[i] = new double[2];
                    data[i][0] = i;
                    data[i][1] = (i > (maxSamples / 2)) ? 1 : 0;
                }

                return data;
            });

            api.UsePerceptron(0.02, 10000);

            IScore score = api.Run() as IScore;

            double[][] testData = new double[4][];
            testData[0] = new double[] { 2.0, 0.0 };
            testData[1] = new double[] { 2000.0, 0.0 };
            testData[2] = new double[] { 6000.0, 0.0 };
            testData[3] = new double[] { 5001, 0.0 };

            var result = api.Algorithm.Predict(testData, api.Context) as PerceptronResult;

            Assert.True(result.PredictedValues[0] == 0);
            Assert.True(result.PredictedValues[1] == 0);
            Assert.True(result.PredictedValues[2] == 1);
            Assert.True(result.PredictedValues[3] == 1);
        }
        
         private DataDescriptor getDescriptor()
        {
            DataDescriptor desc = new DataDescriptor();
            desc.Features = new LearningFoundation.DataMappers.Column[1];
            desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 0,
                Name = "X",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.LabelIndex = 1;

            return desc;
        }
```
The code shown above sets up the pipeline for two modules. 

1.First one is so called _Action Module_, which defines the custom code (to generate the vector functions) to be executed in the program to achieve . 
```csharp
api.UseActionModule<object, double[][]>((notUsed, ctx)
```

2.Second module injects the perceptron algorithm in the pipeline and it is setup by the following line of code:

```csharp
api.UsePerceptron(0.02, 10000);
```

3.Execution of the pipeline is started with following line of code:

```csharp
IScore score = api.Run() as IScore;
```
4.DataDescriptor part is to define the input data we use.The below lines of code guides the program what input should be considered. 

```csharp
desc.Features[0] = new LearningFoundation.DataMappers.Column()
            {
                Id = 0,
                Name = "X",
                Type = LearningFoundation.DataMappers.ColumnType.NUMERIC,
                Index = 0,
            };

            desc.LabelIndex = 1;
```

When the pipeline starts, modules are executed in the sequence ordered as they are added to the pipeline. 
In this case, first 'Action Module' will be executed and then 'Perceptron' algorithm. After running of the pipeline modules, model is trained. Next common step in Machine Learning applications is called evaluation of the model. Following code in previous example shows how to evaluate (predict) the model:

```csharp
double[][] testData = new double[4][];
            testData[0] = new double[] { 2.0, 0.0 };
            testData[1] = new double[] { 2000.0, 0.0 };
            testData[2] = new double[] { 6000.0, 0.0 };
            testData[3] = new double[] { 5001, 0.0 };

        var result = api.Algorithm.Predict(testData, api.Context) as PerceptronResult;

            Assert.True(result.PredictedValues[0] == 0);
            Assert.True(result.PredictedValues[1] == 0);
            Assert.True(result.PredictedValues[2] == 1);
            Assert.True(result.PredictedValues[3] == 1);
```

# How to build the custom module? <a id="Example_Custom_Module"></a>

The below solution demonstrates how to implement a custom pipeline module. In this example, convolution logic will be impemented.
  
This example is only for reference on steps to implement a solution using LearningApi. 

<a href="https://anushaashokreddy.github.io/trial.github.io/Pipeline_Module/"><span class="button">Example Pipeline Module using LearningApi</span></a>

-------------------------------------------------------------------------------------------------------------------------------------

# What is a LearningApi Algorithm? <a id="What_is_Algorithm"></a>

Machine learning is a class of methods for automatically creating models from data. Machine learning algorithms are the engines of machine learning, meaning it is the algorithms that turn a data set into a model. Which kind of algorithm works best (supervised, unsupervised, classification, regression, etc.) depends on the kind of problem you’re solving, the computing resources available, and the nature of the data.

An algorithm is a set of logical coding which is trained with lots and lots of data to predict the otput most accurately.

# How to build the custom algorithm? <a id="Example_Custom_Algorithm"></a>

  The below solution demonstrates how to implement a simple model SUM calculation using LearningApi. To understand the implementation, you should initially understand the logic we are using. 
	
To train a model to learn predicting the calculation of POWER consumption by doing SUM calculation of 2 variables like SOLAR and WIND, we need to train the model with few real example data and then train it to learn the logic in order to give the accurate output. 

Below is the explanation of implementation of LearningApi concept for building the SUM model as explained earlier. 

<a href="https://anushaashokreddy.github.io/trial.github.io/Sum_Algorithm_Example"><span class="button">Example SUM Algorithm using LearningApi</span></a>

We have also given the similar guidance for an actual Machine Learning Algorithm LINEAR REGRESSION as a complex example to learn the process more deeper in the below link. 

[Click here to find the complex example description..](https://anushaashokreddy.github.io/trial.github.io/Linear_Regression_Example)

<!--
<a href="https://anushaashokreddy.github.io/trial.github.io/Linear_Regression_Example"><span class="button">Example Linear Regression using LearningApi</span></a>
-->
-------------------------------------------------------------------------------------------------------------


# How can you contribute to LearningApi? <a id="Your_Contribution"></a>

If you have implemented a custom module or algorithm and want to integrate it to LearningAPI, then you can do the following, 

<a href="https://anushaashokreddy.github.io/trial.github.io/ContactPage"><span class="button">Contact Us</span></a>

<a href="https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/issues/new"><span class="button">Create an issue in the Repository</span></a>


<!--
<a href="https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src"><span class="button">Implement your algorithm or/and module</span></a>-->

<!--<a href="https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src"><span class="button">Create the pull request</span></a> 
-->




