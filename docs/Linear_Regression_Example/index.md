# How to build the Linear Regression algorithm using LearningAPI? 

  The below solution demonstrates how to implement a Linear Regression algorithm for a model to predict House price using LearningAPI. To understand the implementation, you should initially understand the linear Regression concept. 
	
Simple linear regression is a statistical method that allows us to summarize and study relationships between two continuous and quantitative variables. And in this one variable will be assigned for prediction based on the changes and variations in the other variables.

## LearningApi Example Algorithm <a id="#Example_Algoirthm"></a>

Lets take a simple example to build a model to predict the HOUSE PRICE. For this, we have 3 phases of tasks to do. 

1. Loading data phase
2. Training Phase
3. Prediction Phase 

**Loading data phase :** 

Let's consider 2 simple _features_ (features are the inputs)
	
	*Size* - Size of the house
	
	*Room* - Number of rooms
	
and 1 _label_ (the output feature is called label)

	*Price* - Price of the house based on *room* feature

Each features is given with a index number defined in DataDescriptor as shown in the following line of code :
```csharp
	des.Features[0] = new Column { Id = 1, Name = "size", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
        des.Features[1] = new Column { Id = 2, Name = "rooms", Index = 1, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
        des.Features[2] = new Column { Id = 3, Name = "price", Index = 2, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
```
	
Now we should train the model with the above 2 features and a label by giving a real data (shown below) in csv file, we have taken the pre processed data :

Ex. Size data is taken as 1 instead of 10 sq.m.
    Room data is taken as it is.
    Price data is taken as 6 instead of 600 euros.

| Size | Room | Price | 
|:--- |:--- |:--- |
| 1 | 1 | 6 |
| 1 | 2 | 8 |
| 2 | 1 | 9 |
| 2 | 2 | 11 |
| 3 | 1 | 12 |
| 3 | 2 | 14 |
| 4 | 1 | 15 |
| 4 | 2 | 17 |
| 5 | 1 | 18 |
| 5 | 2 | 20 |

The model uses CsvDataProvider to read the csv file as shown in the following interface :

```csharp
 api.UseCsvDataProvider(trainDataPath, ',', false);
```

The module can be configured by using a DataDescriptor class. This class describes the columns (features and the label). Label index is the index number of feature based on which the prediction should be made. 

The following is the code snippet for the data descriptor :

```csharp
 private DataDescriptor LoadMetaData()
        {
            var des = new DataDescriptor();

            des.Features = new Column[3];

            des.LabelIndex = 2;
            return des;
        }
```

**Training phase :** The model reads the data from _csv_ file and based on the data given, after training the model, the model should be able to predict the result y. 

The linear regression representation for the above situation corresponds to the following equation which is in the form of linear equation 

	y = (w1 * x1) + (w2 * x2) + b , 

	PRICE = (3 * SIZE) + (2 * ROOM) + 1  

	where 

      y  is the predicted price of the house
      x1 is the Size of house 
      x2 is number of rooms in the house
      w1 or 3 is the weight (w1) for feature PRICE     
      w2 or 2 is the weight (w2) for feature ROOM  
      b or 1 is the bias 
      
Here w and b are weight and bias -  To understand this, imagine, suppose the model has to predict the price of house , so it will start guessing the values of w and b , based on these values , it predicts the price (y), we compare this predicted price with the original data value and give the error. Error is found for each data set in the csv file. 

we use *Mean Square Error* concept to find out the error differences and help the model to finalise the least error value to be more accurate for the prediction. For each data set MSE is calculated.

Initially let's initialise the weight and bias (let this be w1=0 , w2=0 and b=0). This w and b are used for calculating the prices across all the data of size given :

**1st data : w1=0 , w2=0 and b=1, size 10sq.m**

	substitute these values in formula --   y = (w1 * x1) + (w2 * x2) + b
					      y1^ = (0*1) + (0*1) + 1 = 1
	
	Square Error -- SE1 = (actual price for size 1 - predicted price for size 1)^2 
	
			SE1 = (6 - 1)^2 = 5^2 = 25
			
**2nd data : w=2 and b=1, size 20**

	substitute these values in formula --   y = (w1 * x1) + (w2 * x2) + b
					      y1^ = (0*1) + (0*2) + 1 = 1
	
	Square Error -- SE2 = (actual price for size 1 - predicted price for size 1)^2 
	
			SE2 = (8 - 1)^2 = 7^2 = 49
			
