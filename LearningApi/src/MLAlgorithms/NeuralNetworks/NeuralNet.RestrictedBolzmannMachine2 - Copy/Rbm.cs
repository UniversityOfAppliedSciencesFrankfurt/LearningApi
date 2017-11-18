using NeuralNetworks.Core;
using System;
using LearningFoundation;


namespace NeuralNet.RestrictedBolzmannMachine2
{
    public class Rbm : NeuralNetCore
    {
        private double learnRate;

        private int maxEpochs;

        /// <summary>
        /// Number of visible nodes.
        /// </summary>
        private int numVisible;    
        
        /// <summary>
        /// Number of hidden nodes.
        /// </summary>
        private int numHidden;

        private Func<double, double> m_ActivationFunction = logSig;

        private double[] m_VisibleVals;

        /// <summary>
        /// Visible node values (0, 1)
        /// </summary>
        public double[] visValues;
        public double[] visProbs;
        public double[] visBiases;

        public double[] hidValues;
        public double[] hidProbs;
        public double[] hidBiases;

        public double[][] vhWeights;

        private Random m_Rnd = new Random();

        public Rbm(int numVisible, int numHidden)
        {
            this.numVisible = numVisible;
            this.numHidden = numHidden;

            //
            // Allocate matrixes

            visValues = new double[numVisible];
            visProbs = new double[numVisible];
            visBiases = new double[numVisible];

            hidValues = new double[numHidden];
            hidProbs = new double[numHidden];
            hidBiases = new double[numHidden];

            vhWeights = new double[numVisible][];  // visible-to-hidden
            for (int i = 0; i < numVisible; ++i)
                vhWeights[i] = new double[numHidden];

            //
            // Small random values for initial weights & biases
            double low = -0.40;
            double high = +0.40;

            for (int i = 0; i < numVisible; ++i)
                for (int j = 0; j < numHidden; ++j)
                    vhWeights[i][j] = (high - low) * m_Rnd.NextDouble() + low;

            for (int i = 0; i < numVisible; ++i)
                visBiases[i] = (high - low) * m_Rnd.NextDouble() + low;

            for (int i = 0; i < numHidden; ++i)
                hidBiases[i] = (high - low) * m_Rnd.NextDouble() + low;
        }


        public override IScore Run(double[][] data, IContext ctx)
        {
            int[] indices = new int[data.Length];
            for (int i = 0; i < indices.Length; ++i)
                indices[i] = i;

            int epoch = 0;
            while (epoch < maxEpochs)
            {
                MathFunctions.Shuffle(indices);

                // 
                // Traversing through shuffled data.
                for (int idx = 0; idx < indices.Length; ++idx) 
                {
                    int i = indices[idx];  // i points to curr train data

                    // Copy train data to visible values
                    for (int j = 0; j < numVisible; ++j)
                        m_VisibleVals[j] = data[i][j];

                    //
                    // Compute hidden node values 
                    for (int hiddenIndx = 0; hiddenIndx < numHidden; ++hiddenIndx)
                    {
                        double sum = 0.0;
                        for (int visibleIndx = 0; visibleIndx < numVisible; ++visibleIndx)
                            sum += m_VisibleVals[visibleIndx] * vhWeights[visibleIndx][hiddenIndx];

                        sum += hidBiases[hiddenIndx]; // add the hidden bias
                        hidProbs[hiddenIndx] = m_ActivationFunction(sum); // compute prob of h activation
                        double pr = m_Rnd.NextDouble();  // determine 0/1 h node value
                        if (hidProbs[hiddenIndx] > pr)
                            hidValues[hiddenIndx] = 1;
                        else
                            hidValues[hiddenIndx] = 0;
                    }

                    // compute positive gradient =  outer product of v & h
                    double[][] positiveGradient = MathFunctions.OuterProd(m_VisibleVals, hidValues);

                    // reconstruct visual Nodes as v'
                    double[] vPrime = new double[numVisible];  // v' in Wikipedia
                    for (int v = 0; v < numVisible; ++v)
                    {
                        double sum = 0.0;
                        for (int h = 0; h < numHidden; ++h)
                            sum += hidValues[h] * vhWeights[v][h];
                        sum += visBiases[v]; // add visible bias
                        double probActiv = m_ActivationFunction(sum);
                        double pr = m_Rnd.NextDouble();
                        if (probActiv > pr)
                            vPrime[v] = 1;
                        else
                            vPrime[v] = 0;
                    }

                    //
                    // Compute new hidden Nodes as h', using v'
                    double[] hPrime = new double[numHidden];
                    for (int hiddenValIndx = 0; hiddenValIndx < numHidden; ++hiddenValIndx)
                    {
                        double sum = 0.0;
                        for (int v = 0; v < numVisible; ++v)
                            sum += vPrime[v] * vhWeights[v][hiddenValIndx];
                        sum += hidBiases[hiddenValIndx]; // add the hidden bias
                        double probActiv = m_ActivationFunction(sum); // apply activation
                        double pr = m_Rnd.NextDouble();  // determine 0/1 node value
                        if (probActiv > pr)
                            hPrime[hiddenValIndx] = 1;
                        else
                            hPrime[hiddenValIndx] = 0;
                    }

                    //
                    // Compute negative grad using v' and h'
                    double[][] negGrad = MathFunctions.OuterProd(vPrime, hPrime);

                    // Update weights
                    for (int row = 0; row < numVisible; ++row)
                        for (int col = 0; col < numHidden; ++col)
                            vhWeights[row][col] += learnRate * (positiveGradient[row][col] - negGrad[row][col]);

                    // Update visBiases
                    for (int v = 0; v < numVisible; ++v)
                        visBiases[v] += learnRate * (m_VisibleVals[v] - vPrime[v]);
                    // update hidBiases
                    for (int h = 0; h < numHidden; ++h)
                        hidBiases[h] += learnRate * (hidValues[h] - hPrime[h]);

                } 

                ++epoch;
            } 

            return null;
        }

        public static double logSig(double x)
        {
            if (x < -20.0) return 0.0000000001;
            else if (x > 20.0) return 0.9999999999;
            else return 1.0 / (1.0 + Math.Exp(-x));
        }
    }
}
