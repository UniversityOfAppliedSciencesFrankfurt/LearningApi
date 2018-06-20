using NeuralNetworks.Core;
using System;
using LearningFoundation;
using System.Diagnostics;
using System.IO;

//Recommendation: https://github.com/echen/restricted-boltzmann-machines
//DATASET: https://grouplens.org/datasets/movielens/

namespace NeuralNet.RestrictedBolzmannMachine2
{
    /// <summary>
    /// Nice article about RBM.
    /// https://deeplearning4j.org/restrictedboltzmannmachine
    /// </summary>
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


        /// <summary>
        /// Visible node values (0, 1)
        /// </summary>
        public double[] m_VisibleValues;
        public double[] visProbs;
        public double[] visBiases;

        public double[] hidValues;
        //public double[] hidProbs;
        public double[] hidBiases;

        public double[][] vhWeights;

        private Random m_Rnd = new Random();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="numVisible">Number of visible nodes.</param>
        /// <param name="numHidden">Number of hidden nodes.</param>
        public Rbm(int numVisible, int numHidden, int maxEpochs = 1000, double learnRate = 0.01)
        {
            // TODO: Do we need activation function
            //if (activationFnc == null)
            //    this.m_ActivationFunction = logSig;
            //else
            //    this.m_ActivationFunction = activationFnc;

            this.learnRate = learnRate;
            this.maxEpochs = maxEpochs;
            this.numVisible = numVisible;
            this.numHidden = numHidden;

            //
            // Allocate matrixes

            m_VisibleValues = new double[numVisible];
            visProbs = new double[numVisible];
            visBiases = new double[numVisible];

            hidValues = new double[numHidden];
            //hidProbs = new double[numHidden];
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
            double loss = 0;
            RbmScore score = new RbmScore();

            //if(m_WriteLossToFile)
            using (StreamWriter sw = new StreamWriter(File.Open("delta - gradient.csv", FileMode.Create)))
            {
                int[] indices = new int[data.Length];
                for (int i = 0; i < indices.Length; ++i)
                    indices[i] = i;

                int epoch = 0;
                while (epoch < maxEpochs)
                {
                    loss = 0;
                    MathFunctions.Shuffle(indices);

                    // 
                    // Traversing through shuffled data.
                    for (int idx = 0; idx < indices.Length; ++idx)
                    {
                        //Debug.WriteLine($"---- epoch: {epoch} ---{idx} of {indices.Length} ------");

                        int i = indices[idx];  // i points to curr train data

                        //
                        // Copy train data to visible values.
                        // It copies input vector to member m_VisibleValues
                        for (int j = 0; j < numVisible; ++j)
                            m_VisibleValues[j] = data[i][j];

                        //
                        // Compute hidden node values for current input vector.
                        for (int hiddenIndx = 0; hiddenIndx < numHidden; ++hiddenIndx)
                        {
                            double sum = 0.0;

                            for (int visibleIndx = 0; visibleIndx < numVisible; ++visibleIndx)
                                sum += m_VisibleValues[visibleIndx] * vhWeights[visibleIndx][hiddenIndx];

                            sum += hidBiases[hiddenIndx]; // add the hidden bias
                                                          //hidProbs[hiddenIndx] = m_ActivationFunction(sum); // compute prob of h activation
                                                          //double pr = m_Rnd.NextDouble();  // determine 0/1 h node value
                                                          //if (hidProbs[hiddenIndx] > pr)
                                                          //    hidValues[hiddenIndx] = 1;
                                                          //else
                                                          //    hidValues[hiddenIndx] = 0;

                            var sumPrime = m_ActivationFunction(sum);

                            //if (sumPrime > 0.5)
                            //    hidValues[hiddenIndx] = 1;
                            //else
                            //    hidValues[hiddenIndx] = 0;

                            double pr = m_Rnd.NextDouble();  // determine 0/1 h node value
                            if (sumPrime > pr)
                                hidValues[hiddenIndx] = 1;
                            else
                                hidValues[hiddenIndx] = 0;
                        }

                        // compute positive gradient =  outer product of v & h
                        double[][] posGrad = MathFunctions.OuterProd(m_VisibleValues, hidValues);

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

                        //printVector("Weights -1", vhWeights);

                        //
                        // Compute negative grad using v' and h'
                        double[][] negGrad = MathFunctions.OuterProd(vPrime, hPrime);

#if DEBUG
                        var val = calcDelta(posGrad, negGrad);
                        loss += val / (numHidden * numVisible);
#endif
                        // printVector("PosGrad", posGrad);
                        // printVector("NegGrad", negGrad);

                        // Update weights
                        for (int row = 0; row < numVisible; ++row)
                            for (int col = 0; col < numHidden; ++col)
                                vhWeights[row][col] += learnRate * (posGrad[row][col] - negGrad[row][col]);

                        // Update visBiases
                        for (int v = 0; v < numVisible; ++v)
                            visBiases[v] += learnRate * (m_VisibleValues[v] - vPrime[v]);
                        // update hidBiases
                        for (int h = 0; h < numHidden; ++h)
                            hidBiases[h] += learnRate * (hidValues[h] - hPrime[h]);

                        // Include these lines to print out internal result.
                        //printVector("Visible", m_VisibleValues);
                        //printVector("Hidden", hidValues);
                        //printVector("Weights", vhWeights);
                    }

                    sw.WriteLine($"{loss / indices.Length};{epoch}");
                    // Loss of iteration is calculated as 
                    // SUM(posGrad-negGrad) / number of training vectors
                    Debug.WriteLine($"loss: {loss / indices.Length}");

                    score.Loss = loss / indices.Length;

                    ++epoch;
                }
            }

            return score;
        }


