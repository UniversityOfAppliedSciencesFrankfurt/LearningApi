using NeuralNetworks.Core;
using System;
using LearningFoundation;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

//Recommendation: https://github.com/echen/restricted-boltzmann-machines
//DATASET: https://grouplens.org/datasets/movielens/
//Nice article about RBM.: https://deeplearning4j.org/restrictedboltzmannmachine
namespace NeuralNet.RestrictedBolzmannMachine2
{
    /// <summary>
    /// Implements RBM algorithm.
    /// </summary>
    public class Rbm : NeuralNetCore
    {
        protected double learnRate;

        private int maxEpochs;

        /// <summary>
        /// Number of visible nodes.
        /// </summary>
        protected int numVisible;

        /// <summary>
        /// Number of hidden nodes.
        /// </summary>
        protected int m_NumHidden;

        private Func<double, double> m_ActivationFunction = logSig;

        /// <summary>
        /// Visible node values (0, 1)
        /// </summary>
        public double[] m_VisibleValues;
        public double[] m_VisProbs;
        public double[] m_VisBiases;

        public double[] m_HidValues;
    
        public double[] m_HidBiases;

        public double[][] m_VH_Weights;

        private Random m_Rnd = new Random();

        // Low and High Extracted as constants
        private const double LOW = -0.40;
        private const double HIGH = +0.40;

        protected double[] vPrime, hPrime;


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
            this.m_NumHidden = numHidden;

            //
            // Allocate matrixes

            m_VisibleValues = new double[numVisible];
            m_VisProbs = new double[numVisible];
            m_VisBiases = new double[numVisible];

            m_HidValues = new double[numHidden];
  
            m_HidBiases = new double[numHidden];

            InitializeWeightAndBiasVectors();          
        }


        private void InitializeWeightAndBiasVectors()
        {
            m_VH_Weights = new double[numVisible][];  // visible-to-hidden
            for (int i = 0; i < numVisible; ++i)
                m_VH_Weights[i] = new double[m_NumHidden];

            for (int i = 0; i < numVisible; ++i)
                for (int j = 0; j < m_NumHidden; ++j)
                    m_VH_Weights[i][j] = generateRandomWeightOrBias();

            for (int i = 0; i < numVisible; ++i)
                m_VisBiases[i] = generateRandomWeightOrBias();

            for (int i = 0; i < m_NumHidden; ++i)
                m_HidBiases[i] = generateRandomWeightOrBias();
        }


        protected double generateRandomWeightOrBias()
        {
            return (HIGH - LOW) * m_Rnd.NextDouble() + LOW;
        }

        /// <summary>
        /// Runs RMB algorithm.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public override IScore Run(double[][] data, IContext ctx)
        {
            double loss = 0;
            RbmScore score = new RbmScore();
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
                    int i = indices[idx];  // i points to curr train data

                    //
                    // Copy train data to visible values.
                    // It copies input vector to member m_VisibleValues
                    for (int j = 0; j < numVisible; ++j)
                        m_VisibleValues[j] = data[i][j];

                    computeHiddenValues();

                    // compute positive gradient =  outer product of v & h
                    double[][] posGrad = MathFunctions.OuterProd(m_VisibleValues, m_HidValues);

                    // Compute reconstructed V' nodes.
                    vPrime = computeVFromH();

                    hPrime = computeHPrimeFromVPrime(vPrime);

                    //
                    // Compute negative grad using v' and h'
                    double[][] negGrad = MathFunctions.OuterProd(vPrime, hPrime);

                    var val = calcDelta(posGrad, negGrad);
                    loss += val / (m_NumHidden * numVisible);

                    // Update weights
                    updateWeights(posGrad, negGrad);
                   
                    // Update visBiases
                    for (int v = 0; v < numVisible; ++v)
                        m_VisBiases[v] += learnRate * (m_VisibleValues[v] - vPrime[v]);
                   
                    // update hidBiases
                    for (int h = 0; h < m_NumHidden; ++h)
                        m_HidBiases[h] += learnRate * (m_HidValues[h] - hPrime[h]);                
                }


                // Loss of iteration is calculated as 
                // SUM(posGrad-negGrad) / number of training vectors
                Debug.WriteLine($"loss: {loss / indices.Length}");

                score.Loss = loss / indices.Length;

