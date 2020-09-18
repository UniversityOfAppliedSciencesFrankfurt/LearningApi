Euclidean Color Filter:
===================


The function of a Euclidean Color Filter is to filter out a specific color spectrum from an image.

## How it works?

the implementation of the filter, a radius and a color center (RGB-value) must be specified. Then the algorithm calculates for each pixel of the image the Euclidean distance to the specific color center. If the distance is within the specific radius, the pixel keeps its RGB-value, if the distance is outside the radius, the RGB-value of the pixel is changed to black (0,0,0). After all pixels have been processed, you get the filtered image as output [1]. The calculation of the Euclidean distance is the most important point for the implementation of the filter.

The interface "IPipeLineModule" of the Learning Api, which has a 3 dimensional double array as input and output. Since the input of a color filter is an image, it must be converted from a bitmap to a 3 dimensional array. The loading and the conversion of the image takes place in a UnitTest, because it was required like this. After the parameters for the radius and the color center have been specified, an image can finally be loaded. After the conversion into the 3-dimensional array, the algorithm is executed in the main class "EuclideanFilterModule". The main class has only one method, the "Run" method, which belongs to the interface. The "Run" method uses the "CalcDistance" class to calculate the Euclidean distance. There is also the class "GetAndSetPixels", which is needed to get the current color value of a pixel and to set a certain color value for the pixels.

- [EuclideanFilterModule.cs](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/Deepali_EclideanFilter/LearningApi/EuclideanColorFilter/EuclideanFilter/EuclideanFilterModule.cs)
- [EuclideanFilterModuleExtensions.cs](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/Deepali_EclideanFilter/LearningApi/EuclideanColorFilter/EuclideanFilter/EuclideanFilterModuleExtensions.cs) 
- [GetAndSetPixels.cs](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/Deepali_EclideanFilter/LearningApi/EuclideanColorFilter/EuclideanFilter/GetAndSetPixels.cs)
- [CalcDistance.cs](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/Deepali_EclideanFilter/LearningApi/EuclideanColorFilter/EuclideanFilter/CalcDistance.cs)

## Link to the Test cases:

- [EuclideanColorFilterTests](https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/Deepali_EclideanFilter/LearningApi/EuclideanColorFilter/EuclideanColorFilterTests)
