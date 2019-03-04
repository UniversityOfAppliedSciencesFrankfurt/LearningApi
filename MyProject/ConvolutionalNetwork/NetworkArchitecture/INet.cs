using System;
using System.Collections.Generic;
using LearningAPIFramework.ConvolutionalNetze.Layers;
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze
{
    public interface INet
    {
        double Backward(Volume y);

        Volume Forward(Volume input, bool isTraining = false);

        double GetCostLoss(Volume input, Volume y);

        List<ParametersAndGradients<double>> GetParametersAndGradients();

        int[] GetPrediction();
    }
}