                ++epoch;

            }

            score.HiddenValues = new List<double>(this.m_HidValues).ToArray();
            score.HiddenBisases = new List<double>(this.m_HidBiases).ToArray();
            score.Weights = new List<double[]>(this.m_VH_Weights).ToArray();

            return score;
        }

        protected void updateWeights(double[][] posGrad, double[][] negGrad)
        {
            for (int row = 0; row < numVisible; ++row)
                for (int col = 0; col < m_NumHidden; ++col)
                    m_VH_Weights[row][col] += learnRate * (posGrad[row][col] - negGrad[row][col]);
        }


        protected double[] computeHPrimeFromVPrime(double[] vPrime)
        {
            //
            // Compute new hidden Nodes as h', using v'
            double[] hPrime = new double[m_NumHidden];
            for (int hiddenValIndx = 0; hiddenValIndx < m_NumHidden; ++hiddenValIndx)
            {
                /*
                double sum = 0.0;
                for (int v = 0; v < numVisible; ++v)
                    sum += vPrime[v] * vhWeights[v][hiddenValIndx];
                */

                double sum = calculateSumForHPrimeFromVPrime(vPrime, hiddenValIndx);
                sum += m_HidBiases[hiddenValIndx]; // add the hidden bias
                double probActiv = m_ActivationFunction(sum); // apply activation
              
                double pr = m_Rnd.NextDouble();  // determine 0/1 node value
                //if (probActiv > pr)
                //    hPrime[hiddenValIndx] = 1;
                //else
                //  hPrime[hiddenValIndx] = 0;
    
                //new tria by providing value instead of random
                if (probActiv > pr)
                    hPrime[hiddenValIndx] = 1;
                else
                  hPrime[hiddenValIndx] = 0;
            }

            return hPrime;
        }


        protected double calculateSumForHPrimeFromVPrime(double[] vPrime, int hiddenValIndx)
        {
            double sum = 0.0;
            for (int v = 0; v < numVisible; ++v)
                sum += vPrime[v] * m_VH_Weights[v][hiddenValIndx];
            return sum;
        }


        protected double[] computeVFromH()
        {
            // reconstruct visual Nodes as v'
            double[] vPrime = new double[numVisible];  // v' in Wikipedia
            for (int v = 0; v < numVisible; ++v)
            {
                double sum = calculateSumForVPrimeFromH(v);

                sum += m_VisBiases[v]; // add visible bias

                double probActiv = m_ActivationFunction(sum);

                vPrime[v] = createBinaryValueFromRandom(probActiv);              
            }

            return vPrime;
        }

        private double createBinaryValueFromRandom(double val)
        {
            double result;

            double pr = m_Rnd.NextDouble();
            if (val > pr)
                result = 1;
            else
                result = 0;

            return result;
        }

        protected double calculateSumForVPrimeFromH(int visibleIndex)
        {
            double sum = 0.0;
            for (int h = 0; h < m_NumHidden; ++h)
                sum += m_HidValues[h] * m_VH_Weights[visibleIndex][h];

            return sum;
        }


        /// <summary>
        /// Computes hidden values: H = V * W
        /// </summary>
        private void computeHiddenValues()
        {
            //
            // Compute hidden node values for current input vector.
            for (int hiddenIndx = 0; hiddenIndx < m_NumHidden; ++hiddenIndx)
            {
                double sum = calculateSumForHiddenIndex(hiddenIndx);
                var sumPrime = m_ActivationFunction(sum);

                //if (sumPrime > 0.5)
                //    hidValues[hiddenIndx] = 1;
                //else
                //    hidValues[hiddenIndx] = 0;
               
                double pr = m_Rnd.NextDouble();  // determine 0/1 h node value
                //if (sumPrime > pr)
                  //  m_HidValues[hiddenIndx] = 1;
                //else
                  //  m_HidValues[hiddenIndx] = 0;

                //m_HidValues[hiddenIndx] = sumPrime;
                if (sumPrime > pr)
                    m_HidValues[hiddenIndx] = 1;
                else
                    m_HidValues[hiddenIndx] = 0;

            }
        }

        /// <summary>
        /// SUM = Foreach(SUM+=VisibleNode[vi] * WEIGHT[vi][GivenHiddenIndex])
        /// </summary>
        /// <param name="hiddenIndx"></param>
        /// <returns></returns>
        protected double calculateSumForHiddenIndex(int hiddenIndx)
        {
            double sum = 0.0;

            for (int visibleIndx = 0; visibleIndx < numVisible; ++visibleIndx)
                sum += m_VisibleValues[visibleIndx] * m_VH_Weights[visibleIndx][hiddenIndx];

            sum += m_HidBiases[hiddenIndx]; 
                                        
            return sum;
        }

