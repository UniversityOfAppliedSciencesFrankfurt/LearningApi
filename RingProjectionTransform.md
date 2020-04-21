# Ring Projection Transform :
The Ring Projection Algorithm is used to solve the orientation issues. In these algorithms the image can be recognized when placed in any angles. If the image is moved to 180 degree or 270 degree then these Algorithm matches with the original image and identifies it.

## How it works?
In ring projection suppose we have a set of M-patterns which are classified as M-classes and each class has only one pattern sample and if we have S-different sizes of each pattern and allow R different orientation. The first one is to treat them as single pattern sample classes. This means the uncertainty will increase considerably. For the above inputs we will have M x S x R classes. The second solution is that we only consider the categories of the pattern and disregard their sizes or orientation. This means the uncertainty will remain same as that of the set with M classes. 


![Ring Projection][ringProjection]

[ringProjection]: https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/blob/SJ90/ring-projection.png "Ring Projection Transform"

 ### Link to test case: https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi/tree/master/LearningApi/test/LearningApiTests/UnitTestRingProjectionAlgorithm/RingProjectionTest.cs  
