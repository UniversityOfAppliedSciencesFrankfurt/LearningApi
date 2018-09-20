# Delta Rule Learning

Delta rule learning is a algorithm which uses gradient decent for updating weights of the inputs to neurons. It is a particular special type of Back propagation algorithm. Gradient descent is an optimization algorithm that locates a local minimum of a function by taking proportional steps towards negative gradient of the function as the current point.

The difference between the target activation and the obtained activation is used drive learning. Linear activation method is used to calculate the activation of the output neurons. For a given input vector, the output vector is compared to the correct answer. If the difference is zero, no learning takes place; otherwise, the weights are adjusted to reduce this difference. 

The change in weight from ui to uj is given by: 
                                      dwij = r* ai * ej 
                                      
where r is the learning rate, ai represents the activation of ui and ej is the difference between the expected output and the actual output of uj.

The gradient descent rule updates the weights after calculating the whole error accumulated from all examples, the incremental version approximates the gradient descent error decrease by updating the weights after each training example.

consider the below test example for the better understanding.

## Simple Sequence test

~~~csharp
```//code vomitted for simplicity
   
      
        public void SimpleSequenceTest()
        {
            LearningApi api = new LearningApi();
            api.UseActionModule<object, double[][]>((notUsed, ctx) =>
            {
                const int maxSamples = 10;
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

            api.UseDeltaLearning(0.2, 1000);

            IScore score = api.Run() as IScore;

            double[][] testData = new double[4][];
            testData[0] = new double[] { 2.0, 0.0 };
            testData[1] = new double[] { 4.0, 0.0 }; 
            testData[2] = new double[] {6.0, 0.0 };
            testData[3] = new double[] { 8.0, 0.0 };
            

            var result = api.Algorithm.Predict(testData, api.Context);


            Assert.True(result[0] == 0);
            Assert.True(result[1] == 0);
            Assert.True(result[2] == 1);
            Assert.True(result[3] == 1);

        } ```
        

        