//    Original method before refactoring. To be removed.
//        public IScore Run2222(double[][] data, IContext ctx)
//        {
//            double loss = 0;
//            RbmScore score = new RbmScore();

//            //if(m_WriteLossToFile)
//            using (StreamWriter sw = new StreamWriter(File.Open("delta - gradient.csv", FileMode.Create)))
//            {
//                int[] indices = new int[data.Length];
//                for (int i = 0; i < indices.Length; ++i)
//                    indices[i] = i;

//                int epoch = 0;
//                while (epoch < maxEpochs)
//                {
//                    loss = 0;
//                    MathFunctions.Shuffle(indices);

//                    // 
//                    // Traversing through shuffled data.
//                    for (int idx = 0; idx < indices.Length; ++idx)
//                    {
//                        //Debug.WriteLine($"---- epoch: {epoch} ---{idx} of {indices.Length} ------");

//                        int i = indices[idx];  // i points to curr train data

//                        //
//                        // Copy train data to visible values.
//                        // It copies input vector to member m_VisibleValues
//                        for (int j = 0; j < numVisible; ++j)
//                            m_VisibleValues[j] = data[i][j];

//                        //
//                        // Compute hidden node values for current input vector.
//                        for (int hiddenIndx = 0; hiddenIndx < m_NumHidden; ++hiddenIndx)
//                        {
//                            double sum = 0.0;

//                            for (int visibleIndx = 0; visibleIndx < numVisible; ++visibleIndx)
//                                sum += m_VisibleValues[visibleIndx] * vhWeights[visibleIndx][hiddenIndx];

//                            sum += hidBiases[hiddenIndx]; // add the hidden bias
//                                                          //hidProbs[hiddenIndx] = m_ActivationFunction(sum); // compute prob of h activation
//                                                          //double pr = m_Rnd.NextDouble();  // determine 0/1 h node value
//                                                          //if (hidProbs[hiddenIndx] > pr)
//                                                          //    hidValues[hiddenIndx] = 1;
//                                                          //else
//                                                          //    hidValues[hiddenIndx] = 0;

//                            var sumPrime = m_ActivationFunction(sum);

//                            //if (sumPrime > 0.5)
//                            //    hidValues[hiddenIndx] = 1;
//                            //else
//                            //    hidValues[hiddenIndx] = 0;

//                            double pr = m_Rnd.NextDouble();  // determine 0/1 h node value
//                            if (sumPrime > pr)
//                                m_HidValues[hiddenIndx] = 1;
//                            else
//                                m_HidValues[hiddenIndx] = 0;

//                            //m_HidValues[hiddenIndx] = sumPrime;
//                        }

//                        // compute positive gradient =  outer product of v & h
//                        double[][] posGrad = MathFunctions.OuterProd(m_VisibleValues, m_HidValues);

//                        // reconstruct visual Nodes as v'
//                        double[] vPrime = new double[numVisible];  // v' in Wikipedia
//                        for (int v = 0; v < numVisible; ++v)
//                        {
//                            double sum = 0.0;
//                            for (int h = 0; h < m_NumHidden; ++h)
//                                sum += m_HidValues[h] * vhWeights[v][h];
//                            sum += visBiases[v]; // add visible bias
//                            double probActiv = m_ActivationFunction(sum);

//                            double pr = m_Rnd.NextDouble();
//                            if (probActiv > pr)
//                                vPrime[v] = 1;
//                            else
//                                vPrime[v] = 0;

//                            //vPrime[v] = probActiv;
//                        }

//                        //
//                        // Compute new hidden Nodes as h', using v'
//                        double[] hPrime = new double[m_NumHidden];
//                        for (int hiddenValIndx = 0; hiddenValIndx < m_NumHidden; ++hiddenValIndx)
//                        {
//                            double sum = 0.0;
//                            for (int v = 0; v < numVisible; ++v)
//                                sum += vPrime[v] * vhWeights[v][hiddenValIndx];
//                            sum += hidBiases[hiddenValIndx]; // add the hidden bias
//                            double probActiv = m_ActivationFunction(sum); // apply activation