        /// <summary>
        /// Calculates delta betwen positive and negative gradient
        /// </summary>
        /// <param name="positiveGradient"></param>
        /// <param name="negGrad"></param>
        private double calcDelta(double[][] positiveGradient, double[][] negGrad)
        {
            double sum = 0;
            for (int i = 0; i < positiveGradient.Length; i++)
            {
                for (int j = 0; j < positiveGradient[i].Length; j++)
                {
                    sum += Math.Abs(positiveGradient[i][j] - negGrad[i][j]);
                }
            }

            return sum;
        }

        private void printVector(string name, double[] v)
        {
            Debug.WriteLine("");
            Debug.Write($"{name}\t");
            foreach (var item in v)
            {
                Debug.Write($"{item}, ");
            }
        }


        public static double logSig(double x)
        {
            if (x < -20.0) return 0.0000000001;
            else if (x > 20.0) return 0.9999999999;
            else return 1.0 / (1.0 + Math.Exp(-x));
        }

        public override IResult Predict(double[][] data, IContext ctx)
        {
            RbmResult res = new RbmResult()
            {
                HiddenNodesPredictions = new double[data.Length][],
                VisibleNodesPredictions = new double[data.Length][],
                Weights = vhWeights,
            };

            for (int i = 0; i < data.Length; i++)
            {
                res.HiddenNodesPredictions[i] = new double[numHidden];

                for (int h = 0; h < numHidden; ++h)
                {
                    double sum = 0.0;
                    for (int v = 0; v < numVisible; ++v)
                        sum += data[i][v] * vhWeights[v][h];

                    sum += hidBiases[h]; // add the hidden bias
                    double probActiv = m_ActivationFunction(sum); // compute prob of h activation
                                                                  // Console.WriteLine("Hidden [" + h + "] activation probability = " + probActiv.ToString("F4"));
                    double pr = m_Rnd.NextDouble();  // determine 0/1 h node value
                    if (probActiv > pr)
                        res.HiddenNodesPredictions[i][h] = 1;
                    else
                        res.HiddenNodesPredictions[i][h] = 0;
                }

                res.VisibleNodesPredictions[i] = calcVisibleFromHidden(res.HiddenNodesPredictions[i]);
            }

            return res;
        }

        private double[] calcVisibleFromHidden(double[] hiddens)
        {
            double[] result = new double[numVisible];

            for (int v = 0; v < numVisible; ++v)
            {
                double sum = 0.0;
                for (int h = 0; h < numHidden; ++h)
                    sum += hiddens[h] * vhWeights[v][h];
                // sum up visible bias
                sum += visBiases[v];
                double probActiv = m_ActivationFunction(sum);
                double pr = m_Rnd.NextDouble();
                if (probActiv > pr)
                    result[v] = 1;
                else
                    result[v] = 0;
            }
            return result;
        }


        private void printVector(string name, double[][] vector)
        {
            Debug.WriteLine("");
            Debug.WriteLine(name);
            for (int row = 0; row < vector.Length; ++row)
            {
                Debug.WriteLine("");
                for (int col = 0; col < vector[row].Length; ++col)
                    Debug.Write($"{vector[row][col]:F3}\t");
            }

            Debug.WriteLine("");
        }


        //private void printOut()
        //{
        //    Debug.WriteLine("");

        //    for (int row = 0; row < numVisible; ++row)
        //    {
        //        Debug.WriteLine("");
        //        for (int col = 0; col < numHidden; ++col)
        //            Debug.Write($"{vhWeights[row][col]:F3}\t");
        //    }

        //    Debug.WriteLine("");
        //}


    }
}
