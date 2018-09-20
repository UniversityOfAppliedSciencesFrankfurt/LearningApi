# Delta Rule Learning

Delta rule learning is a algorithm which uses gradient decent for updating weights of the inputs to neurons. It is a particular special type of Back propagation algorithm. Gradient descent is an optimization algorithm that locates a local minimum of a function by taking proportional steps towards negative gradient of the function as the current point.

The difference between the target activation and the obtained activation is used drive learning. Linear activation method is used to calculate the activation of the output neurons. For a given input vector, the output vector is compared to the correct answer. If the difference is zero, no learning takes place; otherwise, the weights are adjusted to reduce this difference. The change in weight from ui to uj is given by: dwij = r* ai * ej where r is the learning rate, ai represents the activation of ui and ej is the difference between the expected output and the actual output of uj.
