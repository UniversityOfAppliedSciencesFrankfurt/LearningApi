WiSE-18/19 ML-2.1:CONVOLUTIONAL NETWORK IMPLEMENTATION UNDER LEARNING API FRAMEWORK:

Convolutional Networks incorporate multiple layers of ,,filters'' which are stacked one atfter the other for extracting learnable features of image type objects.
The network is not useful for standard decision making or curve fitting purposes.It trains its filter co-efficients through backpropogation algorithm...The most
impressive feature of this algorithm is the limited counts of arbitrary parameters(,,weights'' ) required for a large range of size of the image object.
 
In this particular Learning API implementation we are using two stacked layers of 5x5 Convolutional filters first of 8 filter depth and the second of 16.
Each followed by a Rectified Linear Unit and a Pool layer .The final layer stacks the 28x28 image into a SoftMax Class of 10 outputs of the digit classes 0-9.

IP Layer --> Convolutional Filters --> Rectified Linear Unit --> Pool --> Convolutional Filters --> Rectified Linear Unit -->Pool --> Fully Connected Layer --> SoftMax Layer
(28x28)					(8 filters)															(16 filters)													 (10 Output Classes)

																										
References:
https://github.com/cbovar/ConvNetSharp
End to end text recognition with convolutional networks”   
http://www.robotics.stanford.edu/~ang/papers/ICPR12-TextRecognitionConvNeuralNets.pdf(references)