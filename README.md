

# LearningApi
Learning API is Machine Learning Foundation of useful libraries fully implemented in .NET Core. It provides a unique processing API for Machine Learning solutions. 

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

# Implementation of Canny Edge detection Algorithm
Canny edge detector is an edge detecting algorithm which uses a multi stage algorithm to detect a wide range of edges in images.

## The criteria for the edge detection includes:
Edge detection with low error rate, that means it should catch as many edges as possible in the given image.
It localize on the center of the edge from the detected edge point.
A given edge in the image are marked once and does not create false edges.
This algorithm follows the strict defined methods providing good and reliable detection

## Process Procedure.
1 Gaussian filter is applied to smooth the image in order to remove the noise  
2 Find the intensity gradient of the image  
3 Spurious response to detect the edges are taken care by applying the non/maximum suppression  
4 Potential edges are detected using double threshold  
5 Strong edges are finalized.  
 
 Find [Documentation link **here**](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/review-2019/project_documentations/canny%20edge%20detection.docx).

# Implementation of Delta Rule Learning 

Delta rule learning is a algorithm which uses gradient decent for updating weights of the inputs to neurons. It is a particular special type of Back propagation algorithm. Gradient descent is an optimization algorithm that locates a local minimum of a function by taking proportional steps towards negative gradient of the function as the current point.

The difference between the target activation and the obtained activation is used drive learning. Linear activation method is used to calculate the activation of the output neurons. For a given input vector, the output vector is compared to the correct answer. If the difference is zero, no learning takes place; otherwise, the weights are adjusted to reduce this difference.

The change in weight from ui to uj is given by: dwij = r* ai * ej

where r is the learning rate, ai represents the activation of ui and ej is the difference between the expected output and the actual output of uj.

The gradient descent rule updates the weights after calculating the whole error accumulated from all examples, the incremental version approximates the gradient descent error decrease by updating the weights after each training example.

consider the below test example for the better understanding.

Find [Documentation link **here**](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/review-2019/project_documentations/Delta%20rule%20learning%20.docx).

# Implementation of Gaussian mean filters for noise 
It is a method of smoothing images reducing the amount of intensity variation between one pixel and the next. It is often used to reduce noise in images

How it works?
It replaces each pixel value in an image with the mean or average value of its neighbors, including itself. It eliminates the pixel values which are unrepresentative of their surroundings. Mean filtering is also as a convolution filter, which is based around a kernel representing the shape and size of the neighborhood to be sampled when calculating the mean.

Often a 3×3 square kernel is used, larger kernels can be used for severe smoothing. And also small kernel can be applied more than once in order to produce a similar but not identical effect as a single pass with a large kernel.

Find [Documentation link **here**](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/review-2019/project_documentations/Mean%20and%20Gaussian%20filters.docx).

# MULTILAYER         PERCEPTRON

A Perceptron called an artificial neuron takes several inputs and produces a binary output. A multilayer perceptron (MLP) is a class of feed forward artificial neural network which contains of at least three layers of nodes. Except for the input nodes, each node is a neuron that uses a nonlinear activation function .

The leftmost layer in this network is called the input layer, and the neurons within the layer are called input neurons. The rightmost or output layer contains the output neurons. The middle layer is called a hidden layer.

Find [Documentation link **here**](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/review-2019/project_documentations/MultilayerPerceptron%20column.docx).


# Self Organizing Map 

The self-organizing map (SOM) is an automatic data-analysis method. It is widely applied to clustering problems and data exploration in industry, finance, natural sciences, and linguistics. It is one of the most popular artificial neural network (ANN) models that is trained using unsupervised learning to produce a low dimensional (especially two-dimensional), discretized representation of the input space of the training samples which is called a map.


Self-Organizing map is introduced by the Finnish professor Teuvo Kohonen in 1980s is also known as kohonen map or network. With a small number of nodes, SOM behaves in a way that is similar to K-means, larger self-organizing maps rearrange data in a way that is fundamentally topological in character.

Find [Documentation link **here**](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/review-2019/project_documentations/sop.docx).


