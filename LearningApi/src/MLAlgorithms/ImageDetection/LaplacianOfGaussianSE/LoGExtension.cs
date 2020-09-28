using LearningFoundation;

namespace LaplacianOfGaussianSE
{
    public static class LoGExtension
    {
        public static LearningApi UseLaplacianOfgaussian(this LearningApi api)
        {
            LaplacianOfgaussian Imgextension = new LaplacianOfgaussian();
            api.AddModule(Imgextension, "UseLaplacianOfgaussian");
            return api;
        }

    }
}