//                            double pr = m_Rnd.NextDouble();  // determine 0/1 node value
//                            if (probActiv > pr)
//                                hPrime[hiddenValIndx] = 1;
//                            else
//                                hPrime[hiddenValIndx] = 0;
                            
//                            //hPrime[hiddenValIndx] = probActiv;
//                        }

//                        //printVector("Weights -1", vhWeights);

//                        //
//                        // Compute negative grad using v' and h'
//                        double[][] negGrad = MathFunctions.OuterProd(vPrime, hPrime);

//#if DEBUG
//                        var val = calcDelta(posGrad, negGrad);
//                        loss += val / (m_NumHidden * numVisible);
                        
//#endif
//                        // printVector("PosGrad", posGrad);
//                        // printVector("NegGrad", negGrad);

//                        // Update weights
//                        for (int row = 0; row < numVisible; ++row)
//                            for (int col = 0; col < m_NumHidden; ++col)
//                                vhWeights[row][col] += learnRate * (posGrad[row][col] - negGrad[row][col]);

//                        // Update visBiases
//                        for (int v = 0; v < numVisible; ++v)
//                            visBiases[v] += learnRate * (m_VisibleValues[v] - vPrime[v]);
//                        // update hidBiases
//                        for (int h = 0; h < m_NumHidden; ++h)
//                            hidBiases[h] += learnRate * (m_HidValues[h] - hPrime[h]);

//                        // Include these lines to print out internal result.
//                        //printVector("Visible", m_VisibleValues);
//                        //printVector("Hidden", hidValues);
//                        //printVector("Weights", vhWeights);
//                    }

//                    sw.WriteLine($"{loss / indices.Length};{epoch}");
//                    // Loss of iteration is calculated as 
//                    // SUM(posGrad-negGrad) / number of training vectors
//                    Debug.WriteLine($"loss: {loss / indices.Length}");

//                    score.Loss = loss / indices.Length;

//                    ++epoch;
//                }
//            }

//            score.HiddenValues = new List<double>(this.m_HidValues).ToArray();
//            score.HiddenBisases = new List<double>(this.hidBiases).ToArray();
//            score.Weights = new List<double[]>(this.vhWeights).ToArray();

//            return score;
//        }


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
                Weights = m_VH_Weights,
            };

            for (int i = 0; i < data.Length; i++)
            {
                res.HiddenNodesPredictions[i] = new double[m_NumHidden];

                for (int h = 0; h < m_NumHidden; ++h)
                {
                    double sum = 0.0;
                    for (int v = 0; v < numVisible; ++v)
                        sum += data[i][v] * m_VH_Weights[v][h];

                    sum += m_HidBiases[h]; // add the hidden bias
                    double probActiv = m_ActivationFunction(sum); // compute prob of h activation
                                                                  // Console.WriteLine("Hidden [" + h + "] activation probability = " + probActiv.ToString("F4"));

                    double pr = m_Rnd.NextDouble();  // determine 0/1 h node value
                                                     //if (probActiv > pr)
                                                     //  res.HiddenNodesPredictions[i][h] = 1;
                                                     //else
                                                     //  res.HiddenNodesPredictions[i][h] = 0;

                    //res.HiddenNodesPredictions[i][h] = probActiv;
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
                for (int h = 0; h < m_NumHidden; ++h)
                    sum += hiddens[h] * m_VH_Weights[v][h];
                // sum up visible bias
                sum += m_VisBiases[v];
                double probActiv = m_ActivationFunction(sum);
                double pr = m_Rnd.NextDouble();
                if (probActiv > pr)
                   result[v] = 1;
                else
                    result[v] = 0;

                //result[v] = probActiv;
                //if (probActiv < 0.21)
                  //  result[v] = 1;
                //else if (probActiv > 0.20 && probActiv < 0.41)
                //{
                  //  result[v] = 2;
                //}
                //else if (probActiv > 0.40 && probActiv < 0.61)
                //{
                  //  result[v] = 3;
                //}
                //else if (probActiv > 0.60 && probActiv < 0.81)
                //{
                  //  result[v] = 4;
                //}
                //else
                  //  result[v] = 5;
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
