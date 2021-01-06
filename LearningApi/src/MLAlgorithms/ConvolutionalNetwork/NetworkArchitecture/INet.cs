using System;
using System.Collections.Generic;
using ConvolutionalNetworks.Layers;
using ConvolutionalNetworks.Tensor;

namespace ConvolutionalNetworks
{
    /// <summary>
    /// Stacked Network Implementation Interface
    /// </summary>
    public interface INet
    {
        /// <summary>
        /// Backward Pass Framework
        /// </summary>
        /// <param name="y">Output Tensor</param>
        /// <returns>Backward Pass Loss</returns>
        double Backward(Volume y);

        /// <summary>
        /// Forward Pass
        /// </summary>
        /// <param name="input">Input Tensor</param>
        /// <param name="isTraining">Boolean isTraining</param>
        /// <returns>Forward Pass Tensor</returns>
        Volume Forward(Volume input, bool isTraining = false);

        /// <summary>
        /// Compute Mean Squared Loss
        /// </summary>
        /// <param name="input">Input Tensor</param>
        /// <param name="y">Output Tensor</param>
        /// <returns>Mean Squared loss</returns>
        double GetCostLoss(Volume input, Volume y);

        /// <summary>
        /// List all the tensors and their respective gradients
        /// </summary>
        /// <returns>Tangent Points and Derivatives</returns>
        List<ParametersAndGradients<double>> GetParametersAndGradients();

        /// <summary>
        /// Gets Prediction for output classes
        /// </summary>
        /// <returns>Prediction label</returns>
        int[] GetPrediction();
    }
}
