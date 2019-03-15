

# LearningApi
Learning API is Machine Learning Foundation of useful libraries fully implemented in .NET Core. It provides a unique processing API for Machine Learning solutions. 

It has been developed in cooperation with daenet GmBh.
![daenet](https://avatars3.githubusercontent.com/u/12556447?s=50&u=f2cd3be70373c9654b9d53a4f69ddfd7a8ed6596&v=4=)
![uni](https://avatars0.githubusercontent.com/u/12556434?s=400&u=94c1f1c45bee9ffcb167f2f2246dddab19fec420&v=4)

LearningAPI is a foundation of Machine Learning algorithms, which can run in the pipeline of modules compatiple to each other. 
One pipeline module is defined as implementation of interface *IPipeline*.
~~~ csharp
   public interface IPipelineModule
    {

    }

    public interface IPipelineModule<TIN, TOUT> : IPipelineModule
    {
        TOUT Run(TIN data, IContext ctx);
    }

~~~

The module receives an input TIN and context information. Usually TIN is set of data, which results as output of th eprevious module. Typically, first module in the pipeline is responsibe to provide learning data and last module in the pipeline is usually algorithm. 

Following example illustrates how setup the learning pipeline:

~~~ csharp

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
~~~

The code shown above setups the pipeline of two modules. First one is so called *action module*, which defines the custom code to be executed. Second module is setup by following line:
~~~ csharp
api.UsePerceptron(0.02, 10000);
~~~
It injects the perceptron algorithm in the pipeline.

Execution of the pipeline is started with following code:

~~~ csharp
IScore score = api.Run() as IScore;
~~~

When the pipeline starts,  modules are executed in the sequenceordered as they are added to the pipeline. In this case, first action module will be executed and then perceptron algorithm.
After running of the pipeline model is trained. Next common step in Machine Learning applications is called evaluation of the model. 
Following code in previous example shows how to evaluation (predict) the model:

~~~ csharp            
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
~~~

#### Working with batches of data
For more information about training with huge amount of data please read [this](WorkingWithBatches.md)
