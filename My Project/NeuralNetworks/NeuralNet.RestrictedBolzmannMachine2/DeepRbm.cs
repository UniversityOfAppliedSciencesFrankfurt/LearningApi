using NeuralNetworks.Core;
using System;
using LearningFoundation;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

//Recommendation: https://github.com/echen/restricted-boltzmann-machines
//DATASET: https://grouplens.org/datasets/movielens/

namespace NeuralNet.RestrictedBolzmannMachine2
{

    /// <summary>
    /// Nice article about RBM.
    /// https://deeplearning4j.org/restrictedboltzmannmachine
    /// </summary>
    public class DeepRbm : NeuralNetCore
    {
        private double learnRate;

        private int maxEpochs;

        private Func<double, double> m_ActivationFunction = logSig;

        public RbmLayer[] Layers { get; set; }

        private Random m_Rnd = new Random();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="numVisible">Number of visible nodes.</param>
        /// <param name="numHidden">Number of hidden nodes.</param>
        public DeepRbm(int[] layerDims, int maxEpochs = 1000, double learnRate = 0.01)
        {
            this.learnRate = learnRate;
            this.maxEpochs = maxEpochs;

            this.Layers = new RbmLayer[layerDims.Length - 1];
            for (int i = 0; i <= this.Layers.Length - 1; i++)
            {
                this.Layers[i] = new RbmLayer()
                {
                    NumVisible = layerDims[i],
                    //VisProbs = new double[layerDims[i]],

                    NumHidden = layerDims[i + 1],
                    HidValues = new double[layerDims[i + 1]],
                    HidBiases = new double[layerDims[i + 1]],

                    VHWeights = new double[layerDims[i]][],  // visible-to-hidden
                };

                if (i == 0)
                {
                    this.Layers[i].VisibleValues = new double[layerDims[i]];
                    //this.Layers[i].VisProbs = new double[layerDims[i]];
                    this.Layers[i].VisBiases = new double[layerDims[i]];
                }
                else
                {
                    this.Layers[i].VisibleValues = this.Layers[i - 1].HidValues;
                    this.Layers[i].VisBiases = this.Layers[i - 1].HidBiases;
                }

                for (int k = 0; k < this.Layers[i].NumVisible; ++k)
                    this.Layers[i].VHWeights[k] = new double[this.Layers[i].NumHidden];

                //
                // Small random values for initial weights & biases
                double low = -0.40;
                double high = +0.40;

                for (int k = 0; k < this.Layers[i].NumVisible; ++k)
                    for (int j = 0; j < this.Layers[i].NumHidden; ++j)
                        this.Layers[i].VHWeights[k][j] = (high - low) * m_Rnd.NextDouble() + low;

                for (int k = 0; k < this.Layers[i].NumVisible; ++k)
                    this.Layers[i].VisBiases[k] = (high - low) * m_Rnd.NextDouble() + low;

                for (int k = 0; k < this.Layers[i].NumHidden; ++k)
                    this.Layers[i].HidBiases[k] = (high - low) * m_Rnd.NextDouble() + low;
            }
        }


        public override IScore Run(double[][] data, IContext ctx)
        {
            double loss = 0;
            RbmDeepScore score = new RbmDeepScore() { Layers = new List<RbmLayer>(this.Layers) };

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
                    int layerIndx = 0;
                    foreach (var layer in this.Layers)
                    {
                        int i = indices[idx];  // i points to curr train data

                        //
                        // Copy train data to visible values.
                        // It copies input vector to member m_VisibleValues
                        for (int j = 0; j < layer.NumVisible; ++j)
                            layer.VisibleValues[j] = data[i][j];

                        computeHiddenValues(layer);

                        // compute positive gradient =  outer product of v & h
                        double[][] posGrad = MathFunctions.OuterProd(layer.VisibleValues, layer.HidValues);

                        // Compute reconstructed V' nodes.
                        double[] vPrime = computeVFromH(layer);

                        double[] hPrime = computeHPrimeFromVPrime(vPrime, layer);

                        //printVector("Weights -1", vhWeights);

                        //
                        // Compute negative grad using v' and h'
                        double[][] negGrad = MathFunctions.OuterProd(vPrime, hPrime);

                        var val = calcDelta(posGrad, negGrad);
                        loss += val / (layer.NumHidden * layer.NumVisible);

                        // Update weights
                        for (int row = 0; row < layer.NumVisible; ++row)
                            for (int col = 0; col < layer.NumHidden; ++col)
                                layer.VHWeights[row][col] += learnRate * (posGrad[row][col] - negGrad[row][col]);

                        // Update visBiases
                        for (int v = 0; v < layer.NumVisible; ++v)
                            layer.VisBiases[v] += learnRate * (layer.VisibleValues[v] - vPrime[v]);
                        // update hidBiases
                        for (int h = 0; h < layer.NumHidden; ++h)
                            layer.HidBiases[h] += learnRate * (layer.HidValues[h] - hPrime[h]);

                        layer.Loss = loss / indices.Length;
                        score.Layers[layerIndx] = layer;
                       // score.Layers.Add(layer);

                        layerIndx++;
                    }

                    // Loss of iteration is calculated as 
                    // SUM(posGrad-negGrad) / number of training vectors
                    Debug.WriteLine($"loss: {loss / indices.Length / this.Layers.Length}");

                    ++epoch;
                }
            }



