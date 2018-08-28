using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralNet.RestrictedBolzmannMachine2
{
    class CRbm : Rbm
    {
        protected int numFeatureVector;
        protected double[] featureVector;
        protected double[][] fhWeights;
        protected double[][] fvWeights;


        public CRbm(double[] featureVector, int numVisible, int numHidden, int maxEpochs = 1000, double learnRate = 0.01) : base(numVisible, numHidden, maxEpochs, learnRate)
        {

            this.featureVector = featureVector;
            this.numFeatureVector = this.featureVector.Length;

            initializeFeatureWeightVectors();
        }

        private void initializeFeatureWeightVectors()
        {
            fhWeights = new double[numFeatureVector][];
            fvWeights = new double[numFeatureVector][];

            for (int i = 0; i < numFeatureVector; ++i)
            {
                fhWeights[i] = new double[m_NumHidden];
                fvWeights[i] = new double[numVisible];
            }
                

            for (int i = 0; i < numFeatureVector; ++i)
            {
                // Initialize The Feature--Hidden Weight Vectors
                for (int j = 0; j < m_NumHidden; j++)
                {
                    fhWeights[i][j] = generateRandomWeightOrBias();
                }

                // Initialize the Feature--Visible Weight Wectors
                for (int j = 0; j < numVisible; j++)
                {
                    fvWeights[i][j] = generateRandomWeightOrBias();
                }
            }        
        }


        protected new double calculateSumForHiddenIndex(int hiddenIndx) => base.calculateSumForHiddenIndex(hiddenIndx) + calculateFeatureVectorSum(hiddenIndx);


        private double calculateFeatureVectorSum(int hiddenIndx)
        {
            double sum = 0.0;

            for(int i = 0; i < numFeatureVector; i++)
            {
                sum += featureVector[i] * fhWeights[i][hiddenIndx];
            }

            return sum;
        }

        protected new double calculateSumForVPrimeFromH(int visibleIndex) => base.calculateSumForVPrimeFromH(visibleIndex) + calculateSumForVPrimeFromHFeature(visibleIndex);

        private double calculateSumForVPrimeFromHFeature(int visibleIndex)
        {
            double sum = 0.0;

            for(int i = 0; i < numFeatureVector; i++)
            {
                sum += featureVector[i] * fvWeights[i][visibleIndex];
            }

            return sum;
        }


        protected new double calculateSumForHPrimeFromVPrime(double[] vPrime, int hiddenValIndx) => base.calculateSumForHPrimeFromVPrime(vPrime, hiddenValIndx) + calculateFeatureVectorSum(hiddenValIndx);

        protected new void updateWeights(double[][] posGradVH, double[][] negGradVH)
        {
            base.updateWeights(posGradVH, negGradVH);
            updateFeatureRelatedWeights();
        }

        private void updateFeatureRelatedWeights()
        {
            double[][] posGradFH = MathFunctions.OuterProd(featureVector, m_HidValues);
            double[][] negGradFHPrime = MathFunctions.OuterProd(featureVector, hPrime);

            for (int row = 0; row < numFeatureVector; ++row)
                for (int col = 0; col < m_NumHidden; ++col)
                    fhWeights[row][col] += learnRate * (posGradFH[row][col] - negGradFHPrime[row][col]);


            double[][] posGradFV = MathFunctions.OuterProd(featureVector, m_VisibleValues);
            double[][] negGradFVPrime = MathFunctions.OuterProd(featureVector, vPrime);

            for (int row = 0; row < numFeatureVector; ++row)
                for (int col = 0; col < numVisible; ++col)
                    fvWeights[row][col] += learnRate * (posGradFV[row][col] - negGradFVPrime[row][col]);
        }
    }
}