The above calculation occurs for all the values of size and room data sets with the initialised w and b values.

Mean square error calculation (MSE1)-- 

    	     = (SE1+SE2+....)/ (Total number of data)
    
    	     = (25 + 49 +....)/10
    
	MSE1 = x

The following code snippet shows the calculating logic in the program :
```csharp
for (int trainDataIndex = 0; trainDataIndex < numTrainData; trainDataIndex++)
                {
                    estimatedOutputLabels[trainDataIndex] = ComputeOutput(inputFeatures[trainDataIndex], weights, bias);
                    squareErrors[trainDataIndex] = ComputeSquareError(actualOutputLabels[trainDataIndex], estimatedOutputLabels[trainDataIndex]);
                }

                double meanSquareError = squareErrors.Sum() / numTrainData;

                loss[epoch] = meanSquareError;
```

Likewise this will continue with other weights and biases which are assumed as explained below. 

After calculating the MSE , we should minimize this error or loss by *Gradient Descent* method. 
 
**W and b values updation**

New weights and biases will be calculated by 

- finding the least w and b values by Gradient Descent concept
- We calculate partial derivative of loss with respect to w (dw) and partial derivative of loss with respect to b (db)
- setting up the Leanring rate (LR)

Hence new w and new b values would be :

	w1(new) = w1(old) - (d(w1) * LR)
	
	w2(new) = w2(old) - (d(w2) * LR)
	
	b(new) = b(old) - (d(b) * LR)

```csharp
Tuple<double[], double> hyperParameters = GradientDescent(actualOutputLabels, estimatedOutputLabels, inputFeatures, numTrainData, numFeatures);

                // Partial derivatives of loss with respect to weights
                double[] dWeights = hyperParameters.Item1;

                // Updating weights
                for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                {
                    weights[featureIndex] = weights[featureIndex] - m_LearningRate * dWeights[featureIndex];
                }

                // Partial derivative of loss with respect to bias
                double dbias = hyperParameters.Item2;

                // Updating bias
                bias = bias - m_LearningRate * dbias;
            }
```

Likewise the model will do these calculations by assuming several weights and biases.

So, the process from initialising of w and b till finding the W(new) and b(new) values is called 1 epoch. 

In the coding, we initialise the number of epochs and Learning Rate for training the model. 

The following lines of code initialises LR and epochs at the starting of the program - 

```csharp
	private double m_LearningRate;
        private int m_Epochs;

        public HelloLearningApiExampleAlgorithm (double learningRate, int epochs)
        {
            m_LearningRate = learningRate;
            m_Epochs = epochs;
        }
```

Now in 1st epoch, MSE1 is some x value, likewise after 1000 epochs, MSE1000 would be (lets assume) some 0.001. Therefore the mean square error is completely reduced which means the error is less and the w1, w2 and b values of this least MSE is the right value for the final prediction. 

Assume, for the correct epoch (1000th epoch), model has taken the values of W1=2.98, W2=2.02 and B=1.02 (near to the value W1=3, W2=2 and B=1 as shown in the initial equation of the logic).

	PRICE = 2.98 * SIZE + 2.02 * ROOM + 1.02
	
**Prediction Phase :** Now, the model has an exact equation (found out as the output of training).  In the last epoch, the hyper parameters values will be used for prediction. This formula will be used by the model to predict price of the house with any further sizes and room combinations of the house.

## Implementation of LearningApi for the above example Algorithm :

This topic provides a deep knowledge on implementation of LearningApi for the algorithm we discussed above. Here main focus is to learn where and how we implement the LerningApi interfaces in the project and not the algorithm itself.

### Step 1: Create a solution 

In the Visual Studio, create a new solution by following the steps -
	
    Navigate to File --> New --> Project

Use the selectors on the left side to choose the different types of programming languages or platforms to work with. For example, we are creating a class library with the template .NET STANDARD under the Visual C# selector as show in Fig. 4.

    Click on NEXT 	

