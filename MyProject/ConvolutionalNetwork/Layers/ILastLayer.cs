using System;
using LearningAPIFramework.Tensor;
namespace LearningAPIFramework.ConvolutionalNetze.Layers
{
    public interface ILastLayer 
    {
        void Backward(Volume y, out double loss);
    }
}