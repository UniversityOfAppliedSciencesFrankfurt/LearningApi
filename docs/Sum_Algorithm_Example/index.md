## Example Algorithm using LearningApi <a id="#Example_Algoirthm"></a>

Let's take a simple example to build a model to predict the power consumption using the given set of data. For this, we have 3 phases of tasks to perform. 

1. Loading data phase
2. Training Phase
3. Prediction Phase 

**Loading data phase :** 

Let's consider 2 variables which are refered as _features_ in ML (features are the inputs)
	
	*SOLAR* - Solar data in kWh unit
	
	*WIND* - Wind data in knot unit
	
and 1 _label_ (the output feature is called label)

	*POWER* - Power consumption data 

Each features is given with an index number defined in DataDescriptor as shown in the following line of code :

```csharp
	des.Features[0] = new Column { Id = 1, Name = "solar", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
        des.Features[1] = new Column { Id = 2, Name = "wind", Index = 1, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
        des.Features[2] = new Column { Id = 3, Name = "power", Index = 2, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
```
	
Now we should train the model with the above 2 features and a label by giving a real data (shown below) in csv file, we have taken the following data :

| solar | wind | power | 
|:--- |:--- |:--- |
| 10 | 5 | 15 |
| 20 | 8 | 28.4 |
| 25 | 2 | 27 |
| 34 | 10 | 44 |
| 40 | 20 | 60 |
| 42 | 2 | 44 |
| 45 | 40 | 85 |
| 48 | 2 | 50 |
| 50 | 20 | 70 |
| 55 | 25 | 80 |

The model uses CsvDataProvider to read the csv file as shown in the following interface :

```csharp
 api.UseCsvDataProvider(trainDataPath, ',', false);
```

The module can be configured by using a DataDescriptor class. This class describes the columns (features and the label). Label index is the index number of feature for which the prediction should be made. 

The following is the code snippet for the data descriptor :

```csharp
 private DataDescriptor LoadMetaData()
        {
            var des = new DataDescriptor();

            des.Features = new Column[3];
	    
	    //.....(DESCRIBE THE COLUMNS HERE)

            des.LabelIndex = 2;
            return des;
        }
```

**Training phase :** The model reads the data from _csv_ file and based on the data given, after training, the model should be able to predict the result y. 

By seeing the data set given, we can easily recognise that power consumption is the result of summing up solar and wind data. The simple logic we are using here is SUM : 

The expression for the problem can be identified as: 

	POWER = SOLAR + WIND 

For this basic example, learning is not required as the data is straight forward. Model predicts the power consumption value using the data and most of the time loss we get would be zero. 

**1st data : solar 10 and wind value 5 **

	substitute these values in formula --   y = solar + wind
					       y1 = 10 + 5 = 15	      
	
	Square Error -- SE1 = (actual power value for above solar and wind values - predicted price for above solar and wind values)^2 
	
			SE1 = (15 - 15)^2 = 0^2 = 0 
			
But in some cases, there might be outliers like shown below :
			
**2nd data : solar 20 and wind value 8 **

	substitute these values in formula --   y = solar + wind
					       y2 = 20 + 8 = 28
	
	Square Error -- SE2 = (actual power value for above solar and wind values - predicted price for above solar and wind values)^2
	
			SE2 = (28.4 - 28)^2 = 0.4^2 = 0.16
			
The above calculation occurs for all the values of solar and wind data set.

We use *Mean Square Error* concept to find out the error differences and help the model to finalise the least error value to be more accurate for the prediction. For each data set MSE is calculated.

Mean square error calculation (MSE1)-- 

    	     = (SE1+SE2+....)/ (Total number of data)
    
	MSE1 = x

The following code snippet shows the calculating logic in the program :
```csharp
for (int trainDataIndex = 0; trainDataIndex < numTrainData; trainDataIndex++)
            {
                estimatedOutputLabels[trainDataIndex] = ComputeSum(inputFeatures[trainDataIndex]);
                squareErrors[trainDataIndex] = ComputeSquareError(actualOutputLabels[trainDataIndex], estimatedOutputLabels[trainDataIndex]);
            }

            double meanSquareError = squareErrors.Sum() / numTrainData;

            loss = meanSquareError;

            if (ctx.Score as SumScore == null)
                ctx.Score = new SumScore();

            SumScore scr = ctx.Score as SumScore;
            scr.Loss = loss;

            return ctx.Score;
```

While the loss is very less, model gives the accurate power consumption value for the given set of solar and wind data for test. 
	
**Prediction Phase :** Now, the model has an exact equation (found out as the output of training with least loss).  This formula/logic will be used by the model to predict power consumption with any further solar and wind combinations.

## Implementation of LearningApi for the above example of SUM Algorithm :