            return score;
        }

        public override IResult Predict(double[][] data, IContext ctx)
        {
            RbmDeepResult res = new RbmDeepResult()
            {
                Results = new List<List<RbmLayerResult>>(),
            };

            for (int i = 0; i < data.Length; i++)
            {
                List<RbmLayerResult> results = new List<RbmLayerResult>();

                int lyrIndx = 0;
                foreach (var layer in this.Layers)
                {
                    RbmLayerResult lyrRes = new RbmLayerResult();

                    lyrRes.HiddenNodesPredictions = new double[layer.NumHidden];

                    for (int h = 0; h < layer.NumHidden; ++h)
                    {
                        double sum = 0.0;
                        for (int v = 0; v < layer.NumVisible; ++v)
                            sum += data[i][v] * layer.VHWeights[v][h];

                        sum += layer.HidBiases[h]; // add the hidden bias
                        double probActiv = m_ActivationFunction(sum); // compute prob of h activation

                        double pr = m_Rnd.NextDouble();  // determine 0/1 h node value
                        if (probActiv > 0.5)
                            lyrRes.HiddenNodesPredictions[h] = 1;
                        else
                            lyrRes.HiddenNodesPredictions[h] = 0;
                    }

                    lyrRes.VisibleNodesPredictions = calcVisibleFromHidden(lyrRes.HiddenNodesPredictions, layer);

                    results.Add(lyrRes);

                    lyrIndx++;
                }

                res.Results.Add(results);
            }

            return res;
        }


        private double[] computeHPrimeFromVPrime(double[] vPrime, RbmLayer layer)
        {
            //
            // Compute new hidden Nodes as h', using v'
            double[] hPrime = new double[layer.NumHidden];
            for (int hiddenValIndx = 0; hiddenValIndx < layer.NumHidden; ++hiddenValIndx)
            {
                double sum = 0.0;
                for (int v = 0; v < layer.NumVisible; ++v)
                    sum += vPrime[v] * layer.VHWeights[v][hiddenValIndx];
                sum += layer.HidBiases[hiddenValIndx]; // add the hidden bias
                double probActiv = m_ActivationFunction(sum); // apply activation
                double pr = m_Rnd.NextDouble();  // determine 0/1 node value
                if (probActiv > 0.5)
                    hPrime[hiddenValIndx] = 1;
                else
                    hPrime[hiddenValIndx] = 0;
            }

            return hPrime;
        }

        private double[] computeVFromH(RbmLayer layer)
        {
            // reconstruct visual Nodes as v'
            double[] vPrime = new double[layer.NumVisible];  // v' in Wikipedia
            for (int v = 0; v < layer.NumVisible; ++v)
            {
                double sum = 0.0;
                for (int h = 0; h < layer.NumHidden; ++h)
                    sum += layer.HidValues[h] * layer.VHWeights[v][h];
                sum += layer.VisBiases[v]; // add visible bias
                double probActiv = m_ActivationFunction(sum);
                double pr = m_Rnd.NextDouble();
                if (probActiv > 0.5)
                    vPrime[v] = 1;
                else
                    vPrime[v] = 0;
            }

            return vPrime;
        }


        /// <summary>
        /// Computes hidden values: H = V * W
        /// </summary>
        private void computeHiddenValues(RbmLayer layer)
        {
            //
            // Compute hidden node values for current input vector.
            for (int hiddenIndx = 0; hiddenIndx < layer.NumHidden; ++hiddenIndx)
            {
                double sum = 0.0;

                for (int visibleIndx = 0; visibleIndx < layer.NumVisible; ++visibleIndx)
                    sum += layer.VisibleValues[visibleIndx] * layer.VHWeights[visibleIndx][hiddenIndx];

                sum += layer.HidBiases[hiddenIndx]; // add the hidden bias
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
                if (sumPrime > 0.5)
                    layer.HidValues[hiddenIndx] = 1;
                else
                    layer.HidValues[hiddenIndx] = 0;
            }
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

        private double[] calcVisibleFromHidden(double[] hiddens, RbmLayer layer)
        {
            double[] result = new double[layer.NumVisible];

            for (int v = 0; v < layer.NumVisible; ++v)
            {
                double sum = 0.0;
                for (int h = 0; h < layer.NumHidden; ++h)
                    sum += hiddens[h] * layer.VHWeights[v][h];
                // sum up visible bias
                sum += layer.VisBiases[v];
                double probActiv = m_ActivationFunction(sum);
                double pr = m_Rnd.NextDouble();
                if (probActiv > 0.5)
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
