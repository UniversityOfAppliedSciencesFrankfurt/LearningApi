##### Author: Trinh Tran Nguyen Phuong

---
### How to use
1. **Download NuGet package from [LearningApi.DR.1.3.0.nupkg](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-dystsys-2018-2019-softwareengineering/blob/TrinhTran/MyProject/OutputResult/LearningApi.DR.1.3.0.nupkg)**
2. **Add it to NuGet repository and as dependency**
4. **Root namespace is LearningFoundation.DimensionalityReduction.PCA**
3. **The PCAPipelineModule and PCAPipelineExtensions class should be used for LearningApi**

---
### Example LearningApi Intergration
```java
LearningApi api = new LearningApi();
api.UseActionModule<object, double[][]>((notUsed, ctx) =>
{
    //This is an examle of how data should be inject into PCAPipelineModule
    string filePath = Path.Combine(INPUT_DATA_DIR, "face1.csv");
    double[][] data = CsvUtils.ReadCSVToDouble2DArray(filePath, ',');
    
    //PCAPipelineModule receive data as double[][] array
    return data;
});

//Now we tell LearningApi to use a PCAPipelineModule instance, we named it "PCAPipelineModule-Ex1"
//The new data should only have the new dimensions with size 10
api.UsePCAPipelineModule(moduleName: "PCAPipelineModule-Ex1", newDimensionSize: 10);

// or api.UsePCAPipelineModule(moduleName: "PCAPipelineModule-Ex1", maximumLoss: 0.05); to specify the maximum amount of loss 
// that we want to have if we plan to use the result from PCA to estimate the orginal data.

//Now all modules will be run, output of the prior module will be pipe to the input of the next module
api.Run();

//After each calculation PCAPipelineModule will contains lots more useful information
//To retrieve this, we will use its name above to get back to module instance
PCAPipelineModule moduleInstance = api.GetModule<PCAPipelineModule>("PCAPipelineModule");
double[][] explainedVariance = new double[2][];
explainedVariance[0] = moduleInstance.ExplainedVariance;
explainedVariance[1] = moduleInstance.CummulativeExplainedVariance;
```
---
### API References
```
+-- namespace: LearningFoundation.DimensionalityReduction.PCA
|   +-- class: PCAPipelineModule
|   +-- class: PCAPipelineExtensions
|   +-- namespace: .Utils
|   |   +-- class: GeneralLinearAlgebraUtils
|   |   +-- class: MathNetNumericsLinearAlgebraUtils
|   |   +-- class: CsvUtils
|   |   +-- class: DataParserUtils
|   |   +-- class: ImageUtils
|   +-- namespace: .Exceptions
|   |   +-- class: InvalidDimensionException
```
---
##### class: ```PCAPipelineModule``` implement ```LearningFoundation.IPipelineModule```
###### Public Fields:
1. ```public double MaximumLoss {get; set;}: to set the maximum information loss of estimated data, which is re-constructed from PCA components```
2. ```public int NewDimensionSize { get; set; }: to set the new dimension size for the new data points, after project onto the new coordinate of PCA components```
3. ```public double[] ExplainedVariance { get; private set; }: to get the explained variance of each component of PCA's component```
4. ```public double[] CummulativeExplainedVariance { get; private set; }: to get the cumulative explained variance of ith component of PCA```
5. ```public double[][] KComponents { get; private set; }: maxtrix stored each component of PCA's as a column vector.```
6. ```public double[][] EstimatedData {get; private set; }: matrix stored the estimated data, which is reconstructed from PCA's components and new data points```
###### Public Methods:
1. ```public PCAPipelineModule(int newDimensionSize = 0, double maximumLoss = 0.05)```
2. ```public double[][] Run(double[][] data, IContext ctx): data params is used, however ctx object is currently not in used. Because this implementation should be able to run on any data which knowing the label of data```
