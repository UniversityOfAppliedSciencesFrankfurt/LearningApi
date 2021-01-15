## Example Pipeline Module using LearningApi 

Pipeline module is a canal to send the data to the actual Algorithm. For a deeper knowledge on Pipeline Module click on [Pipeline Module Concept..](https://universityofappliedsciencesfrankfurt.github.io/LearningApi/#What_is_Pipeline_Module)

Let's implement Pipelinemodule for a Convolution Filter.

### Step 1: Create a solution for Pipeline module

This does not have any particular structure and we won’t pass any major algorithm here. 

In the Visual Studio, create a new solution by following the steps -
	
    Navigate to File --> New --> Project/New Solution

Use the selectors on the left side to choose the different types of programming languages or platforms to work with. For example, we are creating a class library with the template .NET STANDARD under the Visual C# selector as show in Fig. 4.

    Click on NEXT 	

<!--![Image 4](https://user-images.githubusercontent.com/44580961/98464414-04ee5680-21c3-11eb-82fe-910a29ed7d4d.png) -->

<img src="https://user-images.githubusercontent.com/44580961/98464414-04ee5680-21c3-11eb-82fe-910a29ed7d4d.png" width="600" height="450" />

Fig. 16 : New Project

For our example - given the project name as **“HelloLearningApiPipelineModule”**	

    Name the project --> Solution Name --> Specify the location --> Click OK/CREATE
    
<!--![Image 17](https://user-images.githubusercontent.com/44580961/98464517-bf7e5900-21c3-11eb-9a71-1d03adfea118.png)--> 

<img src="https://user-images.githubusercontent.com/44580961/98464517-bf7e5900-21c3-11eb-9a71-1d03adfea118.png" width="600" height="450" />

Fig. 17 : Project and Solution name

Now the project is created with the name _'HelloLearningApiPipelineModule.sln'_
  
<!--![Image 18](https://user-images.githubusercontent.com/44580961/98464519-c2794980-21c3-11eb-81e7-dee1ccd54601.png) -->

<img src="https://user-images.githubusercontent.com/44580961/98464519-c2794980-21c3-11eb-81e7-dee1ccd54601.png" width="500" height="300" />

Fig. 18 : Creation of Solution	

### Step 2: Create the class library for the module 
	
When solution(HelloLearningApiPipelineModule.sln) is created, by default a class library is also created automatically (.cs file).

Change the class library name as “HelloLearningApiPipelineModule.cs” and also create a nwe class withe name 'HelloLearningApiPipelineModuleExtension' as shown in Fig. 19.

![Image 19](https://user-images.githubusercontent.com/44580961/98464522-c60cd080-21c3-11eb-85cf-dea9250c2e4d.png) 

Fig. 19 : Pipeline and Extension class files

### Step 3 : Add NuGet Package 'LearningApi' to our pipeline module project 

We should add NuGet package called _LearningApi_ to our project by following the steps below, 

		
	Right click on project (HelloWorldTutorial.sln) --> Click on ‘Manage NuGet packages..’ 

	in the pop up window --> Click on BROWSE, 
	
	search for LearningApi and select --> Select the checkbox of LearningApi nuget --> Click on SELECT/ADD PACKAGE button 

<!--![Image 20](https://user-images.githubusercontent.com/44580961/98464524-ca38ee00-21c3-11eb-9542-c05a6e9922f1.png) -->

<img src="https://user-images.githubusercontent.com/44580961/98464524-ca38ee00-21c3-11eb-9542-c05a6e9922f1.png" width="400" height="300" />

Fig. 20 : Nuget package added to pipeline project

### Step 5 : Implement IPipeline Module 

Ipipeline Module from LearningApi should be integrated in the Module coding as shown in the Fig. 21.

![Image 21](https://user-images.githubusercontent.com/44580961/98464526-cc02b180-21c3-11eb-9e2d-1afa8e1f86d2.png)

Fig. 21 : IPipeline module Interface in example module

### Step 6 : Coding for the example pipeline module logic of convolution filter 

This is not a major algorithm, instead a small pre processing of Convolution filter which can be used for any other algorithms as the data. Code format is as shown below, 

![Image 22](https://user-images.githubusercontent.com/44580961/98464527-d02ecf00-21c3-11eb-81e8-6180c1901fac.png)

Fig. 22 : IPipeline module for example module


## Pipeline Module Example code in Github

You can refer this example project by clicking here : [Refer the above Pipeline Module code in GitHub](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/src/Individual%20project_AnushaAshokReddy/HelloLearningApiPipelineModule)
