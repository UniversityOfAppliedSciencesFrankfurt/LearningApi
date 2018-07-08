# Self Organizing Map

A self- organizing map is a clustering technique to help to uncover categories in large datasets. It is a special type of unsupervised neural networks, where neurons are arranged in a single and 2-dimentional grid, which are arranged in the shape of rectangles or hexagons. 
Through multiple iterations, neurons in the grid will gradually join together around the areas with high density of data points. So, the areas with many neurons will form a cluster in the data. As the neurons move, they inadequately bend and twist the grid to more closely influence the overall shape of the data. 

## How does it work :
SOM includes neurons in grid, which gradually adapt to the intrinsic shape of the data. The result allows visualizing data points and identifying clusters in a lower dimension. 
### SOM follows the below steps in iterative process:
**Step 0**: Randomly position the grid’s neurons in the data space.

**Step 1**: Select one data point, either randomly or systematically cycling through the dataset in order

**Step 2**: Find the neuron that is closest to the chosen data point. This neuron is called the Best Matching Unit (BMU).

**Step 3**: Move the BMU closer to that data point. The distance moved by the BMU is determined by a learning rate, which decreases after each iteration.

**Step 4**: Move the BMU’s neighbors closer to that data point as well, with farther away neighbors moving less. Neighbors are identified using a radius around the BMU, and the value for this radius decreases after each iteration.

**Step 5**: Update the learning rate and BMU radius, before repeating Steps 1 to 4. Iterate these steps until positions of neurons have been stabilized.

# Delta Rule Learning:

Delta rule learning is a algorithm which uses gradient decent for updating weights of the inputs to neurons. It is a particular special type of Back propagation algorithm. Gradient descent is an optimization algorithm that locates a local minimum of a function by taking proportional steps towards negative gradient of the function as the current point.

The difference between the target activation and the obtained activation is used drive learning. Linear activation method is used to calculate the activation of the output neurons. 
For a given input vector, the output vector is compared to the correct answer. If the difference is zero, no learning takes place; otherwise, the weights are adjusted to reduce this difference. 
The change in weight from ui to uj is given by: 
               **dwij** **=** **r*** **ai** * **ej**
where **r** is the learning rate, **ai** represents the activation of **ui** and **ej** is the difference between the expected output and the actual output of **uj**. 

The gradient descent rule updates the weights after calculating the whole error accumulated from all examples, the incremental version approximates the gradient descent error decrease by updating the weights after each training example.

# Gaussian mean filter :
It is a method of smoothing images reducing the amount of intensity variation between one pixel and the next. It is often used to reduce noise in images
## How it works?
It replaces each pixel value in an image with the mean or average value of its neighbors, including itself. It eliminates the pixel values which are unrepresentative of their surroundings. Mean filtering is also as a convolution filter, which is based around a kernel representing the shape and size of the neighborhood to be sampled when calculating the mean.
 
Often a 3×3 square kernel is used, larger kernels can be used for severe smoothing. And also small kernel can be applied more than once in order to produce a similar but not identical effect as a single pass with a large kernel. 

# Canny Edge Detection:
Canny edge detector is an edge detecting algorithm which uses a multi stage algorithm to detect a wide range of edges in images. 
## The criteria for the edge detection includes:
1.	Edge detection with low error rate, that means it should catch as many edges as possible in the given image.
2.	It localize on the center of the edge from the detected edge point.
3.	A given edge in the image are marked once and does not create false edges.

This algorithm follows the strict defined methods providing good and reliable detection 
## Process Procedure.
1.	Gaussian filter is applied to smooth the image in order to remove the noise
2.	Find the intensity gradient of the image
3.	Spurious response to detect the edges are taken care by applying the non/maximum suppression
4.	Potential edges are detected using double threshold 
5.	Strong edges are finalized.