This topic provides a deep knowledge on implementation of LearningApi for the algorithm we discussed above. Here main focus is to learn where and how we implement the LerningApi interfaces in the project and not the algorithm itself.

### Step 1: Create a solution 

In the Visual Studio, create a new solution by following the steps -
	
    Navigate to File --> New --> Project

Use the selectors on the left side to choose the different types of programming languages or platforms to work with. For example, we are creating a class library with the template .NET STANDARD under the Visual C# selector as show in Fig. 4.

    Click on NEXT 	

<!--![Image 4](https://user-images.githubusercontent.com/44580961/98464414-04ee5680-21c3-11eb-82fe-910a29ed7d4d.png) -->

<img src="https://user-images.githubusercontent.com/44580961/98464414-04ee5680-21c3-11eb-82fe-910a29ed7d4d.png" width="600" height="450" />

Fig. 4 : New Project

For our example - given the project name as **“SumAlgorithm”**	

    Name the project --> Solution Name --> Specify the location --> Click OK/CREATE
    
<!--![Image 5]() -->

<img src="https://user-images.githubusercontent.com/44580961/100554990-cbd76c80-32be-11eb-8f0e-2b203e669ea0.png" width="600" height="450" />

Fig. 5 : Project and Solution name

Now the project is created with the name _'SumAlgorithm.sln'_
  
<!--![Image 6] -->

<img src="(https://user-images.githubusercontent.com/44580961/100554992-d09c2080-32be-11eb-8a96-c717eabe077b.png)" width="450" height="300" />

Fig. 6 : Creation of Solution	
	
### Step 2: Create the class library for the algorithm 
	
When solution (SumAlgorithm.sln) is created, by default a class library is also created automatically (.cs file).

We have to change the names accordingly. Here for example, change the class library name as **_“Sum.cs”_** as shown in Fig. 6.

_Sum.cs_ serves as the main class folder for the algorithm.

<!--![Image 7]-->

<img src="(https://user-images.githubusercontent.com/44580961/100554994-d265e400-32be-11eb-95b8-dbeaa8f5c69a.png)" width="450" height="300" />

Fig. 7 : The project and class library folder structure

### Step 3: Create the Test folder and Test class library for the algorithm 

We should create a Test folder where we can initiate the program and command the directions. 

	Select the project folder --> Right click --> Add --> New Project

![Image 8](https://user-images.githubusercontent.com/44580961/100554995-d560d480-32be-11eb-8735-1cece51fb26a.png) 

Select the Test file _'MSTest project c#'_ and click on NEXT button as shown in the below Fig. 9.

![Image 9](https://user-images.githubusercontent.com/44580961/100554997-d72a9800-32be-11eb-9991-21e44062166f.png) 

Name the project name as _**SumAlgorithmTest**_ and click on NEXT button. 

![Image 10](https://user-images.githubusercontent.com/44580961/100554998-d98cf200-32be-11eb-9f28-8f19072c1add.png) 

Test project is created under the main solution and rename the class file as _**SumAlgorithmTest1**_ as shown in the below Fig. 11.

<!--![Image 11] -->

<img src="(https://user-images.githubusercontent.com/44580961/100555000-dd207900-32be-11eb-98c7-0db218463908.png)" width="450" height="300" />


### Step 4 : Add NuGet Package 'LearningApi' to both projects 

We should add NuGet package called _LearningApi_ to both project by following the steps below, 

		
	Right click on project (HelloLearningApiExampleAlgorithm/HelloLearningApiExampleAlgorithmTest) --> Click on ‘Manage NuGet packages..’ (Fig. 12)	

	in the pop up window --> Click on BROWSE, (Fig. 13)
	
	search for LearningApi and select --> Select the checkbox of LearningApi nuget --> Click on SELECT/ADD PACKAGE button (Fig. 14)

	
<!--![Image 12]()-->

<img src="https://user-images.githubusercontent.com/44580961/100555001-de51a600-32be-11eb-96a8-6d4b0a446a0b" width="400" height="550" />

Fig. 12 : NuGet package integration step1,

In the pop up, search for the package LearningAPI , select the latest version and click on ADD PACKAGE button.

<!--![Image 13]()-->

<img src="https://user-images.githubusercontent.com/44580961/100555004-e0b40000-32be-11eb-9aef-245dc985f877.png" width="800" height="450" />

Fig. 13 : NuGet package integration step2,  

A pop up with the packages installed along with the LearningApi NuGet package is displayed. Click on OK/Accept button.

### Step 5 : Start the Code for the project and test .cs files  

<a href="#Example_Algoirthm">Click here to recap the LearningApi Example Algorithm</a>

**In Test.cs file** , we direct the model to read the csv file and take the data for training of the model. We also provide data mapper to extract the data from the columns with the following code :

```csharp
api.UseCsvDataProvider(trainDataPath, ',', false);
api.UseDefaultDataMapper();
```
csv data file path is recognised by the line of code :

```csharp
string trainDataPathString = @"SampleData\power_consumption_train.csv";
```
load the meta data (where the features are explained) by the following bit of code:

```csharp
private DataDescriptor LoadMetaData()
        {
            var des = new DataDescriptor();

            des.Features = new Column[3];
            des.Features[0] = new Column { Id = 1, Name = "solar", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "wind", Index = 1, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[2] = new Column { Id = 3, Name = "power", Index = 2, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };

            des.LabelIndex = 2;
            return des;
        }
```

In **Algorithm.cs** file , we implement the *IAlgorithm* in the code which is taken from LearningApi NuGet package. *IAlgorithm*  is in the library and it has a separate structure which we have to use in the project as we already have discussed in the section <a href="#LearningApi_Concept">LearningApi Concept</a>. 

![Image 14](https://user-images.githubusercontent.com/44580961/100555006-e3aef080-32be-11eb-9da6-41d0a5089027.png)

Fig. 14 : IAlgorithm interface integrated in the project

Here, In _'IScore Run'_ method, we direct the model to TRAIN (IScore Train) interface where the logic for SUM algorithm is defined. 

The following code is used for training the model :

```csharp
public IScore Train(double[][] data, IContext ctx)
        {
            for (int trainDataIndex = 0; trainDataIndex < numTrainData; trainDataIndex++)
            {
                estimatedOutputLabels[trainDataIndex] = ComputeSum(inputFeatures[trainDataIndex]);
                squareErrors[trainDataIndex] = ComputeSquareError(actualOutputLabels[trainDataIndex], estimatedOutputLabels[trainDataIndex]);
            }

            double meanSquareError = squareErrors.Sum() / numTrainData;

            loss = meanSquareError;

            if (ctx.Score as SumScore == null)
                ctx.Score = new SumScore();

            SumScore scr = ctx.Score as SumScore;
            scr.Loss = loss;

            return ctx.Score;
        }
```
In PREDICT interface, all the logics for computing mean square error is provided as shown in the below code lines. 

```csharp
public IResult Predict(double[][] data, IContext ctx)
        {
            var testData = data;

            int numTestData = testData.Length;

            int numFeatures = ctx.DataDescriptor.Features.Length - 1;

            double[][] inputFeatures = GetInputFeaturesFromData(testData, numFeatures);

            double[] predictedOutputLabels = new double[numTestData];

            for (int testDataIndex = 0; testDataIndex < numTestData; testDataIndex++)
            {
                predictedOutputLabels[testDataIndex] = ComputeSum(inputFeatures[testDataIndex]);
            }

            SumResult res = new SumResult();
            res.PredictedValues = predictedOutputLabels;

            return res;
        }
````

### Step 6 : Create the *Extension.cs* , *Result.cs* and *Score.cs* files

Extension file in a project facilitates other users to utilise our project code in their implementations. Calling this file in other projects enables the project code in other projects.
      
      Right Click on Project name --> Add --> New Class (Fig. 12_left side)
      
      Select Empty class --> Give the class name 'SumExtension' --> Click on NEW button (Fig. right side)

![Image 15](https://user-images.githubusercontent.com/44580961/100555008-e578b400-32be-11eb-99cf-ea1176ff7269.png)

Fig. 15 : Adding Extension class to ALgorithm project 

The following is given as code for extension.cs file in order to use it anywhere in the project further:

```csharp
public static LearningApi UseSum(this LearningApi api)
        {
            var alg = new Sum();
            api.AddModule(alg, "Sum");
            return api;
        }
```

Likewise, in the example solution, the *SumResult.cs* and *SumScore.cs* files should be created to define the values which should be storing the result and trained score data. Follow the steps explained above in Fig.12 to create these classes also.

The values are get and set in the _Result.cs_ file with the following code line :

```csharp
public class SumResult : IResult
    {
        public double[] PredictedValues { get; set; }
    }
```
The values for the Loss are get and set in the _Score.cs_ file with the following lines of code :

```csharp
 public class SumScore : IScore
    {
        public double Loss { get; set; }
    }
 ```

### Step 7 : Result 

According to the algorithm, the set of data of house details is given and trained the model with these data. The data for house price is used to calculate the mean square error and When this score is multiplied with each data given, we get the house price value predicted.

![Image 16]()

Fig. 16 : Result is shown here

## Basic Example SUM algorithm code in Github

You can refer this example project by [Clicking here to refer the SUM algorithm project code in GitHub..](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Individual%20project_AnushaAshokReddy/SumAlgorithm)
