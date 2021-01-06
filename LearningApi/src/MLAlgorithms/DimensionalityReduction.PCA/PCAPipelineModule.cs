using LearningFoundation.DimensionalityReduction.PCA.Utils;
using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation.DimensionalityReduction.PCA.Exceptions;

namespace LearningFoundation.DimensionalityReduction.PCA
{
    /// <summary>
    /// PCAPipelineModule is used to do Principle Component Analysis on an double[][] array of data
    /// output of this module is the new data point that has smaller dimension.
    /// </summary>
    public class PCAPipelineModule : IPipelineModule<double[][], double[][]>
    {
        private GeneralLinearAlgebraUtils LinagUtils;

        /// <summary>
        /// Maximum of data loss when comparing the estimated data with the original data
        /// </summary>
        public double MaximumLoss { get; set; }

        /// <summary>
        /// The size of the dimension of new data points
        /// </summary>
        public int NewDimensionSize { get; set; }

        /// <summary>
        /// double array [0 -> i] which show how many information could be retrieve
        /// if using ong component (i+1) to reconstruct the original data
        /// </summary>
        public double[] ExplainedVariance { get; private set; }

        /// <summary>
        /// double array[0 -> i] which show how many information could be retrieve
        /// if using component from 1 -> (i+1)  to reconstruct the original data
        /// </summary>
        public double[] CummulativeExplainedVariance { get; private set; }

        /// <summary>
        /// matrix store new K components (coordinate vectors) of PCA
        /// KComponents[i] => column vectors represent component ith
        /// </summary>
        public double[][] KComponents { get; private set; }

        /// <summary>
        /// This is the estimation of the original data, which is reconstructed from new data points and PCA components
        /// </summary>
        public double[][] EstimatedData {get; private set; }

        /// <summary>
        /// Constructor, to create a new instance of PCAPipelineModule
        /// </summary>
        /// <param name="newDimensionSize">The size of new dimension of new data points</param>
        /// <param name="maximumLoss">The maximum loss that data from PCA should not be come over</param>
        public PCAPipelineModule(int newDimensionSize = 0, double maximumLoss = 0.05)
        {
            this.NewDimensionSize = newDimensionSize;
            this.LinagUtils = GeneralLinearAlgebraUtils.getImpl();
            this.MaximumLoss = maximumLoss;
        }

        /// <summary>
        /// This will be invoked by LearningApi Module
        /// After all paramater have been set, this method could also be called manually
        /// </summary>
        /// <param name="data">double[][] data with data[i] is the column vector of each data point</param>
        /// <param name="ctx">currently we don't use this at the moment</param>
        /// <returns>new data points stored in data[][], with reduced dimensions size</returns>
        public double[][] Run(double[][] data, IContext ctx)
        {
            Console.Out.WriteLine(data);
            int n_samples = data.GetLength(0);
            int dimensionOfData = data[0].GetLength(0);

            if (dimensionOfData < this.NewDimensionSize)
            {
                throw new InvalidDimensionException("New dimension is bigger than the current dimension of data");
            }

            double[] meanVector = LinagUtils.CalculateMeanVector(data);
            double[][] centeredData = LinagUtils.SubstractMatrixVector(data, meanVector);
            double[][] covarianceMatrix = LinagUtils.CalculateCovarienceMatrix(centeredData);
            this.CalculateExplainedVariance(covarianceMatrix);
            if (this.NewDimensionSize == 0)
            {
                this.NewDimensionSize = this.CalculateNewDimensionSize(dimensionOfData - 1);
            }
            double[][] sortedEigenVectors = LinagUtils.CalculateEigenVectors(covarianceMatrix);
            double[][] k_EigenVectors = new double[this.NewDimensionSize][];
            for (int i = 0; i < this.NewDimensionSize; i++)
            {
                k_EigenVectors[i] = sortedEigenVectors[i];
            }
            this.KComponents = k_EigenVectors;
            double[][] newDataMatrix = LinagUtils.DotProductM2M(LinagUtils.Transpose(k_EigenVectors), centeredData);
            this.CalculateEstimatedData(newDataMatrix, KComponents, meanVector);
            return newDataMatrix;
        }

        /// <summary>
        /// Calculate the ExplainedVariance and CummulativeExplainedVariance
        /// These two information could be retrieve by mesaure the percent of contribution 
        /// of eigen values.
        /// </summary>
        /// <param name="covarianceMatrix"></param>
        private void CalculateExplainedVariance(double[][] covarianceMatrix)
        {
            double[][][] svdMatrices = LinagUtils.SVD(covarianceMatrix);
            double[] eigenValues = svdMatrices[3][0];
            double total = 0.0;
            foreach (double val in eigenValues)
            {
                total += val;
            }
            this.ExplainedVariance = new double[eigenValues.GetLength(0)];
            this.CummulativeExplainedVariance = new double[eigenValues.GetLength(0)];
            for (int i = 0; i < eigenValues.GetLength(0); i++)
            {
                this.ExplainedVariance[i] = 100.0*(eigenValues[i] / total);
                if (i<=0)
                {
                    this.CummulativeExplainedVariance[i] = this.ExplainedVariance[i];
                } else
                {
                    this.CummulativeExplainedVariance[i] = this.CummulativeExplainedVariance[i-1] +
                                                           this.ExplainedVariance[i];
                }
            }
        }

        /// <summary>
        /// Tried to calculate the new dimension size with the constrain of MaximumLoss
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns>size of new dimension</returns>
        private int CalculateNewDimensionSize(int defaultValue)
        {
            double retainedInformationRatio = 100.0 - (MaximumLoss*100.0);
            for (int i = 0; i < this.CummulativeExplainedVariance.GetLength(0); i++)
            {
                if (this.CummulativeExplainedVariance[i] >= retainedInformationRatio)
                {
                    return i + 1;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// This will tried to reconstruct the original data from newData, 
        /// PCA components and the mean vector from original data
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="components"></param>
        /// <param name="meanVector"></param>
        private void CalculateEstimatedData(double[][] newData, double[][] components, double[] meanVector) {
            this.EstimatedData = LinagUtils.DotProductM2M(components, newData);
            this.EstimatedData = LinagUtils.AddMatrixVector(this.EstimatedData, meanVector);
        }
    }
}
