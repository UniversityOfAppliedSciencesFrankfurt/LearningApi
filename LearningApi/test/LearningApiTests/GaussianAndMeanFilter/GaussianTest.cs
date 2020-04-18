using FluentAssertions;
using GaussianAndMeanFilter;
using LearningFoundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LearningApiTests.GaussianAndMeanFilter
{
    [TestClass]
    public class BasicGaussianAndMeanTest
    {
        [TestMethod]
        public void GaussianFilter_FourByFourInputMatrix_AveragedCenterInputs()
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                return GetInputMatrix();
            });

            lApi.AddModule(new GaussianFilter());

            double[,,] result = lApi.Run() as double[,,];

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(GetExpectedGaussianMatrix());
        }

        [TestMethod]
        public void MeanFilter_FourByFourInputMatrix_AveragedCenterInputs()
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                return GetInputMatrix();
            });

            lApi.AddModule(new MeanFilter());

            double[,,] result = lApi.Run() as double[,,];

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(GetExpectedMeanMatrix());
        }

        [TestMethod]
        public void GaussianAndMeanFilter_FourByFourInputMatrix_AveragedCenterInputs()
        {
            LearningApi lApi = new LearningApi();
            lApi.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                return GetInputMatrix();
            });

            lApi.AddModule(new GaussianFilter());
            lApi.AddModule(new MeanFilter());

            double[,,] result = lApi.Run() as double[,,];

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(GetExpectedGaussianAndMeanMatrix());
        }
        private static double[,,] GetInputMatrix()
        {
            return new double[4, 4, 3]
                {
                    { {1, 0, 1}, {2, 2, 5}, {3, 0, 1}, {2,5,1} },
                    { {2, 4, 7}, {4, 3, 1}, {1, 2, 1}, {5,3,1} },
                    { {3, 2, 2}, {1, 2, 3}, {4, 1, 3}, {2,2,2} },
                    { {1, 1, 2}, {2, 1, 1}, {3, 5, 2}, {2,1,1} }
                };
        }

        private static double[,,] GetExpectedGaussianMatrix()
        {
            return new double[4, 4, 3]
                {
                    { {1, 0, 1}, {2, 2, 5}, {3, 0, 1}, {2,5,1} },
                    { {2, 4, 7}, {2.4375, 2.1875, 2.6875}, {2.4921875, 1.9609375, 1.8984375}, {5,3,1} },
                    { {3, 2, 2}, {2.21044921875, 2.02099609375, 2.64208984375}, {2.92767333984375, 2.07196044921875, 2.17303466796875}, {2,2,2} },
                    { {1, 1, 2}, {2, 1, 1}, {3, 5, 2}, {2,1,1} }
                };
        }

        private static double[,,] GetExpectedMeanMatrix()
        {
            return new double[4, 4, 3]
                {
                    { {1, 0, 1}, {2, 2, 5}, {3, 0, 1}, {2,5,1} },
                    { {2, 4, 7}, {2.3333333333333335, 1.7777777777777777, 2.6666666666666665}, {2.4814814814814818, 2.0864197530864197, 2.1851851851851851}, {5,3,1} },
                    { {3, 2, 2}, {2.3127572016460909, 2.2071330589849105, 2.7613168724279831}, {2.791952446273434, 2.1190367322054566, 1.9570187471422038}, {2,2,2} },
                    { {1, 1, 2}, {2, 1, 1}, {3, 5, 2}, {2,1,1} }
                };
        }

        private static double[,,] GetExpectedGaussianAndMeanMatrix()
        {
            return new double[4, 4, 3]
                {
                    { {1, 0, 1}, {2, 2, 5}, {3, 0, 1}, {2,5,1} },
                    { {2, 4, 7}, {2.3408677842881946, 1.8045993381076388, 2.8223402235243054}, {2.66346420476466, 2.2064992645640431, 2.1706558039158952}, {5,3,1} },
                    { {3, 2, 2}, {2.3491616164051781, 2.34489501618227, 2.6453467265732171}, {2.6979074383668649, 2.2697726742303, 1.8679308246646853}, {2,2,2} },
                    { {1, 1, 2}, {2, 1, 1}, {3, 5, 2}, {2,1,1} }
                };
        }
    }
}