<!--![Image 4](https://user-images.githubusercontent.com/44580961/98464414-04ee5680-21c3-11eb-82fe-910a29ed7d4d.png) -->

<img src="https://user-images.githubusercontent.com/44580961/98464414-04ee5680-21c3-11eb-82fe-910a29ed7d4d.png" width="600" height="450" />

Fig. 4 : New Project

For our example - given the project name as **“HelloLearningApiExampleAlgorithm”**	

    Name the project --> Solution Name --> Specify the location --> Click OK/CREATE
    
<!--![Image 5]() -->

<img src="https://user-images.githubusercontent.com/44580961/99399484-bac84c00-290b-11eb-93b1-504faf36eec1.png" width="600" height="450" />

Fig. 5 : Project and Solution name

Now the project is created with the name _'HelloLearningApiExampleAlgorithm.sln'_
  
<!--![Image 6](https://user-images.githubusercontent.com/44580961/98464421-0ddf2800-21c3-11eb-9951-f66298e25891.png) -->

<img src="(https://user-images.githubusercontent.com/44580961/99438805-5375c080-293a-11eb-928a-ba234162a1ea.png)" width="450" height="300" />

Fig. 6 : Creation of Solution	
	
### Step 2: Create the class library for the algorithm 
	
When solution(HelloLearningApiExampleAlgorithm.sln) is created, by default a class library is also created automatically (.cs file).

We have to change the names accordingly. Here for example, change the class library name as “ExampleLearningApiAlgorithm.cs” as shown in Fig. 6.

ExampleLearningApiAlgorithm.cs serves as the main class folder for the algorithm.

<!--![Image 7]-->

<img src="(https://user-images.githubusercontent.com/44580961/99438878-743e1600-293a-11eb-985d-20de14d1dda5.png)" width="450" height="300" />

Fig. 7 : The project and class library folder structure

### Step 3: Create the Test folder and Test class library for the algorithm 

We should create a Test folder where we can initiate the program and command the directions. 

	Select the project folder --> Right click --> Add --> New Project

![Image 8](https://user-images.githubusercontent.com/44580961/99399514-c4ea4a80-290b-11eb-939f-4ea14c0ee485.png) 

Select the Test file _'MSTest project c#'_ and click on NEXT button as shown in the below Fig. 9.

![Image 9](https://user-images.githubusercontent.com/44580961/99399521-c87dd180-290b-11eb-8e5b-42adaa90a054.png) 

Name the project name as _**HelloLearningApiExampleAlgorithmTest**_ and click on NEXT button. 

![Image 10](https://user-images.githubusercontent.com/44580961/99399539-d16ea300-290b-11eb-9336-5d3f6710190b.png) 

Test project is created under the main solution and rename the class file as _**ExampleLearningApiTest**_ as shown in the below Fig. 11.

<!--![Image 11](https://user-images.githubusercontent.com/44580961/99399545-d3d0fd00-290b-11eb-9c1a-135f301f7f64.png) -->

<img src="(https://user-images.githubusercontent.com/44580961/99399545-d3d0fd00-290b-11eb-9c1a-135f301f7f64.png)" width="450" height="300" />


### Step 4 : Add NuGet Package 'LearningApi' to both projects 

We should add NuGet package called _LearningApi_ to both project by following the steps below, 

		
	Right click on project (HelloLearningApiExampleAlgorithm/HelloLearningApiExampleAlgorithmTest) --> Click on ‘Manage NuGet packages..’ (Fig. 12)	

	in the pop up window --> Click on BROWSE, (Fig. 13)
	
	search for LearningApi and select --> Select the checkbox of LearningApi nuget --> Click on SELECT/ADD PACKAGE button (Fig. 14)

	
<!--![Image 12]()-->

<img src="https://user-images.githubusercontent.com/44580961/99399553-d7fd1a80-290b-11eb-8a42-8e6e11eb47a2.png" width="400" height="550" />

Fig. 12 : NuGet package integration step1,

In the pop up, search for the package LearningAPI , select the latest version and click on ADD PACKAGE button.

<!--![Image 13]()-->

<img src="https://user-images.githubusercontent.com/44580961/99399561-daf80b00-290b-11eb-868e-e4ce3329ad56.png" width="800" height="450" />

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
string testDataPathString = @"SampleData\house_price_train.csv";
```
load the meta data (where the features are explained) by the following bit of code:

```csharp
private DataDescriptor LoadMetaData()
        {
            var des = new DataDescriptor();

            des.Features = new Column[3];
            des.Features[0] = new Column { Id = 1, Name = "size", Index = 0, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[1] = new Column { Id = 2, Name = "rooms", Index = 1, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };
            des.Features[2] = new Column { Id = 3, Name = "price", Index = 2, Type = ColumnType.NUMERIC, DefaultMissingValue = 0, Values = null };

            des.LabelIndex = 2;
            return des;
        }
```

In **Algorithm.cs** file , we implement the *IAlgorithm* in the code which is taken from LearningApi NuGet package. *IAlgorithm*  is in the library and it has a separate structure which we have to use in the project as we already have discussed in the section <a href="#LearningApi_Concept">LearningApi Concept</a>. 

![Image 14](https://user-images.githubusercontent.com/44580961/99401608-62467e00-290e-11eb-8197-6dfa9aa06f32.png)

Fig. 14 : IAlgorithm interface integrated in the project

Here, In _'IScore Run'_ we direct the model to TRAIN interface where the logic for algorithm is defined. 

The following code is used for training the model :

```csharp
for (int epoch = 0; epoch < m_Epochs; epoch++)
            {
                for (int trainDataIndex = 0; trainDataIndex < numTrainData; trainDataIndex++)
                {
                    estimatedOutputLabels[trainDataIndex] = ComputeOutput(inputFeatures[trainDataIndex], weights, bias);
                    squareErrors[trainDataIndex] = ComputeSquareError(actualOutputLabels[trainDataIndex], estimatedOutputLabels[trainDataIndex]);
                }

                double meanSquareError = squareErrors.Sum() / numTrainData;

                loss[epoch] = meanSquareError;
                
                Tuple<double[], double> hyperParameters = GradientDescent(actualOutputLabels, estimatedOutputLabels, inputFeatures, numTrainData, numFeatures);

                // Partial derivatives of loss with respect to weights
                double[] dWeights = hyperParameters.Item1;

                // Updating weights
                for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                {
                    weights[featureIndex] = weights[featureIndex] - m_LearningRate * dWeights[featureIndex];
                }

                // Partial derivative of loss with respect to bias
                double dbias = hyperParameters.Item2;

                // Updating bias
                bias = bias - m_LearningRate * dbias;
            }

            if (ctx.Score as LinearRegressionScore == null)
                ctx.Score = new LinearRegressionScore();

            LinearRegressionScore scr = ctx.Score as LinearRegressionScore;
            scr.Weights = weights;
            scr.Bias = bias;
            scr.Loss = loss;            

            return ctx.Score;

```
In PREDICT interface, all the logics for computing mean square error, outputlabels are provided. 

### Step 6 : Create the *Extension.cs* , *Result.cs* and *Score.cs* files

Extension file in a project facilitates other users to utilise our project code in their implementations. Calling this file in other projects enables the project code in other projects.
      
      Right Click on Project name --> Add --> New Class (Fig. 12_left side)
      
      Select Empty class --> Give the class name ExampleLearningApiAlgorithmExtension --> Click on NEW button (Fig. right side)

![Image 15](https://user-images.githubusercontent.com/44580961/99399573-df242880-290b-11eb-8487-36d3aeaf4b28.png)

Fig. 15 : Adding Extension class to ALgorithm project 

The following is given as code for extension.cs file in order to use it anywhere in the project further:

```csharp
public static LearningApi UseExampleLearningApiAlgorithm(this LearningApi api, double learningRate, int epochs)

        {
            var alg = new ExampleLearningApiAlgorithm(learningRate, epochs);
            api.AddModule(alg, "Linear Regression");
            return api;
        }
```

Likewise, in the example solution, the *ExampleLearningApiAlgorithmResult.cs* and *LearningApiAlgorithmScore.cs* files should be created to define the values which should be storing the result and trained score data. Follow the steps explained above in Fig.12 to create these classes also.

The values are get and set in the _Result.cs_ file with the following code line :

```csharp
public class ExampleLearningApiAlgorithmResult : IResult
    {
        public double[] PredictedValues { get; set; }
    }
```
The values for the features are get and set in the _Score.cs_ file with the following lines of code :

```csharp
public class LinearRegressionScore : IScore
    {
        public double[] Weights { get; set; }

        public double Bias { get; set; }

        public double[] Loss { get; set; }
    }
 ```

### Step 7 : Result 

According to the algorithm, the set of data of house details is given and trained the model with these data. The data for house price is used to calculate the mean square error and When this score is multiplied with each data given, we get the house price value predicted.

![Image 16]()

Fig. 16 : Result is shown here

## LINEAR REGRESSION algorithm code in Github

You can refer this example project in the [Click here to refer the above project code in GitHub..](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Individual%20project_AnushaAshokReddy)
